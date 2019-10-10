namespace sdpeval

module internal F =
    open System

    let getFiles searchPattern folder =
        System.IO.Directory.GetFiles(folder, searchPattern)
    
    let fileExists filePath =
        System.IO.File.Exists(filePath)

    open System.Text.RegularExpressions
    //Source: http://www.fssnip.net/29/title/Regular-expression-active-pattern
    let (|Regex|_|) pattern input =
        let m = Regex.Match(input, pattern)
        if m.Success then Some(List.tail [ for g in m.Groups -> g.Value ])
        else None

    //Source: http://www.fssnip.net/7UQ/title/Null-Value-guard-active-pattern
    let (|Null|NotNull|) (x : 'T when 'T : not struct) =
        if obj.ReferenceEquals(x, null) then Null else NotNull x
    
    let tryCatch<'T, 'R> f  (t:'T) : Result<'R, Exception> =
        try
            Result.Ok (f t)
        with
            | ex -> Result.Error ex
    
    let toOption value =
        match value with
        |null -> None
        |v -> Some v

    let toOptionalBool (boolString:string option) =
        match boolString with
        |None -> None
        |Some "true" -> Some true
        |Some "false" -> Some false
        |_ -> None


    open System.Collections.Generic
    ///
    /// Source: http://www.fssnip.net/8P/title/Memoization-for-dynamic-programming
    ///
    /// The function creates a function that calls the argument 'f'
    /// only once and stores the result in a mutable dictionary (cache)
    /// Repeated calls to the resulting function return cached values.
    ///
    let memoize f =    
      // Create (mutable) cache that is used for storing results of 
      // for function arguments that were already calculated.
      let cache = new Dictionary<_, _>()
      (fun x ->
          // The returned function first performs a cache lookup
          let success, value = cache.TryGetValue(x)
          if success then value else 
            // If value was not found, calculate & cache it
            let v = f(x) 
            cache.[x] <- v
            v)
    
    let processBit =
        match IntPtr.Size with
        |8 -> "64-Bit"
        |4 -> "32-Bit"
        |_ -> "Unknown"

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

    let rec getAccumulatedExceptionMessages (ex: Exception) =
        match ex.InnerException with
        | null -> ex.Message
        | _ -> ex.Message + " " + (getAccumulatedExceptionMessages ex.InnerException)

    let getAllExceptions (results:seq<Result<_,Exception>>) =
        let f = fun (r:Result<_,Exception>) ->
            match r with
            |Error ex -> Some(getAccumulatedExceptionMessages ex)
            |Ok v -> None
        results 
        |> Seq.choose f
    
    let getAllValues (results:seq<Result<_,Exception>>) =
        let f = fun (r:Result<_,Exception>) ->
            match r with
            |Error ex -> None
            |Ok v -> Some(v)
        results 
        |> Seq.choose f

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

    let toAccumulatedResult (results:seq<Result<_,Exception>>) =
        let resultsArray = results |> Seq.toArray        
        
        let allExceptionMessages = 
                (getAllExceptions resultsArray) 
                |> Seq.toArray
        
        let accumulatedResult =             
            match allExceptionMessages.Length with
            | 0 -> 
                let allValues = getAllValues resultsArray
                Result.Ok allValues
            | _ -> 
                toErrorResult (String.Join<string>(" ", allExceptionMessages)) None
        accumulatedResult

    let toSearchOptions recurse =
        match recurse with
        |false -> System.IO.SearchOption.TopDirectoryOnly
        |true -> System.IO.SearchOption.AllDirectories

    let findFilesUnsafe recurse searchPattern folder =
        let searchOptions = toSearchOptions recurse
        System.IO.Directory.GetFiles(folder, searchPattern,searchOptions)

    let findFiles recurse searchPattern folder =
        try
            Result.Ok (findFilesUnsafe recurse searchPattern (folder))            
        with
        |ex -> Result.Error (new Exception(sprintf "Failed to find files in folder '%s' due to: %s" folder ex.Message,ex))