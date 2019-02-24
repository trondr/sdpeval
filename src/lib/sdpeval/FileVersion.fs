namespace sdpeval

module FileVersion =
    open System
    open BaseApplicabilityRules
    open sdpeval.BaseTypes
    open spdeval.csharp
    open System.Diagnostics

    let getFileVersion filePath =
        FileVersionInfo.GetVersionInfo(filePath).FileVersion

    let getPath (fileVersion:FileVersion) =
        match fileVersion.Csidl with
            |Some csidl-> 
                let parentFolder = FileSystemNativeMethods.GetFolderPath(toInt32 csidl)
                System.IO.Path.Combine(parentFolder,fileVersion.Path)                
            |None ->
                fileVersion.Path                

    let getCurrentFileVersion (fileVersion:FileVersion) =        
        let path = getPath fileVersion
        let version = getFileVersion path
        {fileVersion with Version=version}

    type FourPartVersion =
        {
            MajorVersion:UInt16;
            MinorVersion:UInt16;
            BuildNumber:UInt16;
            Revision:UInt16;
        }

    let toFourPartVersion (version:string) =
        let va = version.Split([|'.'|])
        match va.Length with
        |4 -> {MajorVersion=(toUInt16 va.[0]);MinorVersion=(toUInt16 va.[1]);BuildNumber=(toUInt16 va.[2]);Revision=(toUInt16 va.[3])}
        |3 -> {MajorVersion=(toUInt16 va.[0]);MinorVersion=(toUInt16 va.[1]);BuildNumber=(toUInt16 va.[2]);Revision=0us}
        |2 -> {MajorVersion=(toUInt16 va.[0]);MinorVersion=(toUInt16 va.[1]);BuildNumber=0us;Revision=0us}
        |1 -> {MajorVersion=(toUInt16 va.[0]);MinorVersion=0us;BuildNumber=0us;Revision=0us}
        |_ -> raise (new Exception(sprintf "Invalid version string: '%s'" version))
    
    let toByteArray (value:UInt16) = 
        BitConverter.GetBytes(value)

    open System.Numerics

    let versionToNumber (version:FourPartVersion) =
        let byteArray = 
            Array.concat [ 
                (toByteArray version.MajorVersion);
                (toByteArray version.MinorVersion);
                (toByteArray version.BuildNumber);
                (toByteArray version.Revision);                
            ]
        let number = new BigInteger(byteArray|>Array.rev)
        number

    let isFileVersionBase (currentFileVersion:FileVersion) (fileVersion:FileVersion) =
        let currentVersionNumber = versionToNumber (toFourPartVersion currentFileVersion.Version)
        let versionNumber = versionToNumber (toFourPartVersion fileVersion.Version)
        let comparison = BaseTypes.toScalarComparison fileVersion.Comparison
        BaseTypes.compareScalar comparison currentVersionNumber versionNumber

    let isFileVersion fileVersion = 
        let path = getPath fileVersion
        match (sdpeval.F.fileExists path) with
        |true -> 
            let currentFileVersion = getCurrentFileVersion fileVersion
            isFileVersionBase currentFileVersion fileVersion
        |false -> false

