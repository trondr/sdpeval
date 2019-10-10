namespace sdpeval.tests

module F =    
    open System
    type ThisAssembly = { Empty:string;}

    let rec getAccumulatedExceptionMessages (ex: Exception) =
                match ex.InnerException with
                | null -> ex.Message
                | _ -> ex.Message + " " + (getAccumulatedExceptionMessages ex.InnerException)

    let sourceException ex = 
        System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(ex).SourceException

    let toException message (innerException: System.Exception option) =
        match innerException with
        |Some iex ->
            (new System.Exception(message, sourceException iex))            
        |None ->
            (new System.Exception(message))

    let toErrorResult message (innerException: System.Exception option) =
        Result.Error (toException message innerException)

    let tryCatch<'T, 'R> f  (t:'T) : Result<'R, Exception> =
        try
            Result.Ok (f t)
        with
            | ex -> Result.Error (sourceException ex)

    //Source: http://www.fssnip.net/7UJ/title/ResultBuilder-Computational-Expression
    let ofOption error = function Some s -> Ok s | None -> Error error
    //Source: http://www.fssnip.net/7UJ/title/ResultBuilder-Computational-Expression
    type ResultBuilder() =
        member __.Return(x) = Ok x

        member __.ReturnFrom(m: Result<_, _>) = m

        member __.Bind(m, f) = Result.bind f m
        member __.Bind((m, error): (Option<'T> * 'E), f) = m |> ofOption error |> Result.bind f

        member __.Zero() = None

        member __.Combine(m, f) = Result.bind f m

        member __.Delay(f: unit -> _) = f

        member __.Run(f) = f()

        member __.TryWith(m, h) =
            try __.ReturnFrom(m)
            with e -> h e

        member __.TryFinally(m, compensation) =
            try __.ReturnFrom(m)
            finally compensation()

        member __.Using(res:#IDisposable, body) =
            __.TryFinally(body res, fun () -> match res with null -> () | disp -> disp.Dispose())

        member __.While(guard, f) =
            if not (guard()) then Ok () else
            do f() |> ignore
            __.While(guard, f)

        member __.For(sequence:seq<_>, body) =
            __.Using(sequence.GetEnumerator(), fun enum -> __.While(enum.MoveNext, __.Delay(fun () -> body enum.Current)))

    let result = new ResultBuilder()

    //Source: http://www.fssnip.net/7UQ/title/Null-Value-guard-active-pattern
    let (|Null|NotNull|) (x : 'T when 'T : not struct) =
        if obj.ReferenceEquals(x, null) then Null else NotNull x

    let nullGuard (obj: 'T when 'T : not struct) (argumentName:string) =
        match obj with
        |Null -> raise (new ArgumentNullException("Value cannot be null.",argumentName))
        |NotNull _ -> ()

    let createWithContinuationGeneric success failure validator (value:'T) : Result<'T,Exception> = 
                match value with
                |Null -> failure (new ArgumentNullException("value","Value cannot be null.") :> Exception)
                |NotNull v -> 
                    let result = validator value
                    match result with
                    |Ok vr -> success vr
                    |Error ex -> failure ex   
    
    let createGeneric validator (value:'T) =
        let success v = Result.Ok v
        let failure ex = Result.Error (sourceException ex)
        createWithContinuationGeneric success failure validator value 

    type Msg = System.Action<Common.Logging.FormatMessageHandler>
    let msg message =
        new Msg(fun m -> m.Invoke(message)|>ignore)

    let directoryPathExists (directoryPath) =
        System.IO.Directory.Exists(directoryPath)

    let createDirectoryUnsafe (directoryPath) =
        System.IO.Directory.CreateDirectory(directoryPath) |> ignore
        directoryPath

    let createDirectory (directoryPath) =
        try
            Result.Ok (createDirectoryUnsafe directoryPath)
        with
        | ex -> Result.Error (new Exception(sprintf "Failed to create directory '%s'" (directoryPath),ex))

    let ensureDirectoryExistsWithMessage createIfNotExists message directoryPath =
        let directoryExists = 
            directoryPathExists directoryPath
        match (not directoryExists && createIfNotExists) with
        |true->            
            createDirectory directoryPath
        |false->
           match directoryExists with
           | true -> Result.Ok directoryPath
           | false -> Result.Error (new Exception(sprintf "Directory not found: '%s'. %s" (directoryPath) message))

    let ensureDirectoryExists createIfNotExists directoryPath =
        ensureDirectoryExistsWithMessage createIfNotExists String.Empty directoryPath
    
    let deleteDirectoryUnsafe force (folderPath) =
        match (System.IO.Directory.Exists(folderPath)) with
        |true -> 
            System.IO.Directory.Delete(folderPath, force)
            folderPath
        |false -> 
            folderPath

    let tryCatch2<'T1,'T2, 'R> f  (t1:'T1) (t2:'T2) : Result<'R, Exception> =
        try
            Result.Ok (f t1 t2)
        with
            | ex -> Result.Error (sourceException ex)

    let deleteDirectory force (folderPath) =
        tryCatch2 deleteDirectoryUnsafe force folderPath

    let deleteDirectoryIfExists folderPath =
        match (directoryPathExists folderPath) with
        |true -> 
            deleteDirectory true folderPath                                    
        |false -> 
            Result.Ok folderPath

    [<AllowNullLiteral>]
    type TemporaryFolder(logger:Common.Logging.ILog)=
        let temporaryFolderPath = 
            result {
                let nonExistingTempFolderPath = (System.IO.Path.Combine(System.IO.Path.GetTempPath(),"SDPEVAL",(Guid.NewGuid().ToString())))
                let! existingTempFolderPath = ensureDirectoryExists true nonExistingTempFolderPath
                return existingTempFolderPath
            }

        member this.FolderPath = temporaryFolderPath
        interface IDisposable with
            member x.Dispose() = 
                logger.Debug(new Msg(fun m -> m.Invoke((sprintf "Disposing folder '%A'" temporaryFolderPath))|>ignore))
                match (result{
                    let! folderPath = temporaryFolderPath
                    let! deleted = deleteDirectory true folderPath
                    return deleted                
                }) with
                |Result.Ok v -> ()
                |Result.Error ex -> raise ex
        
    let ensureFileExists path = 
        match System.IO.File.Exists(path) with
        | true -> Result.Ok path            
        | false -> Result.Error (new System.Exception(sprintf "File does not exist: '%s'" path))
        
    let ensureFileExistsWithMessage message path = 
        match System.IO.File.Exists(path) with
        | true -> Result.Ok path            
        | false -> Result.Error (new System.IO.FileNotFoundException(message) :> Exception)

    let directoryIsEmpty directoryPath =
        match (directoryPathExists directoryPath) with
        |true ->
            let isEmpty = not (System.IO.Directory.GetDirectories(directoryPath).Length > 0 || System.IO.Directory.GetFiles(directoryPath).Length > 0)
            isEmpty
        |false -> true
    
    let ensureDirectoryExistsAndIsEmptyWithMessage  message (directoryPath) createIfNotExists =
       match (ensureDirectoryExists createIfNotExists directoryPath) with
       |Ok dp -> 
           match (directoryIsEmpty dp) with
           |true -> Result.Ok dp
           |false -> Result.Error (new Exception(sprintf "Directory '%s' is not empty. %s" (dp) message))
       |Result.Error ex -> Result.Error ex

    let tryCatchWithMessage<'T,'R> f (t:'T) message : Result<'R, Exception> =
        try
            Result.Ok (f t)
        with
            | ex -> toErrorResult message (Some ex)

    type FileExistsException(message : string) =
        inherit Exception(message)    
    
    let deleteFileUnsafe path  =
        System.IO.File.Delete (path)

    let deleteFile path = 
        let deleteFileResult = tryCatch deleteFileUnsafe path
        match deleteFileResult with
        | Result.Ok _ -> Result.Ok path
        | Result.Error ex -> Result.Error ex

    let ensureFileDoesNotExistWithMessage message overwrite filePath =
        match System.IO.File.Exists(filePath) with
        | true -> 
            match overwrite with
            | true -> deleteFile filePath        
            | false -> Result.Error (new FileExistsException(sprintf "File allready exists: '%s'. %s" (filePath) message) :> Exception)
        | false -> Result.Ok filePath