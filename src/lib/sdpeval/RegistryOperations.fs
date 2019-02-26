namespace sdpeval

module RegistryOperations =
    open System
    open F    
    open Microsoft.Win32;
    open BaseApplicabilityRules
        
    let logger = Logging.getLoggerByName("RegistryOperations")
    
    let toRegHive regHiveName =
        match regHiveName with
        |"HKCU"|"HKEY_CURRENT_USER" -> Registry.CurrentUser
        |"HKLM"|"HKEY_LOCAL_MACHINE" -> Registry.LocalMachine
        |"HKCR"|"HKEY_CLASSES_ROOT" -> Registry.ClassesRoot        
        |"HKCC"|"HKEY_CURRENT_CONFIG" -> Registry.CurrentConfig
        |"HKU"|"HKEY_USERS" -> Registry.Users
        |_ -> raise (new Exception("Unknown registry hive: " + regHiveName))

    let getRegistryValueUnsafe hiveName subKeyPath valueName =
        let regHive = toRegHive hiveName
        let regKey = toOption (regHive.OpenSubKey(subKeyPath,false))
        match regKey with
        |Some k -> toOption (k.GetValue(valueName))
        |None -> None

    let getRegistryValue hiveName subKeyPath valueName =
        try
            getRegistryValueUnsafe hiveName subKeyPath valueName
        with
        |ex ->
            if(logger.IsDebugEnabled) then logger.Debug(sprintf "[%s\\%s]%s. Error: %s" hiveName subKeyPath valueName ex.Message)
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
        let localData = toString (getRegistryValue regsz.Key regsz.Subkey regsz.Value)
        let comparison = BaseTypes.toStringComparison regsz.Comparison
        match localData with
        |Some ld -> BaseTypes.compareString comparison ld regsz.Data
        |None -> false
