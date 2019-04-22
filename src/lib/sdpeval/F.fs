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
