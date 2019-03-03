# sdpeval
Evaluate WSUS Software Distribution Package applicability rules as defined in WSUS XML Schema Reference (https://docs.microsoft.com/en-us/previous-versions/windows/desktop/bb972752(v=vs.85)).

## Requirements

sdpeval dependes on Microsoft Update Services. More specifically Xml Schemas located in "%ProgramFiles%\Update Services\Schema".

Update Services is installed as part of the RSAT Windows Feature included in Windows 10 1809 (requires download and install from microsoft.com for ealier operating systems).

Microsoft.UpdateServices.Administration.dll use the registry value [HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Update Services\Server\Setup]TargetDir to find the Update Services install location. The default location is "%ProgramFiles%\Update Services\". Note that it is required that the process is running in 64 bit process, otherwise it will attempt to access and fail on "c:\Program Files (x86)\Update Services" folder. The TargetDir registry value might be overwritten for test purposes during development and set to the location of a copy of the update services installation, for example ".\tools\Update Services" where the developer has a copy of the xml schemas folder.

## Example

### C#
```csharp
var sdp = sdpeval.Sdp.LoadSdp(@"C:\Temp\DriverToolCache\HpCatalogForSms.latest\V2\00004850-0000-0000-5350-000000094801.sdp");
var isInstallable = sdpeval.Sdp.EvaluateApplicabilityXml(sdp.IsInstallable);
Console.WriteLine($"isInstallable={isInstallable}");
```
### F#
```fsharp
let sdp = sdpeval.Sdp.LoadSdp(@"C:\Temp\DriverToolCache\HpCatalogForSms.latest\V2\00004850-0000-0000-5350-000000094801.sdp")
let isInstallable = sdpeval.Sdp.EvaluateApplicabilityXml(sdp.IsInstallable)
printfn "isInstallable=%b" isInstallable
```

## Example sdp xml

```xml
<sdp:SoftwareDistributionPackage xmlns:bar="http://schemas.microsoft.com/wsus/2005/04/CorporatePublishing/BaseApplicabilityRules.xsd" xmlns:bt="http://schemas.microsoft.com/wsus/2005/04/CorporatePublishing/BaseTypes.xsd" xmlns:cmd="http://schemas.microsoft.com/wsus/2005/04/CorporatePublishing/Installers/CommandLineInstallation.xsd" xmlns:lar="http://schemas.microsoft.com/wsus/2005/04/CorporatePublishing/LogicalApplicabilityRules.xsd" xmlns:msi="http://schemas.microsoft.com/wsus/2005/04/CorporatePublishing/Installers/MsiInstallation.xsd" xmlns:msiar="http://schemas.microsoft.com/wsus/2005/04/CorporatePublishing/MsiApplicabilityRules.xsd" xmlns:msp="http://schemas.microsoft.com/wsus/2005/04/CorporatePublishing/Installers/MspInstallation.xsd" xmlns:uei="http://schemas.microsoft.com/wsus/2005/04/CorporatePublishing/Installers/UpdateExeInstallation.xsd" xmlns:usp="http://schemas.microsoft.com/wsus/2005/04/CorporatePublishing/UpdateServicesPackage.xsd" xmlns:drv="http://schemas.microsoft.com/wsus/2005/04/CorporatePublishing/Installers/WindowsDriver.xsd" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" SchemaVersion="1.2" xmlns:sdp="http://schemas.microsoft.com/wsus/2005/04/CorporatePublishing/SoftwareDistributionPackage.xsd">
    <sdp:Properties PackageID="00004850-0000-0000-5350-000000094728" CreationDate="2019-02-12T21:42:03" VendorName="HP Business Clients">
        <sdp:MoreInfoUrl>http://ftp.hp.com/pub/softpaq/sp94501-95000/sp94728.html</sdp:MoreInfoUrl>
        <sdp:SupportUrl>http://www.hp.com/support</sdp:SupportUrl>
        <sdp:ProductName>Driver</sdp:ProductName>
    </sdp:Properties>
    <sdp:LocalizedProperties>
        <sdp:Language>en</sdp:Language>
        <sdp:Title>Synaptics HD audio driver for 2018 DT [8.65.296.0.A3]</sdp:Title>
        <sdp:Description>[8.65.296.0.A3] DCH Audio driver for 2018 DT</sdp:Description>
    </sdp:LocalizedProperties>
    <sdp:UpdateSpecificData MsrcSeverity="Low" UpdateClassification="Updates">
        <sdp:KBArticleID>sp94728</sdp:KBArticleID>
    </sdp:UpdateSpecificData>
    <sdp:IsInstallable>
        <lar:And>
            <bar:WmiQuery Namespace="Root\cimv2" WqlQuery="select * from Win32_ComputerSystem where (Manufacturer='Hewlett-Packard' and not (Model like '%Proliant%')) or (Manufacturer='HP') " />
            <lar:Or>
                <lar:And>
                    <bar:Processor Architecture="9" />
                    <bar:WindowsVersion Comparison="EqualTo" MajorVersion="10" MinorVersion="0" />
                    <bar:RegSz Key="HKEY_LOCAL_MACHINE" Subkey="Software\Microsoft\Windows NT\CurrentVersion" Value="ReleaseId" Comparison="EqualTo" Data="1809" />
                </lar:And>
                <lar:And>
                    <bar:Processor Architecture="9" />
                    <bar:WindowsVersion Comparison="EqualTo" MajorVersion="10" MinorVersion="0" />
                    <bar:RegSz Key="HKEY_LOCAL_MACHINE" Subkey="Software\Microsoft\Windows NT\CurrentVersion" Value="ReleaseId" Comparison="EqualTo" Data="1803" />
                </lar:And>
            </lar:Or>
        </lar:And>
    </sdp:IsInstallable>
    <sdp:SupersededPackages>
        <sdp:PackageID>00004850-0000-0000-5350-000000088821</sdp:PackageID>
        <sdp:PackageID>00004850-0000-0000-5350-000000087800</sdp:PackageID>
    </sdp:SupersededPackages>
    <sdp:InstallableItem ID="00004850-0000-0001-5350-000000094728">
        <sdp:ApplicabilityRules>
            <sdp:IsInstalled>
                <lar:And>
                    <bar:WmiQuery Namespace="Root\cimv2" WqlQuery="select * from Win32_BaseBoard where Product LIKE '%840a%'" />
                    <bar:WmiQuery Namespace="Root\cimv2" WqlQuery="select * from Win32_PnpEntity where DeviceID LIKE '%HDAUDIO\\FUNC_01&amp;VEN_14F1&amp;DEV_5098&amp;SUBSYS_103C840A%' or DeviceID LIKE '%HDAUDIO\\FUNC_01&amp;VEN_14F1&amp;DEV_5098&amp;SUBSYS_103C8476%'" />
                    <lar:And>
                        <bar:Processor Architecture="9" />
                        <bar:WindowsVersion Comparison="EqualTo" MajorVersion="10" MinorVersion="0" />
                        <bar:FileVersion Csidl="37" Path="drivers\CHDRT64.sys" Comparison="GreaterThanOrEqualTo" Version="8.65.296.0" />
                    </lar:And>
                </lar:And>
            </sdp:IsInstalled>
            <sdp:IsInstallable>
                <lar:And>
                    <bar:WindowsVersion Comparison="GreaterThanOrEqualTo" MajorVersion="4" />
                    <bar:WmiQuery Namespace="Root\cimv2" WqlQuery="select * from Win32_BaseBoard where Product LIKE '%840a%'" />
                    <bar:WmiQuery Namespace="Root\cimv2" WqlQuery="select * from Win32_PnpEntity where DeviceID LIKE '%HDAUDIO\\FUNC_01&amp;VEN_14F1&amp;DEV_5098&amp;SUBSYS_103C840A%' or DeviceID LIKE '%HDAUDIO\\FUNC_01&amp;VEN_14F1&amp;DEV_5098&amp;SUBSYS_103C8476%'" />
                </lar:And>
            </sdp:IsInstallable>
        </sdp:ApplicabilityRules>
        <sdp:InstallProperties CanRequestUserInput="false" Impact="Normal" RequiresNetworkConnectivity="false" RebootBehavior="AlwaysRequiresReboot" />
        <cmd:CommandLineInstallerData Program="sp94728.exe" Arguments="-s -e cmd.exe -a /c &quot;setup64.exe&quot; -s" DefaultResult="Failed" RebootByDefault="false">
            <cmd:ReturnCode Code="-522190592" Result="Succeeded" Reboot="false" />
            <cmd:ReturnCode Code="1623" Result="Failed" Reboot="false" />
            <cmd:ReturnCode Code="1073807364" Result="Succeeded" Reboot="false" />
            <cmd:ReturnCode Code="3" Result="Succeeded" Reboot="false" />
            <cmd:ReturnCode Code="10" Result="Succeeded" Reboot="false" />
            <cmd:ReturnCode Code="1" Result="Succeeded" Reboot="true" />
            <cmd:ReturnCode Code="1613" Result="Failed" Reboot="false" />
            <cmd:ReturnCode Code="1624" Result="Failed" Reboot="false" />
            <cmd:ReturnCode Code="1073807464" Result="Succeeded" Reboot="false" />
            <cmd:ReturnCode Code="2" Result="Succeeded" Reboot="false" />
            <cmd:ReturnCode Code="-5001" Result="Succeeded" Reboot="false" />
            <cmd:ReturnCode Code="1639" Result="Failed" Reboot="false" />
            <cmd:ReturnCode Code="259" Result="Succeeded" Reboot="false" />
            <cmd:ReturnCode Code="-2" Result="Succeeded" Reboot="false" />
            <cmd:ReturnCode Code="99" Result="Succeeded" Reboot="false" />
            <cmd:ReturnCode Code="-5006" Result="Succeeded" Reboot="false" />
            <cmd:ReturnCode Code="-522189566" Result="Succeeded" Reboot="false" />
            <cmd:ReturnCode Code="1602" Result="Succeeded" Reboot="false" />
            <cmd:ReturnCode Code="1158" Result="Succeeded" Reboot="false" />
            <cmd:ReturnCode Code="1603" Result="Succeeded" Reboot="false" />
            <cmd:ReturnCode Code="255" Result="Succeeded" Reboot="false" />
            <cmd:ReturnCode Code="7" Result="Succeeded" Reboot="false" />
            <cmd:ReturnCode Code="3010" Result="Succeeded" Reboot="true" />
            <cmd:ReturnCode Code="1620" Result="Failed" Reboot="false" />
            <cmd:ReturnCode Code="1601" Result="Succeeded" Reboot="false" />
            <cmd:ReturnCode Code="87" Result="Failed" Reboot="false" />
            <cmd:ReturnCode Code="1619" Result="Succeeded" Reboot="false" />
            <cmd:ReturnCode Code="1153" Result="Succeeded" Reboot="false" />
            <cmd:ReturnCode Code="1641" Result="Succeeded" Reboot="false" />
            <cmd:ReturnCode Code="1631" Result="Failed" Reboot="false" />
            <cmd:ReturnCode Code="1632" Result="Failed" Reboot="false" />
            <cmd:ReturnCode Code="-3" Result="Succeeded" Reboot="false" />
            <cmd:ReturnCode Code="0" Result="Succeeded" Reboot="true" />
            <cmd:ReturnCode Code="1604" Result="Failed" Reboot="false" />
            <cmd:ReturnCode Code="1622" Result="Succeeded" Reboot="false" />
            <cmd:ReturnCode Code="-5012" Result="Succeeded" Reboot="false" />
            <cmd:ReturnCode Code="1638" Result="Succeeded" Reboot="false" />
            <cmd:ReturnCode Code="1612" Result="Failed" Reboot="false" />
            <cmd:ReturnCode Code="13" Result="Succeeded" Reboot="false" />
            <cmd:ReturnCode Code="1621" Result="Failed" Reboot="false" />
            <cmd:ReturnCode Code="-522190759" Result="Succeeded" Reboot="false" />
            <cmd:ReturnCode Code="5" Result="Succeeded" Reboot="false" />
            <cmd:ReturnCode Code="14" Result="Succeeded" Reboot="true" />
            <cmd:ReturnCode Code="1633" Result="Succeeded" Reboot="false" />
            <cmd:ReturnCode Code="1618" Result="Succeeded" Reboot="false" />
            <cmd:ReturnCode Code="1636" Result="Succeeded" Reboot="false" />
        </cmd:CommandLineInstallerData>
        <sdp:OriginFile Digest="dzismAZBcZ33vdj/7kekAp6yO9o=" FileName="sp94728.exe" Size="57251704" Modified="2019-01-23T09:37:31" OriginUri="http://ftp.hp.com/pub/softpaq/sp94501-95000/sp94728.exe" />
    </sdp:InstallableItem>
</sdp:SoftwareDistributionPackage>
```

## Example sdp applicability rule xml

``` xml
  <lar:And>
	<bar:WmiQuery Namespace="Root\cimv2" WqlQuery="select * from Win32_ComputerSystem where (Manufacturer='Hewlett-Packard' and not (Model like '%Proliant%')) or (Manufacturer='HP') " />
	<lar:Or>
	  <lar:And>
		<bar:Processor Architecture="9" />
		<bar:WindowsVersion Comparison="EqualTo" MajorVersion="10" MinorVersion="0" />
		<bar:RegSz Key="HKEY_LOCAL_MACHINE" Subkey="Software\Microsoft\Windows NT\CurrentVersion" Value="ReleaseId" Comparison="EqualTo" Data="1803" />
	  </lar:And>
	  <lar:And>
		<bar:Processor Architecture="9" />
		<bar:WindowsVersion Comparison="EqualTo" MajorVersion="10" MinorVersion="0" />
		<bar:RegSz Key="HKEY_LOCAL_MACHINE" Subkey="Software\Microsoft\Windows NT\CurrentVersion" Value="ReleaseId" Comparison="EqualTo" Data="1703" />
	  </lar:And>
	  <lar:And>
		<bar:Processor Architecture="9" />
		<bar:WindowsVersion Comparison="EqualTo" MajorVersion="10" MinorVersion="0" />
		<bar:RegSz Key="HKEY_LOCAL_MACHINE" Subkey="Software\Microsoft\Windows NT\CurrentVersion" Value="ReleaseId" Comparison="EqualTo" Data="1709" />
	  </lar:And>
	  <lar:And>
		<bar:Processor Architecture="9" />
		<bar:WindowsVersion Comparison="EqualTo" MajorVersion="10" MinorVersion="0" />
		<bar:RegSz Key="HKEY_LOCAL_MACHINE" Subkey="Software\Microsoft\Windows NT\CurrentVersion" Value="ReleaseId" Comparison="EqualTo" Data="1607" />
	  </lar:And>
	</lar:Or>
  </lar:And>
```

## Status 

5 of 5 logical applicability rules are supported

| Logical Applicability Rule | Status      |
|----------------------------|-------------|
| True                       | &#x2705;    |
| False                      | &#x2705;    |
| And                        | &#x2705;    |
| Or                         | &#x2705;    |
| Not                        | &#x2705;    |

5 of 29 base applicability rules are supported, currently sufficient to process sdp xml found in ftp://ftp.hp.com/pub/softlib/software/sms_catalog/HpCatalogForSms.latest.cab

| Base Applicability Rule | Status      |
|-------------------------|-------------|
| WindowsVersion          | &#x2705;    |
| WindowsLanguage         | &#x274C;    |
| MuiInstalled            | &#x274C;    |
| MuiLanguageInstalled    | &#x274C;    |
| SystemMetric            | &#x274C;    |
| Processor               | &#x2705;    |
| NumberOfProcessors      | &#x274C;    |
| ClusteredOS             | &#x274C;    |
| ClusterResourceOwner    | &#x274C;    |
| FileExists              | &#x274C;    |
| FileExistsPrependRegSz  | &#x274C;    |
| FileVersion             | &#x2705;    |
| FileVersionPrependRegSz | &#x274C;    |
| FileCreated             | &#x274C;    |
| FileCreatedPrependRegSz | &#x274C;    |
| FileModified            | &#x274C;    |
| FileModifiedPrependRegSz| &#x274C;    |
| FileSize                | &#x274C;    |
| FileSizePrependRegSz    | &#x274C;    |
| RegKeyExists            | &#x274C;    |
| RegValueExists          | &#x274C;    |
| RegDword                | &#x274C;    |
| RegExpandSz             | &#x274C;    |
| RegSz                   | &#x2705;    |
| RegSzToVersion          | &#x274C;    |
| RegKeyLoop              | &#x274C;    |
| WmiQuery                | &#x2705;    |
| InstalledOnce           | &#x274C;    |
| GenericQuery            | &#x274C;    |

0 of 9 Msi applicability rules are supported

| Msi Applicability Rule       | Status      |
|------------------------------|-------------|
| MsiProductInstalled          | &#x274C;    |
| MsiFeatureInstalledForProduct| &#x274C;    |
| MsiPatchInstalledForProduct  | &#x274C;    |
| MsiPatchInstalled            | &#x274C;    |
| MsiPatchSuperseded           | &#x274C;    |
| MsiPatchInstallable          | &#x274C;    |
| MsiApplicationInstalled      | &#x274C;    |
| MsiApplicationSuperseded     | &#x274C;    |
| MsiApplicationInstallable    | &#x274C;    |


## Build

* Install chocolatey 
	```batch
	@"%SystemRoot%\System32\WindowsPowerShell\v1.0\powershell.exe" -NoProfile -InputFormat None -ExecutionPolicy Bypass -Command "iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))" && SET "PATH=%PATH%;%ALLUSERSPROFILE%\chocolatey\bin"
	```
* Install fake
	
	```batch
	choco install fake
	
* Upgrade fake
	
	```batch
	choco upgrade fake
	
	```
	
* Build
	
	```batch
	fake run build.fsx
	```
