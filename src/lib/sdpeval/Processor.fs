namespace sdpeval

module internal SystemInfo =

    open System
    open sdpeval.BaseTypes
    open sdpeval.NativeMethods
    open sdpeval.Logging

    let systemInfo = 
        getNativeSystemInfo()

    let processorArchitecture =
        systemInfo.wProcessorArchitecture

    let processorLevel =
        systemInfo.wProcessorLevel

    let processorRevision =
        systemInfo.wProcessorRevision

    let isProcessor (logger:Common.Logging.ILog) architecture level revision =
        
        let all = [|architecture;level;revision|]
        let allIsNull = all|>Array.forall(fun i-> (i = null))

        if(allIsNull) then
            raise (new Exception("Invalid Processor definition in SDP.xml. At least one of the attributes must be set: Architecture,Level,Revision"))
                
        let isArchitecture =
            if(architecture = null) then
                true
            else
                logger.Debug(new Msg(fun m -> m.Invoke( (sprintf "Current processor architecture: '%u'." processorArchitecture))|>ignore))
                (toUInt16 architecture) = processorArchitecture

        let isLevel =
            if(level = null) then
                true
            else
                logger.Debug(new Msg(fun m -> m.Invoke( (sprintf "Current processor level: '%i'." processorLevel))|>ignore))
                (toInt16 level) = processorLevel

        let isRevision =
            if(revision = null) then
                true
            else
                logger.Debug(new Msg(fun m -> m.Invoke( (sprintf "Current processor revision: '%u'." processorRevision))|>ignore))
                (toUInt16 revision) = processorRevision
        
        (isArchitecture && isLevel && isRevision)           