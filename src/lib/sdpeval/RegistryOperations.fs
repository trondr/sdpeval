namespace sdpeval

module internal RegistryOperations =
    open System
    open F    
    open Microsoft.Win32;
    open BaseApplicabilityRules
    open sdpeval.Logging
        
    let logger = Logging.getLoggerByName("RegistryOperations")

    let toRegistryView isRegType32 =
        match isRegType32 with
        |true -> RegistryView.Registry32
        |false -> RegistryView.Registry64

    let toRegHive regHiveName isRegType32 =
        let registryView = toRegistryView isRegType32        
        match regHiveName with
        |"HKCU"|"HKEY_CURRENT_USER" -> RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, registryView)
        |"HKLM"|"HKEY_LOCAL_MACHINE" -> RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView)
        |"HKCR"|"HKEY_CLASSES_ROOT" -> RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, registryView)
        |"HKCC"|"HKEY_CURRENT_CONFIG" -> RegistryKey.OpenBaseKey(RegistryHive.CurrentConfig, registryView)
        |"HKU"|"HKEY_USERS" -> RegistryKey.OpenBaseKey(RegistryHive.Users, registryView)
        |_ -> raise (new Exception("Unknown registry hive: " + regHiveName))

    let getRegistryValueUnsafe hiveName subKeyPath valueName isRegType32 =
        let regHive = toRegHive hiveName isRegType32
        let regKey = toOption (regHive.OpenSubKey(subKeyPath,false))
        match regKey with
        |Some k -> toOption (k.GetValue(valueName))
        |None -> None

    let toBoolean (value:string option) defaultValue =
        match value with
        |Some v -> Convert.ToBoolean(v)
        |None -> defaultValue

    let getRegistryValue hiveName subKeyPath valueName regType32 =
        try
            let isRegType32 = toBoolean regType32 false
            getRegistryValueUnsafe hiveName subKeyPath valueName isRegType32
        with
        |ex ->
            logger.Debug(sprintf "[%s\\%s]%s. Error: %s" hiveName subKeyPath valueName ex.Message)
            None
     
    let toString (data:obj option) = 
        match data with
        |Some d -> 
            if (d :? string) then
                Some (d :?> string)
            else    
                None
        |None -> None

    let isRegSz (regsz:RegSz) =
        let localData = toString (getRegistryValue regsz.Key regsz.Subkey regsz.Value regsz.RegType32)
        let comparison = BaseTypes.toStringComparison regsz.Comparison
        match localData with
        |Some ld -> BaseTypes.compareString comparison ld regsz.Data
        |None -> false
