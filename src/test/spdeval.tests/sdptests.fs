namespace sdpeval.tests

open NUnit.Framework


[<TestFixture>]
module sdptest =
    open NUnit.Framework
    open sdpeval.Sdp
    open sdpeval.F
    open sdpeval.tests

    //1. Download 'ftp://ftp.hp.com/pub/softlib/software/sms_catalog/HpCatalogForSms.latest.cab' and extract to C:\\Temp\\DriverToolCache\\HpCatalogForSms.latest
    //2. Make sure 'C:\Program Files (x86)\Update Services\Schema' exists

    [<Test>]
    [<Category(TestCategory.ManualTests)>]
    [<Timeout(120000)>]
    [<TestCase("C:\\Temp\\DriverToolCache\\HpCatalogForSms.latest\\\V2")>]
    let loadsdpTest (folderWithSdpFiles) =
        let sdpfiles = getFiles "*.sdp" folderWithSdpFiles 
        let logger = Common.Logging.Simple.ConsoleOutLogger("sdpeval.tests",Common.Logging.LogLevel.All,true,true,true,"yyyy-MM-dd-HH-mm-ss-ms")

        sdpfiles
        |> Array.map(fun f ->                 
                let actual = LoadSdp f
                Assert.IsNotNull(actual.Description)                
                let isInstallableRules = sdpeval.Sdp.sdpXmlToApplicabilityRules logger actual.IsInstallable
                let isRootInstallable = (sdpeval.Sdp.evaluateApplicabilityRule logger isInstallableRules)
                //if(isInstallable) then                    
                //    printfn "%s IsInstallable:%b Rule:%A" actual.Title isInstallable actual.IsInstallable
                
                actual.InstallableItems
                |> Seq.map(fun ii ->                                                 
                        let IsInstallableRules = sdpeval.Sdp.sdpXmlToApplicabilityRules logger ii.IsInstallableApplicabilityRule                        
                        let isInstallable = (sdpeval.Sdp.evaluateApplicabilityRule logger IsInstallableRules)
                        if(isInstallable) then                            
                            //printf "%s IsInstallable: %b Rule:%A" actual.Title isInstallable ii.IsInstallableApplicabilityRule
                            printf "%s (%A) IsInstallable: %b (%b)" actual.Title actual.CreationDate  isInstallable (isRootInstallable)
                            let isInstalledRules = sdpeval.Sdp.sdpXmlToApplicabilityRules logger ii.IsInstalledApplicabilityRule
                            let isInstalled = (sdpeval.Sdp.evaluateApplicabilityRule logger isInstalledRules)
                            //if(isInstalled) then                            
                            //printfn " IsInstalled:%b Rule:%A" isInstalled ii.IsInstalledApplicabilityRule
                            printfn " IsInstalled:%b" isInstalled

                    ) 
                |>Seq.toArray
                |>ignore
                
            ) |> ignore
        
        ()
    
    [<Test>]
    [<Category(TestCategory.ManualTests)>]
    [<Timeout(120000)>]
    [<TestCase("C:\\Temp\\DriverToolCache\\HpCatalogForSms.latest\\\V2\\00004850-0000-0000-5350-000000071121.sdp")>]
    let loadsdpFileTest (sdpFile) =
        let sdpfiles = [|sdpFile|] 
        let logger = Common.Logging.Simple.ConsoleOutLogger("sdpeval.tests",Common.Logging.LogLevel.All,true,true,true,"yyyy-MM-dd-HH-mm-ss-ms")

        sdpfiles
        |> Array.map(fun f ->                 
                let actual = LoadSdp f
                Assert.IsNotNull(actual.Description)                
                let isInstallableRules = sdpeval.Sdp.sdpXmlToApplicabilityRules logger actual.IsInstallable
                let isRootInstallable = (sdpeval.Sdp.evaluateApplicabilityRule logger isInstallableRules)
                //if(isInstallable) then                    
                //    printfn "%s IsInstallable:%b Rule:%A" actual.Title isInstallable actual.IsInstallable
                
                actual.InstallableItems
                |> Seq.map(fun ii ->                                                 
                        let IsInstallableRules = sdpeval.Sdp.sdpXmlToApplicabilityRules logger ii.IsInstallableApplicabilityRule                        
                        let isInstallable = (sdpeval.Sdp.evaluateApplicabilityRule logger IsInstallableRules)
                        //if(isInstallable) then                            
                        //printf "%s IsInstallable: %b Rule:%A" actual.Title isInstallable ii.IsInstallableApplicabilityRule
                        printf "%s (%A) IsInstallable: %b (%b)" actual.Title actual.CreationDate  isInstallable (isRootInstallable)
                        let isInstalledRules = sdpeval.Sdp.sdpXmlToApplicabilityRules logger ii.IsInstalledApplicabilityRule
                        let isInstalled = (sdpeval.Sdp.evaluateApplicabilityRule logger isInstalledRules)
                        //if(isInstalled) then                            
                        //printfn " IsInstalled:%b Rule:%A" isInstalled ii.IsInstalledApplicabilityRule
                        printfn " IsInstalled:%b" isInstalled

                    ) 
                |>Seq.toArray
                |>ignore
                
            ) |> ignore
        
        ()


    [<Test>]
    [<Category(TestCategory.UnitTests)>]
    [<TestCase("01.And:True+True->True","AllModels","<lar:And><lar:True /><lar:True /></lar:And>",true,"",false)>]
    [<TestCase("02.And:False+True->False","AllModels","<lar:And><lar:False /><lar:True /></lar:And>",false,"",false)>]
    [<TestCase("03.And:True+False->False","AllModels","<lar:And><lar:True /><lar:False /></lar:And>",false,"",false)>]
    [<TestCase("04.And:False+False->False","AllModels","<lar:And><lar:False /><lar:False /></lar:And>",false,"",false)>]

    [<TestCase("05.Or:True+True->True","AllModels","<lar:Or><lar:True /><lar:True /></lar:Or>",true,"",false)>]
    [<TestCase("06.Or:False+True->True","AllModels","<lar:Or><lar:False /><lar:True /></lar:Or>",true,"",false)>]
    [<TestCase("07.Or:True+False->True","AllModels","<lar:Or><lar:True /><lar:False /></lar:Or>",true,"",false)>]
    [<TestCase("08.Or:False+False->False","AllModels","<lar:Or><lar:False /><lar:False /></lar:Or>",false,"",false)>]

    [<TestCase("09.Not:False->True","AllModels","<lar:Not><lar:False /></lar:Not>",true,"",false)>]
    [<TestCase("10.Not:True->False","AllModels","<lar:Not><lar:True /></lar:Not>",false,"",false)>]

    [<TestCase("11.WmiQuery:Check for disk C:->True","AllModels","<lar:And><bar:WmiQuery Namespace=\"Root\cimv2\" WqlQuery=\"select * from Win32_LogicalDisk where (DeviceId='C:') \" /></lar:And>",true,"Hmm could not find c: drive. Is this system setup like most systems?",false)>]
    [<TestCase("12.WmiQuery:Check for disk Z:->False","AllModels","<lar:And><bar:WmiQuery Namespace=\"Root\cimv2\" WqlQuery=\"select * from Win32_LogicalDisk where (DeviceId='Z:') \" /></lar:And>",false,"Make sure z: is not mapped to a drive when running this test.",false)>]

    [<TestCase("13.ComplexLogic:->True","AllModels","<lar:And><lar:And><lar:True /><lar:True /></lar:And><lar:And><lar:True /><lar:True /></lar:And></lar:And>",true,"",false)>]
    [<TestCase("14.ComplexLogic:->False","AllModels","<lar:And><lar:And><lar:False /><lar:True /></lar:And><lar:And><lar:True /><lar:True /></lar:And></lar:And>",false,"",false)>]
    [<TestCase("15.ComplexLogic:->False","AllModels","<lar:And><lar:And><lar:True /><lar:True /></lar:And><lar:And><lar:True /><lar:False /></lar:And></lar:And>",false,"",false)>]
    
    [<TestCase("16.ComplexLogic:->False","AllModels","<lar:Not><lar:Not><lar:False /></lar:Not></lar:Not>",false,"",false)>]
    [<TestCase("17.ComplexLogic:->true","AllModels","<lar:Not><lar:Not><lar:True /></lar:Not></lar:Not>",true,"",false)>]

    [<TestCase("18.Processor:Incorrect specified processor->Throw Exception","AllModels","<lar:And><bar:Processor /></lar:And>",true,"",true)>]
    [<TestCase("19.Processor:Architecture 9 -> True","AllModels","<lar:And><bar:Processor Architecture=\"9\"/></lar:And>",true,"This unit test might fail depending on the current processor architecture.",false)>]

    [<TestCase("20.Processor:Level 6->True","AllModels","<lar:And><bar:Processor Level=\"6\"/></lar:And>",true,"This unit test might fail depending on the current processor level.",false)>]
    [<TestCase("21.Processor:Revision 19971","SomeDell???","<lar:And><bar:Processor Revision=\"19971\"/></lar:And>",true,"This unit test might fail depending on the current processor revision.",false)>]
    [<TestCase("22.Processor:Revision 24067","20EQ0022MN???","<lar:And><bar:Processor Revision=\"24067\"/></lar:And>",true,"This unit test might fail depending on the current processor revision.",false)>]
    [<TestCase("23.Processor:Architecture 9, Level 6, Revision 19971","SomeHP???","<lar:And><bar:Processor Architecture=\"9\" Level=\"6\" Revision=\"19971\"/></lar:And>",true,"This unit test might fail depending on the current processor revision.",false)>]
    [<TestCase("24.Processor:Architecture 9","20EQ0022MN","<lar:And><bar:Processor Architecture=\"9\" Level=\"6\" Revision=\"24067\"/></lar:And>",true,"This unit test might fail depending on the current processor revision. Run successfully on Lenovo P50 (20EQ0022MN).",false)>]
    [<TestCase("25.Processor:Architecture 9","854A","<lar:And><bar:Processor Architecture=\"9\" Level=\"6\" Revision=\"36364\"/></lar:And>",true,"This unit test might fail depending on the current processor revision. Run successfully on HP 830 G6 (854A).",false)>]
    [<TestCase("26.WmiQuery: Complex Logic","SomeDell","<lar:And><bar:WmiQuery Namespace=\"Root\\Dell\\sysinv\" WqlQuery=\"SELECT * FROM Dell_SoftwareIdentity WHERE (Description &gt;= 'Dell:APAC_102511__' AND Description &lt; 'Dell:APAC_102511__~')\" /><lar:Not><bar:WmiQuery Namespace=\"Root\Dell\sysinv\" WqlQuery=\"SELECT * FROM Dell_SoftwareIdentity WHERE (Description &gt;= 'Dell:APAC_102511__' AND Description &lt; 'Dell:APAC_102511__~') AND VersionString &lt; '0001.0002.1004.0000'\" /></lar:Not></lar:And>",false,"This unit test might fail depending on the current PC model not beeing a Dell and that Dell sysinventory agent not beeing installed.",false)>]
    [<TestCase("27.WmiQuery: Complex Logic->True","AllModels","<lar:And>
    <bar:WmiQuery Namespace=\"Root\cimv2\" WqlQuery=\"select * from Win32_ComputerSystem where (Manufacturer='Hewlett-Packard' and not (Model like '%Proliant%')) or (Manufacturer='HP') or (Manufacturer='System manufacturer') \" />
    <lar:Or>
      <lar:And>
        <bar:Processor Architecture=\"9\" />
        <bar:WindowsVersion Comparison=\"EqualTo\" MajorVersion=\"10\" MinorVersion=\"0\" />
        <bar:RegSz Key=\"HKEY_LOCAL_MACHINE\" Subkey=\"Software\Microsoft\Windows NT\CurrentVersion\" Value=\"ReleaseId\" Comparison=\"EqualTo\" Data=\"2009\" />
      </lar:And>
      <lar:And>
        <bar:Processor Architecture=\"9\" />
        <bar:WindowsVersion Comparison=\"EqualTo\" MajorVersion=\"10\" MinorVersion=\"0\" />
        <bar:RegSz Key=\"HKEY_LOCAL_MACHINE\" Subkey=\"Software\Microsoft\Windows NT\CurrentVersion\" Value=\"ReleaseId\" Comparison=\"EqualTo\" Data=\"2003\" />
      </lar:And>
      <lar:And>
        <bar:Processor Architecture=\"9\" />
        <bar:WindowsVersion Comparison=\"EqualTo\" MajorVersion=\"10\" MinorVersion=\"0\" />
        <bar:RegSz Key=\"HKEY_LOCAL_MACHINE\" Subkey=\"Software\Microsoft\Windows NT\CurrentVersion\" Value=\"ReleaseId\" Comparison=\"EqualTo\" Data=\"1909\" />
      </lar:And>
      <lar:And>
        <bar:Processor Architecture=\"9\" />
        <bar:WindowsVersion Comparison=\"EqualTo\" MajorVersion=\"10\" MinorVersion=\"0\" />
        <bar:RegSz Key=\"HKEY_LOCAL_MACHINE\" Subkey=\"Software\Microsoft\Windows NT\CurrentVersion\" Value=\"ReleaseId\" Comparison=\"EqualTo\" Data=\"1903\" />
      </lar:And>
      <lar:And>
        <bar:Processor Architecture=\"9\" />
        <bar:WindowsVersion Comparison=\"EqualTo\" MajorVersion=\"10\" MinorVersion=\"0\" />
        <bar:RegSz Key=\"HKEY_LOCAL_MACHINE\" Subkey=\"Software\Microsoft\Windows NT\CurrentVersion\" Value=\"ReleaseId\" Comparison=\"EqualTo\" Data=\"1809\" />
      </lar:And>
      <lar:And>
        <bar:Processor Architecture=\"9\" />
        <bar:WindowsVersion Comparison=\"EqualTo\" MajorVersion=\"10\" MinorVersion=\"0\" />
        <bar:RegSz Key=\"HKEY_LOCAL_MACHINE\" Subkey=\"Software\Microsoft\Windows NT\CurrentVersion\" Value=\"ReleaseId\" Comparison=\"EqualTo\" Data=\"1803\" />
      </lar:And>
      <lar:And>
        <bar:Processor Architecture=\"9\" />
        <bar:WindowsVersion Comparison=\"EqualTo\" MajorVersion=\"10\" MinorVersion=\"0\" />
        <bar:RegSz Key=\"HKEY_LOCAL_MACHINE\" Subkey=\"Software\\Microsoft\\Windows NT\\CurrentVersion\" Value=\"ReleaseId\" Comparison=\"EqualTo\" Data=\"1703\" />
      </lar:And>
      <lar:And>
        <bar:Processor Architecture=\"9\" />
        <bar:WindowsVersion Comparison=\"EqualTo\" MajorVersion=\"10\" MinorVersion=\"0\" />
        <bar:RegSz Key=\"HKEY_LOCAL_MACHINE\" Subkey=\"Software\\Microsoft\\Windows NT\\CurrentVersion\" Value=\"ReleaseId\" Comparison=\"EqualTo\" Data=\"1709\" />
      </lar:And>
      <lar:And>
        <bar:Processor Architecture=\"9\" />
        <bar:WindowsVersion Comparison=\"EqualTo\" MajorVersion=\"10\" MinorVersion=\"0\" />
        <bar:RegSz Key=\"HKEY_LOCAL_MACHINE\" Subkey=\"Software\\Microsoft\\Windows NT\\Current\\Version\" Value=\"ReleaseId\" Comparison=\"EqualTo\" Data=\"1607\" />
      </lar:And>
    </lar:Or>
</lar:And>",true,"This unit test might fail depending on the current manufacturer, processor architecturer, windows version or release id read from registry.",false)>]

    [<TestCase("28.WmiQuery: Installable update sp96016 (00004850-0000-0000-5350-000000096016.sdp)","854A","<lar:And>
    <bar:WmiQuery Namespace=\"Root\\cimv2\" WqlQuery=\"select * from Win32_ComputerSystem where (Manufacturer='Hewlett-Packard' and not (Model like '%Proliant%')) or (Manufacturer='HP') \" />
    <lar:Or>
      <lar:And>
        <bar:Processor Architecture=\"9\" />
        <bar:WindowsVersion Comparison=\"EqualTo\" MajorVersion=\"10\" MinorVersion=\"0\" />
        <bar:RegSz Key=\"HKEY_LOCAL_MACHINE\" Subkey=\"Software\\Microsoft\\Windows NT\\CurrentVersion\" Value=\"ReleaseId\" Comparison=\"EqualTo\" Data=\"1903\" />
      </lar:And>
      <lar:And>
        <bar:Processor Architecture=\"9\" />
        <bar:WindowsVersion Comparison=\"EqualTo\" MajorVersion=\"10\" MinorVersion=\"0\" />
        <bar:RegSz Key=\"HKEY_LOCAL_MACHINE\" Subkey=\"Software\\Microsoft\\Windows NT\\CurrentVersion\" Value=\"ReleaseId\" Comparison=\"EqualTo\" Data=\"1803\" />
      </lar:And>
      <lar:And>
        <bar:Processor Architecture=\"9\" />
        <bar:WindowsVersion Comparison=\"EqualTo\" MajorVersion=\"10\" MinorVersion=\"0\" />
        <bar:RegSz Key=\"HKEY_LOCAL_MACHINE\" Subkey=\"Software\\Microsoft\\Windows NT\\CurrentVersion\" Value=\"ReleaseId\" Comparison=\"EqualTo\" Data=\"1809\" />
      </lar:And>
    </lar:Or>
</lar:And>",true,"This unit test might fail depending on the current processor revision. Run successfully on HP 830 G6 (854A).",false)>]

    [<TestCase("29.WmiQuery: IsInstalled update sp96016 (00004850-0000-0000-5350-000000096016.sdp)","854A","<lar:And>
    <bar:WmiQuery Namespace=\"Root\cimv2\" WqlQuery=\"select * from Win32_BaseBoard where Product LIKE '%8524%' or Product LIKE '%853d%' or Product LIKE '%8549%' or Product LIKE '%854a%' or Product LIKE '%856d%' or Product LIKE '%856e%' or Product LIKE '%860c%' or Product LIKE '%860f%' or Product LIKE '%8655%'\" />
    <bar:WmiQuery Namespace=\"Root\cimv2\" WqlQuery=\"select * from Win32_PnpEntity where DeviceID LIKE '%ACPI\\\\HPQ8002%'\" />
    <lar:Or>
      <lar:And>
        <bar:Processor Architecture=\"9\" />
        <bar:WindowsVersion Comparison=\"EqualTo\" MajorVersion=\"10\" MinorVersion=\"0\" />
        <bar:RegSz Key=\"HKEY_LOCAL_MACHINE\" Subkey=\"Software\Microsoft\Windows NT\CurrentVersion\" Value=\"ReleaseId\" Comparison=\"EqualTo\" Data=\"1803\" />
        <bar:FileVersion Csidl=\"37\" Path=\"drivers\HpqKbFiltr.sys\" Comparison=\"GreaterThanOrEqualTo\" Version=\"11.0.7.1\" />
      </lar:And>
      <lar:And>
        <bar:Processor Architecture=\"9\" />
        <bar:WindowsVersion Comparison=\"EqualTo\" MajorVersion=\"10\" MinorVersion=\"0\" />
        <bar:RegSz Key=\"HKEY_LOCAL_MACHINE\" Subkey=\"Software\Microsoft\Windows NT\CurrentVersion\" Value=\"ReleaseId\" Comparison=\"EqualTo\" Data=\"1809\" />
        <bar:FileVersion Csidl=\"37\" Path=\"drivers\HpqKbFiltr.sys\" Comparison=\"GreaterThanOrEqualTo\" Version=\"11.0.7.1\" />
      </lar:And>
      <lar:And>
        <bar:Processor Architecture=\"9\" />
        <bar:WindowsVersion Comparison=\"EqualTo\" MajorVersion=\"10\" MinorVersion=\"0\" />
        <bar:RegSz Key=\"HKEY_LOCAL_MACHINE\" Subkey=\"Software\Microsoft\Windows NT\CurrentVersion\" Value=\"ReleaseId\" Comparison=\"EqualTo\" Data=\"1903\" />
        <bar:FileVersion Csidl=\"37\" Path=\"drivers\HpqKbFiltr.sys\" Comparison=\"GreaterThanOrEqualTo\" Version=\"11.0.7.1\" />
      </lar:And>
    </lar:Or>
    </lar:And>",true,"This unit test might fail depending on the current processor revision. Run successfully on HP 830 G6 (854A).",false)>]

    [<TestCase("30.WmiQuery: IsInstalled update sp96016 (00004850-0000-0000-5350-000000096016.sdp)","854A","<lar:And>
    <bar:WindowsVersion Comparison=\"GreaterThanOrEqualTo\" MajorVersion=\"4\" />
    <bar:WmiQuery Namespace=\"Root\\cimv2\" WqlQuery=\"select * from Win32_BaseBoard where Product LIKE '%8524%' or Product LIKE '%853d%' or Product LIKE '%8549%' or Product LIKE '%854a%' or Product LIKE '%856d%' or Product LIKE '%856e%' or Product LIKE '%860c%' or Product LIKE '%860f%' or Product LIKE '%8655%'\" />
    <bar:WmiQuery Namespace=\"Root\\cimv2\" WqlQuery=\"select * from Win32_PnpEntity where DeviceID LIKE '%ACPI\\\\HPQ8002%'\" />
</lar:And>",true,"This unit test might fail depending on the current processor revision. Run successfully on HP 830 G6 (854A).",false)>]


    let sdpXmlToApplicabilityRulesTests (testName:string,validModel:string,xmlString:string,expectedEvaluation:bool,errorMessage:string,expectException:bool) =         
    
        let testIsRelevant = TestHelper.IsTestRelevant testName validModel        
        let logger = Common.Logging.Simple.ConsoleOutLogger("sdpeval.tests",Common.Logging.LogLevel.All,true,true,true,"yyyy-MM-dd-HH-mm-ss-ms")
        match testIsRelevant with
        |false ->
            printfn "WARNING: Skipped test '%s'" testName
            Assert.Inconclusive()
        |true -> 
            try
                let actual = sdpXmlToApplicabilityRules logger xmlString
                let actualEvaluation = evaluateApplicabilityRule logger actual
                Assert.AreEqual(expectedEvaluation,actualEvaluation,errorMessage)
            with
            |ex -> Assert.IsTrue(expectException,sprintf "%s. %s" ex.Message errorMessage)
    