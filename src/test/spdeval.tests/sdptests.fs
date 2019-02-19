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
    [<TestCase("<lar:And><lar:True /><lar:True /></lar:And>",true)>]
    [<TestCase("<lar:And><lar:False /><lar:True /></lar:And>",false)>]
    [<TestCase("<lar:And><lar:True /><lar:False /></lar:And>",false)>]
    [<TestCase("<lar:And><lar:False /><lar:False /></lar:And>",false)>]

    [<TestCase("<lar:Or><lar:True /><lar:True /></lar:Or>",true)>]
    [<TestCase("<lar:Or><lar:False /><lar:True /></lar:Or>",true)>]
    [<TestCase("<lar:Or><lar:True /><lar:False /></lar:Or>",true)>]
    [<TestCase("<lar:Or><lar:False /><lar:False /></lar:Or>",false)>]

    [<TestCase("<lar:Not><lar:False /></lar:Not>",true)>]
    [<TestCase("<lar:Not><lar:True /></lar:Not>",false)>]
    let sdpXmlToApplicabilityRulesTests (xmlString,expectedEvaluation:bool) =
        let actual = sdpXmlToApplicabilityRules xmlString
        let actualEvaluation = evaluateApplicabilityRule actual
        Assert.AreEqual(expectedEvaluation,actualEvaluation)