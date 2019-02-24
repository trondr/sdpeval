namespace spdeval.tests

open NUnit.Framework


[<TestFixture>]
module sdptest =
    open NUnit.Framework
    open sdpeval.sdp
    open sdpeval.F

    [<Test>]
    [<TestCase("C:\\Temp\\DriverToolCache\\HpCatalogForSms.latest\\\V2")>]
    let loadsdpTest (folderWithSdpFiles) =
        let sdpfiles = getFiles "*.sdp" folderWithSdpFiles 
        
        sdpfiles
        |> Array.map(fun f ->                 
                let actual = loadsdp f
                Assert.IsNotNull(actual.Description)
                printfn "IsInstallable: %A" actual.IsInstallable
                let isInstallableRules = sdpeval.sdp.sdpXmlToApplicabilityRules actual.IsInstallable
                printfn "IsInstallable: %A" (sdpeval.sdp.evaluateApplicabilityRule isInstallableRules)
                
                actual.InstallableItems
                |> Seq.map(fun ii ->                         
                        printfn "IsInstalledApplicabilityRule:%A  %A" ii.Id ii.IsInstalledApplicabilityRule
                        let isInstalledRules = sdpeval.sdp.sdpXmlToApplicabilityRules ii.IsInstalledApplicabilityRule
                        printfn "IsInstalled: %A" (sdpeval.sdp.evaluateApplicabilityRule isInstalledRules)

                        printfn "IsInstallableApplicabilityRule:%A  %A" ii.Id ii.IsInstalledApplicabilityRule
                        let IsInstallableRules = sdpeval.sdp.sdpXmlToApplicabilityRules ii.IsInstallableApplicabilityRule
                        printfn "IsInstallable: %A" (sdpeval.sdp.evaluateApplicabilityRule IsInstallableRules)
                    ) 
                |>Seq.toArray
                |>ignore
                
            ) |> ignore
        
        ()
    
    [<Test>]
    [<TestCase("<lar:And><lar:True /><lar:True /></lar:And>",true,"",false)>]
    [<TestCase("<lar:And><lar:False /><lar:True /></lar:And>",false,"",false)>]
    [<TestCase("<lar:And><lar:True /><lar:False /></lar:And>",false,"",false)>]
    [<TestCase("<lar:And><lar:False /><lar:False /></lar:And>",false,"",false)>]

    [<TestCase("<lar:Or><lar:True /><lar:True /></lar:Or>",true,"",false)>]
    [<TestCase("<lar:Or><lar:False /><lar:True /></lar:Or>",true,"",false)>]
    [<TestCase("<lar:Or><lar:True /><lar:False /></lar:Or>",true,"",false)>]
    [<TestCase("<lar:Or><lar:False /><lar:False /></lar:Or>",false,"",false)>]

    [<TestCase("<lar:Not><lar:False /></lar:Not>",true,"",false)>]
    [<TestCase("<lar:Not><lar:True /></lar:Not>",false,"",false)>]

    [<TestCase("<lar:And><bar:WmiQuery Namespace=\"Root\cimv2\" WqlQuery=\"select * from Win32_LogicalDisk where (DeviceId='C:') \" /></lar:And>",true,"",false)>]
    [<TestCase("<lar:And><bar:WmiQuery Namespace=\"Root\cimv2\" WqlQuery=\"select * from Win32_LogicalDisk where (DeviceId='Z:') \" /></lar:And>",false,"",false)>]

    [<TestCase("<lar:And><lar:And><lar:True /><lar:True /></lar:And><lar:And><lar:True /><lar:True /></lar:And></lar:And>",true,"",false)>]
    [<TestCase("<lar:And><lar:And><lar:False /><lar:True /></lar:And><lar:And><lar:True /><lar:True /></lar:And></lar:And>",false,"",false)>]
    [<TestCase("<lar:And><lar:And><lar:True /><lar:True /></lar:And><lar:And><lar:True /><lar:False /></lar:And></lar:And>",false,"",false)>]
    
    [<TestCase("<lar:Not><lar:Not><lar:False /></lar:Not></lar:Not>",false,"",false)>]
    [<TestCase("<lar:Not><lar:Not><lar:True /></lar:Not></lar:Not>",true,"",false)>]

    [<TestCase("<lar:And><bar:Processor /></lar:And>",true,"",true)>]
    [<TestCase("<lar:And><bar:Processor Architecture=\"9\"/></lar:And>",true,"This unit test might fail depending on the current processor architecture.",false)>]
    [<TestCase("<lar:And><bar:Processor Level=\"6\"/></lar:And>",true,"This unit test might fail depending on the current processor level.",false)>]
    [<TestCase("<lar:And><bar:Processor Revision=\"-29174\"/></lar:And>",true,"This unit test might fail depending on the current processor revision.",false)>]
    [<TestCase("<lar:And><bar:Processor Architecture=\"9\" Level=\"6\" Revision=\"-29174\"/></lar:And>",true,"This unit test might fail depending on the current processor revision.",false)>]
    
    let sdpXmlToApplicabilityRulesTests (xmlString,expectedEvaluation:bool,errorMessage:string,expectException:bool) =
        try
            let actual = sdpXmlToApplicabilityRules xmlString
            let actualEvaluation = evaluateApplicabilityRule actual
            Assert.AreEqual(expectedEvaluation,actualEvaluation,errorMessage)
        with
        |ex -> Assert.IsTrue(expectException,ex.Message)