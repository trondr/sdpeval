namespace sdpeval

open System.Text

module internal Msi =
    open BaseApplicabilityRules
    open System.Runtime.InteropServices
    open System
    open System.Numerics
    open F

    [<DllImport("msi.dll", SetLastError = true)>]
    extern int MsiQueryProductState(string product)

    type InstallState =         
        |INSTALLSTATE_NOTUSED      = -7  // component disabled
        |INSTALLSTATE_BADCONFIG    = -6  // configuration data corrupt
        |INSTALLSTATE_INCOMPLETE   = -5  // installation suspended or in progress
        |INSTALLSTATE_SOURCEABSENT = -4  // run from source, source is unavailable
        |INSTALLSTATE_MOREDATA     = -3  // return buffer overflow
        |INSTALLSTATE_INVALIDARG   = -2  // invalid function argument
        |INSTALLSTATE_UNKNOWN      = -1  // unrecognized product or feature
        |INSTALLSTATE_BROKEN       =  0  // broken
        |INSTALLSTATE_ADVERTISED   =  1  // advertised feature
        |INSTALLSTATE_REMOVED      =  1  // component being removed (action state, not settable)
        |INSTALLSTATE_ABSENT       =  2  // uninstalled (or action state absent but clients remain)
        |INSTALLSTATE_LOCAL        =  3  // installed on local drive
        |INSTALLSTATE_SOURCE       =  4  // run from source, CD or net
        |INSTALLSTATE_DEFAULT      =  5  // use default, local or source

    type  ErrorCodes =
        |ERROR_SUCCESS = 0
        |ERROR_BAD_CONFIGURATION = 0x0000064A
        |ERROR_INVALID_PARAMETER = 0x00000057
        |ERROR_MORE_DATA = 0x000000EA
        |ERROR_UNKNOWN_PRODUCT = 0x00000645
        |ERROR_UNKNOWN_PROPERTY = 0x00000648

    [<DllImport("msi.dll", SetLastError = true, CharSet=CharSet.Unicode)>]
    extern int MsiGetProductInfoW(string product, string property, [<Out>]StringBuilder valueBuf, int& len);

    type MsiProductCode = private MsiProductCode of string
    let msiProductCode productCode =
        match productCode with
        |Regex "^({[a-zA-Z0-9]{8}-[a-zA-Z0-9]{4}-[a-zA-Z0-9]{4}-[a-zA-Z0-9]{4}-[a-zA-Z0-9]{12}})$" [validProductCode] -> (MsiProductCode validProductCode)
        |_ -> raise (new Exception(sprintf "Invalid msi product code: %s" productCode))
    let msiProductCodeValue (MsiProductCode productCode) = productCode

    type MsiProductLanguage = private MsiProductLanguage of int
    let msiProductLanguage language =
        MsiProductLanguage language
    let msiProductLanguageFromOptionalString (language:string option) =
        match language with
        |None -> None
        | Some l -> 
            match l with
            |Regex @"^(\d+)$" [validProductLanguage] -> Some (MsiProductLanguage (Convert.ToInt32(validProductLanguage)))
            |_ -> None
    let msiProductLanguageValue (MsiProductLanguage language) = language
    
    type ProductVersion = {Major:UInt16;Minor:UInt16;Build:UInt16;Revision:UInt16}

    type MsiProductInstalledRule = {ProductCode:MsiProductCode;VersionMin:ProductVersion option;ExcludeVersionMin:bool option;VersionMax:ProductVersion option;ExcludeVersionMax:bool option;Languange:MsiProductLanguage option}

    let toProductVersion (versionString:string option) =
        match versionString with
        |None -> None
        |Some vs ->        
            match vs with
                |Regex @"^(\d+)\.(\d+)\.(\d+)\.(\d+)$" [major;minor;build;revision] -> Some {Major=BaseTypes.toUInt16 major;Minor=BaseTypes.toUInt16 minor;Build=BaseTypes.toUInt16 build;Revision=BaseTypes.toUInt16 revision}   
                |Regex @"^(\d+)\.(\d+)\.(\d+)$" [major;minor;build] -> Some {Major=BaseTypes.toUInt16 major;Minor=BaseTypes.toUInt16 minor;Build=BaseTypes.toUInt16 build;Revision=0us}   
                |Regex @"^(\d+)\.(\d+)$" [major;minor] -> Some {Major=BaseTypes.toUInt16 major;Minor=BaseTypes.toUInt16 minor;Build=0us;Revision=0us}
                |Regex @"^(\d+)$" [major] -> Some {Major=BaseTypes.toUInt16 major;Minor=0us;Build=0us;Revision=0us}
                |_ -> None

    let getMsiProductInstallState productCode =
        let installState = enum<InstallState>(MsiQueryProductState(msiProductCodeValue productCode))
        installState
    
    let getMsiProductVersion productCode =
        let sb = (new StringBuilder(256))        
        let mutable sbSize = 256
        let result = enum<ErrorCodes>(MsiGetProductInfoW(msiProductCodeValue productCode,"VersionString",sb,&sbSize))        
        toProductVersion (Some (sb.ToString()))
    
    let toProductLanguage languageString =
        match languageString with
        |Regex @"^(\d+)$" [language] -> Some(msiProductLanguage (Convert.ToInt32(language)))
        |_ -> None

    let getMsiProductLanguage productCode =
        let sb = (new StringBuilder(256))        
        let mutable sbSize = 256
        let result = enum<ErrorCodes>(MsiGetProductInfoW(msiProductCodeValue productCode,"Language",sb,&sbSize))        
        toProductLanguage (sb.ToString())

    let toByteArray (value:UInt16) =         
        BitConverter.GetBytes(value)

    let versionToNumber (version:ProductVersion) =
        let byteArray = 
            Array.concat [ 
                (toByteArray version.Major);
                (toByteArray version.Minor);
                (toByteArray version.Build);
                (toByteArray version.Revision);                
            ]
        let number = new BigInteger(byteArray|>Array.rev)
        number

    let optionToBool (optionBool: bool option) =
        match optionBool with
        |None -> false
        |Some true -> true
        |Some false -> false

    let toMsiProductInstalledRule (mp:MsiProductInstalled) = 
        {
            ProductCode = msiProductCode mp.ProductCode;
            VersionMin = toProductVersion mp.VersionMin;
            ExcludeVersionMin = toOptionalBool mp.ExcludeVersionMin;
            VersionMax = toProductVersion mp.VersionMax
            ExcludeVersionMax=toOptionalBool mp.ExcludeVersionMax;
            Languange= msiProductLanguageFromOptionalString mp.Languange        
        }
    
    type MsiProductInfo = {ProductCode: MsiProductCode;IsInstalled:bool;ProductVersion:ProductVersion option;ProductLanguage:MsiProductLanguage option}

    let getMsiProductInfo productCode = 
        let installState = getMsiProductInstallState productCode
        let isInstalled = 
            match installState with
            |InstallState.INSTALLSTATE_DEFAULT
            |InstallState.INSTALLSTATE_LOCAL ->
                true
            |_ -> 
                false;
        let productVersion =
            match isInstalled with
            |true ->
                getMsiProductVersion productCode
            |false -> None                
        let productLanguage =
            match isInstalled with
            |true ->
               getMsiProductLanguage productCode
            |false -> None      
        {
            ProductCode = productCode
            IsInstalled = isInstalled
            ProductVersion =  productVersion
            ProductLanguage = productLanguage
        }
    
    let isVersionMatch installedVersion compareOperator version =
        let installedVersionNumber = versionToNumber installedVersion
        match version with
        |None -> true
        |Some v -> 
            let versionNumber = versionToNumber v
            (compareOperator installedVersionNumber versionNumber)
    
    let isVersionRuleDefined version = 
        match version with
        |Some _ -> true
        |None   -> false

    let getMinOperator exclude =
        match exclude with
        |None -> (>=)
        |Some b ->
            match b with 
            |true -> (>)
            |false -> (>=)

    let getMaxOperator exclude =
        match exclude with
        |None -> (<=)
        |Some b ->
            match b with 
            |true -> (<)
            |false -> (<=)

    let isMsiProductInstalledBase (getMsiProductInfoFunc: MsiProductCode -> MsiProductInfo) (mp:MsiProductInstalled) =
        let mpr = toMsiProductInstalledRule mp
        let msiProductInfo = getMsiProductInfoFunc mpr.ProductCode
        match msiProductInfo.IsInstalled with
        | true ->            
            let isVersionMatch =
                match msiProductInfo.ProductVersion with
                |Some pv ->                    
                    let minOperator = getMinOperator mpr.ExcludeVersionMin
                    let isVersionMin = isVersionMatch pv minOperator mpr.VersionMin
                    let maxOperator = getMaxOperator mpr.ExcludeVersionMax
                    let isVersionMax = isVersionMatch pv maxOperator mpr.VersionMax
                    (isVersionMin && isVersionMax)
                |None -> 
                    let versionRuleIsDefined = (isVersionRuleDefined mpr.VersionMin)||(isVersionRuleDefined mpr.VersionMax)
                    match versionRuleIsDefined with
                    |true -> false
                    |false -> true
            let isLanguageMatch =
                match msiProductInfo.ProductLanguage with
                |Some pl ->
                    match mpr.Languange with
                    |Some mpl -> 
                        pl = mpl
                    |None -> true
                |None -> 
                    match mpr.Languange with
                    |Some _ -> false
                    |None -> true
            (isVersionMatch && isLanguageMatch)
        |_ -> false

    let isMsiProductInstalled (mp:MsiProductInstalled) =
        isMsiProductInstalledBase getMsiProductInfo mp
