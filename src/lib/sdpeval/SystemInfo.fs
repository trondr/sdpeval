namespace sdpeval

module SystemInfo =

    open spdeval.csharp

    let systemInfo = 
        NativeMethods.GetNativeSystemInfo()

    let processorArchitecture =
        systemInfo.wProcessorArchitecture

    let processorLevel =
        systemInfo.wProcessorLevel

    let processorRevision =
        systemInfo.wProcessorRevision
           