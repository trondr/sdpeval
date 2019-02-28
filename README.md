# sdpeval
Evaluate WSUS Software Distribution Package applicability rules as defined in WSUS schema reference https://docs.microsoft.com/en-us/previous-versions/windows/desktop/bb972752(v=vs.85)

## Example sdp xml

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
