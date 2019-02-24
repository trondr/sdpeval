namespace sdpeval

module WindowsVersion =

    open System
    open BaseTypes
    open BaseApplicabilityRules
    open spdeval.csharp    
    open System.Linq

    type OsVersion = 
        {
            MajorVersion:UInt32;
            MinorVersion:UInt32;
            BuildNumber:UInt32;
            ServicePackMajor:UInt16;
            ServicePackMinor:UInt16;
            SuiteMask:UInt16;
            ProductType:UInt16}

    type Version =
        {
            MajorVersion:UInt32 option;
            MinorVersion:UInt32 option;
            BuildNumber:UInt32 option;
            ServicePackMajor:UInt16 option;
            ServicePackMinor:UInt16 option;
        }

    type SuitMask = {AllSuitesMustBePresent:bool;SuiteMask:UInt16 option;ProductType:UInt16 option}

    let currentWindowsVersion =
        let osVersion = NativeMethods.GetOsVersion()
        {
            MajorVersion = osVersion.dwMajorVersion;
            MinorVersion = osVersion.dwMinorVersion;
            BuildNumber = osVersion.dwBuildNumber;
            ServicePackMajor=osVersion.wServicePackMajor;
            ServicePackMinor=osVersion.wServicePackMinor;
            SuiteMask=osVersion.wSuiteMask;
            ProductType=Convert.ToUInt16(osVersion.wProductType)
        }

    let toSuiteMask (windowsVersion:WindowsVersion)  =
        match (windowsVersion.AllSuitesMustBePresent,windowsVersion.SuiteMask,windowsVersion.ProductType) with
        |(Some m, Some s, Some p) -> {AllSuitesMustBePresent=toBoolean m;SuiteMask=Some (toUInt16 s); ProductType=Some (toUInt16 p)}
        |(Some m, Some s, None) -> {AllSuitesMustBePresent=toBoolean m;SuiteMask=Some (toUInt16 s);ProductType=None}
        |(Some m, None, Some p) -> {AllSuitesMustBePresent=toBoolean m;SuiteMask=None;ProductType=Some (toUInt16 p)}
        |(Some m, None, None) -> {AllSuitesMustBePresent=toBoolean m;SuiteMask=None;ProductType=None}
        |(None, Some s, Some p) -> {AllSuitesMustBePresent=false;SuiteMask=Some (toUInt16 s); ProductType=Some (toUInt16 p)}
        |(None, Some s, None) -> {AllSuitesMustBePresent=false;SuiteMask=Some (toUInt16 s);ProductType=None}
        |(None, None, Some p) -> {AllSuitesMustBePresent=false;SuiteMask=None;ProductType=Some (toUInt16 p)}
        |(None, None, None) -> {AllSuitesMustBePresent=false;SuiteMask=None;ProductType=None}
            
    let toVersion (windowsVersion:WindowsVersion) =
        let version =
            match (windowsVersion.MajorVersion, windowsVersion.MinorVersion, windowsVersion.BuildNumber, windowsVersion.ServicePackMajor, windowsVersion.ServicePackMinor) with
            |(Some majorVersion, Some minorVersion, Some buildNumber, Some servicePackMajor, Some servicePackMinor) -> 
                {MajorVersion=Some (toUInt32 majorVersion);MinorVersion=Some (toUInt32 minorVersion);BuildNumber=Some (toUInt32 buildNumber);ServicePackMajor=Some (toUInt16  servicePackMajor);ServicePackMinor=Some (toUInt16 servicePackMinor)}
            |(Some majorVersion, Some minorVersion, Some buildNumber, Some servicePackMajor, None) -> 
                {MajorVersion=Some (toUInt32 majorVersion);MinorVersion=Some (toUInt32 minorVersion);BuildNumber=Some (toUInt32 buildNumber);ServicePackMajor=Some (toUInt16  servicePackMajor);ServicePackMinor=None}
            |(Some majorVersion, Some minorVersion, Some buildNumber, None, None) -> 
                {MajorVersion=Some (toUInt32 majorVersion);MinorVersion=Some (toUInt32 minorVersion);BuildNumber=Some (toUInt32 buildNumber);ServicePackMajor=None;ServicePackMinor=None}
            |(Some majorVersion, Some minorVersion, None, None, None) -> 
                {MajorVersion=Some (toUInt32 majorVersion);MinorVersion=Some (toUInt32 minorVersion);BuildNumber=None;ServicePackMajor=None;ServicePackMinor=None}
            |(Some majorVersion, None, None, None, None) -> 
                {MajorVersion=Some (toUInt32 majorVersion);MinorVersion=None;BuildNumber=None;ServicePackMajor=None;ServicePackMinor=None}
            |(None, None, None, None, None) -> 
                {MajorVersion=None;MinorVersion=None;BuildNumber=None;ServicePackMajor=None;ServicePackMinor=None}
            |_ ->
                raise (new Exception(sprintf "Invalid WindowsVersion '%A'" windowsVersion))
        version
    
    open System.Numerics

    let toUInt16 (uInt32:UInt32 option) = 
        match uInt32 with
        |None -> None
        |Some n ->                               
            Some (Convert.ToUInt16(n))

    let toByteArray (value:UInt16 option) = 
        match value with
        |None -> Array.init 2 (fun _ -> byte 0x00)
        |Some v -> BitConverter.GetBytes(v)

    let versionToNumber (version:Version) =
        let byteArray = 
            Array.concat [ 
                (toByteArray (toUInt16 version.MajorVersion));
                (toByteArray (toUInt16 version.MinorVersion));
                (toByteArray (toUInt16 version.BuildNumber));
                (toByteArray (version.ServicePackMajor));
                (toByteArray (version.ServicePackMinor));
            ]
        let number = new BigInteger(byteArray|>Array.rev)
        number

    let osVersionToVersion (osVersion:OsVersion) =
        {
            MajorVersion = Some osVersion.MajorVersion;
            MinorVersion = Some osVersion.MinorVersion;
            BuildNumber = Some osVersion.BuildNumber;
            ServicePackMajor = Some osVersion.ServicePackMajor;
            ServicePackMinor = Some osVersion.ServicePackMinor;
        }

    let isSuiteMaskMatch allSuitesMustBePresent currentSuiteMask suiteMask = 
            match suiteMask with
                |None -> true
                |Some s ->                     
                    match allSuitesMustBePresent with
                    |true ->                        
                            currentSuiteMask = s
                    |false -> 
                            (currentSuiteMask &&& s) > 1us

    let isProductTypeMatch (currentProuctType:UInt16) (productType:UInt16 option) =
            match productType with
            |None -> true
            |Some p -> 
                (currentProuctType = p)
    
    let optionToNumber  option defaultNumber number =
        match option with
        |Some n -> number
        |None -> defaultNumber

    let toComparableOsVersion (osVersion:OsVersion) (windowsVersion:WindowsVersion) = 
        {
            OsVersion.MajorVersion = optionToNumber windowsVersion.MajorVersion 0u osVersion.MajorVersion;
            OsVersion.MinorVersion = optionToNumber windowsVersion.MinorVersion 0u osVersion.MinorVersion;
            OsVersion.BuildNumber = optionToNumber windowsVersion.BuildNumber 0u osVersion.BuildNumber;
            OsVersion.ServicePackMajor = optionToNumber windowsVersion.ServicePackMajor 0us osVersion.ServicePackMajor;
            OsVersion.ServicePackMinor = optionToNumber windowsVersion.ServicePackMinor 0us osVersion.ServicePackMinor;
            OsVersion.SuiteMask = optionToNumber windowsVersion.SuiteMask 0us osVersion.SuiteMask;
            OsVersion.ProductType = optionToNumber windowsVersion.ProductType 0us osVersion.ProductType;
        }
        

    let isWindowsVersion (currentWindowsVersion:OsVersion) (windowsVersion:WindowsVersion) =
        
        let all = [|windowsVersion.MajorVersion;windowsVersion.MinorVersion;windowsVersion.BuildNumber;windowsVersion.ServicePackMajor;windowsVersion.ServicePackMinor;windowsVersion.SuiteMask;windowsVersion.ProductType|]
        let allIsNone = all|>Array.forall(fun i-> (i = None))

        if(allIsNone) then
            raise (new Exception("Invalid WindowsVersion definition in SDP.xml. At least one of the attributes must be set: MajorVersion,MinorVersion,BuildNumber,ServicePackMajor,ServicePackMinor,SuiteMask,ProductType"))
        
        let comparison = BaseTypes.toScalarComparison windowsVersion.Comparison
               
        let comparableCurrentWindowsVersion = toComparableOsVersion currentWindowsVersion windowsVersion

        let currentVersionNumber = versionToNumber (osVersionToVersion comparableCurrentWindowsVersion)
        let windowsVersionNumber = versionToNumber (toVersion windowsVersion)

        let isVersionMatch = BaseTypes.compareScalar comparison currentVersionNumber windowsVersionNumber

        let suiteMask = toSuiteMask windowsVersion
        let isSuiteMaskMatch = 
            isSuiteMaskMatch suiteMask.AllSuitesMustBePresent comparableCurrentWindowsVersion.SuiteMask suiteMask.SuiteMask

        let isProductTypeMatch =
            isProductTypeMatch comparableCurrentWindowsVersion.ProductType suiteMask.ProductType        
        (isVersionMatch && isSuiteMaskMatch && isProductTypeMatch)