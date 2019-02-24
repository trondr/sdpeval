namespace sdpeval.Tests

open NUnit.Framework

[<TestFixture>]
module WindowsVersionTest =     
    open sdpeval.WindowsVersion    
    open sdpeval.BaseApplicabilityRules
    open System.Numerics    
    
    let defaultCurrentWindowsVersion = {MajorVersion=0u;MinorVersion=0u;BuildNumber=0u;ServicePackMajor=0us;ServicePackMinor=0us;SuiteMask=0us;ProductType=0us}
    let defaultWindowsVersion = {Comparison = "EqualTo";MajorVersion=Some "6";MinorVersion=None;BuildNumber=None;ServicePackMajor=None;ServicePackMinor=None;AllSuitesMustBePresent=Some "false";SuiteMask=None;ProductType=None}
    type TestData ={WEx:OsVersion;W:WindowsVersion;Expected:bool}

    let currentWindowsVersionsTestData =
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
    [<TestCaseSource("currentWindowsVersionsTestData")>]
    let isWindowsVersionsTests(testData:TestData) = 
        let actual = isWindowsVersion testData.WEx testData.W 
        Assert.AreEqual(testData.Expected,actual,(sprintf "'%A' Not %s  '%A'" testData.WEx testData.W.Comparison testData.W))
        ()
    
    type VersionToNumberTestData = {Version:Version;Expected:BigInteger}

    let versionToNumberTestData = 
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
    [<TestCaseSource("versionToNumberTestData")>]
    let versionToNumberTest (testData:VersionToNumberTestData) =
        let actual = versionToNumber testData.Version
        Assert.IsTrue(testData.Expected = actual,sprintf "%A <> %A" testData.Expected actual)

    [<Test>]
    [<TestCase(true,5us|||512us,5us,false)>]
    [<TestCase(true,5us|||512us,0us,false)>]
    [<TestCase(false,5us|||512us,5us,true)>]
    [<TestCase(false,5us|||512us,0us,false)>]
    let isSuiteMaskMatchTest(allSuitesMustBePresent:bool,currentSuiteMask:System.UInt16,suiteMask:System.UInt16,expected:bool)=
        let actual = isSuiteMaskMatch allSuitesMustBePresent currentSuiteMask (Some suiteMask)
        Assert.AreEqual(expected,actual)
