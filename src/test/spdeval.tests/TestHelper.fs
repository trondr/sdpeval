namespace sdpeval.tests

module TestHelper =
    
    let currentModel = 
        let currentModelResult = sdpeval.tests.SystemInfo.getModelCodeForCurrentSystem ()
        match currentModelResult with
        |Ok m -> m
        |Error ex -> sprintf "ERROR: Failed to get model due to: %s" ex.Message

    let IsTestRelevant testName validModel  =            
        match validModel with
        |validModel when validModel = currentModel -> true
        |validModel when validModel = "AllModels" -> true
        |_ -> 
            printfn "WARNING: Test '%s' not valid for model '%s'. Only valid for model '%s'." testName currentModel validModel                
            false

