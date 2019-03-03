namespace sdpeval.tests

open NUnit.Framework
open sdpeval.BaseApplicabilityRules


[<TestFixture>]
module RegSzTests =

    type internal TestData ={RegSz:RegSz;Expected:bool}

    let internal testData = 
        [
            yield {RegSz={Comparison="EqualTo";Key="HKEY_LOCAL_MACHINE";Subkey="SOFTWARE\Classes\AppID\{00944ad3-b2ad-4bcf-9202-59bf4662d521}";RegType32=Some "True";Value="RunAs";Data="Interactive User"};Expected=true}
            yield {RegSz={Comparison="EqualTo";Key="HKEY_LOCAL_MACHINE";Subkey="SOFTWARE\Classes\AppID\{00944ad3-b2ad-4bcf-9202-59bf4662d521}";RegType32=Some "False";Value="RunAs";Data="Interactive User"};Expected=true}
            yield {RegSz={Comparison="EqualTo";Key="HKEY_LOCAL_MACHINE";Subkey="SOFTWARE\Classes\AppID\{00944ad3-b2ad-4bcf-9202-59bf4662d521}";RegType32=Some "True";Value="";Data="Local Service Credential UI Broker"};Expected=true}
            yield {RegSz={Comparison="EqualTo";Key="HKEY_LOCAL_MACHINE";Subkey="SOFTWARE\Classes\AppID\{00944ad3-b2ad-4bcf-9202-59bf4662d521}";RegType32=Some "False";Value="";Data="Local Service Credential UI Broker"};Expected=true}
            yield {RegSz={Comparison="EqualTo";Key="HKEY_LOCAL_MACHINE";Subkey="SOFTWARE\Microsoft\Windows\CurrentVersion";RegType32=Some "False";Value="ProgramFilesDir";Data="C:\Program Files"};Expected=true}
            yield {RegSz={Comparison="EqualTo";Key="HKEY_LOCAL_MACHINE";Subkey="SOFTWARE\Microsoft\Windows\CurrentVersion";RegType32=Some "True";Value="ProgramFilesDir";Data="C:\Program Files (x86)"};Expected=true}
            yield {RegSz={Comparison="EqualTo";Key="HKEY_CURRENT_USER";Subkey="Software\Microsoft\Windows\CurrentVersion\ThemeManager";RegType32=Some "False";Value="ColorName";Data="NormalColor"};Expected=true}
            yield {RegSz={Comparison="EqualTo";Key="HKEY_CURRENT_USER";Subkey="Software\Microsoft\Windows\CurrentVersion\ThemeManager";RegType32=Some "True";Value="ColorName";Data="NormalColor"};Expected=true}
        ]

    [<Test>]
    [<TestCaseSource("testData")>]
    let isRegSzTests (testData:obj) =
        let testDataR = (testData:?>TestData)        
        let actual = sdpeval.RegistryOperations.isRegSz testDataR.RegSz
        Assert.AreEqual(testDataR.Expected, actual, (sprintf "'%A'" testDataR.RegSz))
        ()
