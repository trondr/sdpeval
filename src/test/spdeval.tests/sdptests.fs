namespace spdeval.tests


module sdptest =
    open NUnit.Framework
    open sdpeval.sdp

    [<Test>]
    [<TestCase("C:\\Temp\\DriverToolCache\\HpCatalogForSms.latest\\\V2\\00004850-0000-0000-5350-000000065821.sdp")>]
    let loadsdpTest (fileName) =
        let actual = loadsdp fileName
        Assert.IsNotNull(actual.Description)
        printf "%s" actual.IsInstallable
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