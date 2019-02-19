namespace sdpeval

module sdp =
    
    open Microsoft.UpdateServices.Administration
        
    /// <summary>
    /// Load SoftwareDistributionPackage. To use Microsoft.UpdateServices.Administration it is required that the registry key value [HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Update Services\Server\Setup]TargetDir is defined indicating that Update Services is installed and set to the root of the update services installation. Default value is "%ProgramFiles%\Update Services\". It is required that the process is running in 64 bit process, otherwise it will attempt to access "c:\Program Files (x86)\Update Services" instead of the correct path "C:\Program Files\Update Services". The TargetDir value might be overriden for test purposes and set to the location of a copy of the update services installation. Update Services is installed as part of the RSAT Windows Feature (requires download and install from microsoft.com)
    /// </summary>
    /// <param name="sdpXmlFile"></param>
    let loadsdp (sdpXmlFile:string) =         
        let softwareDistributionPackage = new SoftwareDistributionPackage(sdpXmlFile)
        softwareDistributionPackage        

    