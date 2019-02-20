namespace sdpeval

module WindowsVersion =

    open System
    open BaseTypes
    open BaseApplicabilityRules
    open spdeval.csharp    

    type WindowsVersionEx = 
        {
            MajorVersion:UInt32;
            MinorVersion:UInt32;BuildNumber:UInt32;
            ServicePackMajor:UInt16;
            ServicePackMinor:UInt16;            
            SuiteMask:UInt16;
            ProductType:byte}

    let currentWindowsVersion =
        let osVersion = NativeMethods.GetVersion()
        {
            MajorVersion = osVersion.dwMajorVersion;
            MinorVersion = osVersion.dwMinorVersion;
            BuildNumber = osVersion.dwBuildNumber;
            ServicePackMajor=osVersion.wServicePackMajor;
            ServicePackMinor=osVersion.wServicePackMinor;
            SuiteMask=osVersion.wSuiteMask;
            ProductType=osVersion.wProductType
        }

    let isWindowsVersion (currentWindowsVersion:WindowsVersionEx) (windowsVersion:WindowsVersion) =
        
        let all = [|windowsVersion.MajorVersion;windowsVersion.MinorVersion;windowsVersion.BuildNumber;windowsVersion.ServicePackMajor;windowsVersion.ServicePackMinor;windowsVersion.SuiteMask;windowsVersion.ProductType|]
        let allIsNull = all|>Array.forall(fun i-> (i = null))

        if(allIsNull) then
            raise (new Exception("Invalid WindowsVersion definition in SDP.xml. At least one of the attributes must be set: MajorVersion,MinorVersion,BuildNumber,ServicePackMajor,ServicePackMinor,SuiteMask,ProductType"))
        
        let comparison = BaseTypes.toScalarComparison windowsVersion.Comparison
                
        let isMajorVersion =
            if(windowsVersion.MajorVersion = null) then
                true
            else
                BaseTypes.compareScalar comparison (currentWindowsVersion.MajorVersion) (toUInt32 windowsVersion.MajorVersion) 
        
        let isMinorVersion =
            if(windowsVersion.MinorVersion = null) then
                true
            else
                BaseTypes.compareScalar comparison (currentWindowsVersion.MinorVersion) (toUInt32 windowsVersion.MinorVersion)

        (isMajorVersion && isMinorVersion)