namespace sdpeval.tests

open NUnit.Framework

[<TestFixture>]
module WindowsVersionTest =     
    open sdpeval.WindowsVersion    
    open sdpeval.BaseApplicabilityRules
    open System.Numerics    
    open sdpeval.tests
    
    let internal defaultCurrentWindowsVersion = {MajorVersion=0u;MinorVersion=0u;BuildNumber=0u;ServicePackMajor=0us;ServicePackMinor=0us;SuiteMask=0us;ProductType=0us}
    let internal defaultWindowsVersion = {Comparison = "EqualTo";MajorVersion=Some "6";MinorVersion=None;BuildNumber=None;ServicePackMajor=None;ServicePackMinor=None;AllSuitesMustBePresent=Some "false";SuiteMask=None;ProductType=None}
    type internal TestData ={WEx:OsVersion;W:WindowsVersion;Expected:bool}

    let internal currentWindowsVersionsTestData =
        [
            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 5u};W={defaultWindowsVersion with Comparison="LessThan";MajorVersion=Some "6"};Expected=true}
            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 6u};W={defaultWindowsVersion with Comparison="LessThan";MajorVersion=Some "6"};Expected=false}
            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 10u};W={defaultWindowsVersion with Comparison="LessThan";MajorVersion=Some "6"};Expected=false}

            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 5u};W={defaultWindowsVersion with Comparison="LessThanOrEqualTo";MajorVersion=Some "6"};Expected=true}
            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 6u};W={defaultWindowsVersion with Comparison="LessThanOrEqualTo";MajorVersion=Some "6"};Expected=true}
            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 10u};W={defaultWindowsVersion with Comparison="LessThanOrEqualTo";MajorVersion=Some "6"};Expected=false}

            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 5u};W={defaultWindowsVersion with Comparison="EqualTo";MajorVersion=Some "6"};Expected=false}
            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 6u};W={defaultWindowsVersion with Comparison="EqualTo";MajorVersion=Some "6"};Expected=true}
            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 10u};W={defaultWindowsVersion with Comparison="EqualTo";MajorVersion=Some "6"};Expected=false}


            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 5u};W={defaultWindowsVersion with Comparison="GreaterThanOrEqualTo";MajorVersion=Some "6"};Expected=false}
            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 6u};W={defaultWindowsVersion with Comparison="GreaterThanOrEqualTo";MajorVersion=Some "6"};Expected=true}
            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 10u};W={defaultWindowsVersion with Comparison="GreaterThanOrEqualTo";MajorVersion=Some "6"};Expected=true}

            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 5u};W={defaultWindowsVersion with Comparison="GreaterThan";MajorVersion=Some "6"};Expected=false}
            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 6u};W={defaultWindowsVersion with Comparison="GreaterThan";MajorVersion=Some "6"};Expected=false}
            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 10u};W={defaultWindowsVersion with Comparison="GreaterThan";MajorVersion=Some "6"};Expected=true}
            
            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 5u;MinorVersion=1u};W={defaultWindowsVersion with Comparison="LessThan";MajorVersion=Some "6";MinorVersion=Some "1"};Expected=true}
            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 6u;MinorVersion=1u};W={defaultWindowsVersion with Comparison="LessThan";MajorVersion=Some "6";MinorVersion=Some "1"};Expected=false}
            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 10u;MinorVersion=1u};W={defaultWindowsVersion with Comparison="LessThan";MajorVersion=Some "6";MinorVersion=Some "1"};Expected=false}

            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 5u;MinorVersion=1u};W={defaultWindowsVersion with Comparison="LessThanOrEqualTo";MajorVersion=Some "6";MinorVersion=Some "1"};Expected=true}
            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 6u;MinorVersion=1u};W={defaultWindowsVersion with Comparison="LessThanOrEqualTo";MajorVersion=Some "6";MinorVersion=Some "1"};Expected=true}
            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 10u;MinorVersion=1u};W={defaultWindowsVersion with Comparison="LessThanOrEqualTo";MajorVersion=Some "6";MinorVersion=Some "1"};Expected=false}

            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 5u;MinorVersion=1u};W={defaultWindowsVersion with Comparison="EqualTo";MajorVersion=Some "6";MinorVersion=Some "1"};Expected=false}
            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 6u;MinorVersion=1u};W={defaultWindowsVersion with Comparison="EqualTo";MajorVersion=Some "6";MinorVersion=Some "1"};Expected=true}
            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 10u;MinorVersion=1u};W={defaultWindowsVersion with Comparison="EqualTo";MajorVersion=Some "6";MinorVersion=Some "1"};Expected=false}


            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 5u;MinorVersion=1u};W={defaultWindowsVersion with Comparison="GreaterThanOrEqualTo";MajorVersion=Some "6";MinorVersion=Some "1"};Expected=false}
            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 6u;MinorVersion=1u};W={defaultWindowsVersion with Comparison="GreaterThanOrEqualTo";MajorVersion=Some "6";MinorVersion=Some "1"};Expected=true}
            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 10u;MinorVersion=1u};W={defaultWindowsVersion with Comparison="GreaterThanOrEqualTo";MajorVersion=Some "6";MinorVersion=Some "1"};Expected=true}

            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 5u;MinorVersion=1u};W={defaultWindowsVersion with Comparison="GreaterThan";MajorVersion=Some "6";MinorVersion=Some "1"};Expected=false}
            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 6u;MinorVersion=1u};W={defaultWindowsVersion with Comparison="GreaterThan";MajorVersion=Some "6";MinorVersion=Some "1"};Expected=false}
            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 10u;MinorVersion=1u};W={defaultWindowsVersion with Comparison="GreaterThan";MajorVersion=Some "6";MinorVersion=Some "1"};Expected=true}


            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 6u;MinorVersion=2u;BuildNumber=9200u};W={defaultWindowsVersion with Comparison="EqualTo";MajorVersion=Some "6"};Expected=true}
        ]
    

    [<Test>]
    [<Category(TestCategory.UnitTests)>]
    [<TestCaseSource("currentWindowsVersionsTestData")>]
    let isWindowsVersionsTests(testData:obj) = 
        let testDataR = (testData:?>TestData)
        let actual = isWindowsVersion testDataR.WEx testDataR.W 
        Assert.AreEqual(testDataR.Expected,actual,(sprintf "'%A' Not %s  '%A'" testDataR.WEx testDataR.W.Comparison testDataR.W))
        ()
    
    type internal VersionToNumberTestData = {Version:Version;Expected:BigInteger}

    let internal versionToNumberTestData = 
        [|            
            yield {Version={MajorVersion=Some 1u;MinorVersion=Some 1u;BuildNumber=Some 1u;ServicePackMajor=Some 1us;ServicePackMinor=Some 1us};Expected=(new BigInteger(Array.concat [ 
                (toByteArray (Some (System.Convert.ToUInt16(0x01))));
                (toByteArray (Some (System.Convert.ToUInt16(0x01))));
                (toByteArray (Some (System.Convert.ToUInt16(0x01))));
                (toByteArray (Some (System.Convert.ToUInt16(0x01))));
                (toByteArray (Some (System.Convert.ToUInt16(0x01))));
            ]|>Array.rev))}
        |]

    [<Test>]
    [<Category(TestCategory.UnitTests)>]
    [<TestCaseSource("versionToNumberTestData")>]
    let versionToNumberTest (testData:obj) =
        let testDataR = (testData:?>VersionToNumberTestData)
        let actual = versionToNumber testDataR.Version
        Assert.IsTrue(testDataR.Expected = actual,sprintf "%A <> %A" testDataR.Expected actual)

    [<Test>]
    [<Category(TestCategory.UnitTests)>]
    [<TestCase(true,5us|||512us,5us,false)>]
    [<TestCase(true,5us|||512us,0us,false)>]
    [<TestCase(false,5us|||512us,5us,true)>]
    [<TestCase(false,5us|||512us,0us,false)>]
    let isSuiteMaskMatchTest(allSuitesMustBePresent:bool,currentSuiteMask:System.UInt16,suiteMask:System.UInt16,expected:bool)=
        let actual = isSuiteMaskMatch allSuitesMustBePresent currentSuiteMask (Some suiteMask)
        Assert.AreEqual(expected,actual)
