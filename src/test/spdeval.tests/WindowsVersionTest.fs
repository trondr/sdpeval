namespace sdpeval.Tests

open NUnit.Framework

[<TestFixture>]
module WindowsVersionTest =     
    open sdpeval.WindowsVersion
    open spdeval.csharp
    open sdpeval.BaseApplicabilityRules
    
    let defaultCurrentWindowsVersion = {MajorVersion=0u;MinorVersion=0u;BuildNumber=0u;ServicePackMajor=0us;ServicePackMinor=0us;SuiteMask=0us;ProductType=0uy}
    let defaultWindowsVersion = {Comparison = "EqualTo";MajorVersion="6";MinorVersion=null;BuildNumber=null;ServicePackMajor=null;ServicePackMinor=null;AllSuitesMustBePresent="false";SuiteMask=null;ProductType=null}
    type TestData ={WEx:WindowsVersionEx;W:WindowsVersion;Expected:bool}

    let currentWindowsVersionsTestData =
        [
            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 5u};W={defaultWindowsVersion with Comparison="LessThan";MajorVersion="6"};Expected=true}
            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 6u};W={defaultWindowsVersion with Comparison="LessThan";MajorVersion="6"};Expected=false}
            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 10u};W={defaultWindowsVersion with Comparison="LessThan";MajorVersion="6"};Expected=false}

            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 5u};W={defaultWindowsVersion with Comparison="LessThanOrEqualTo";MajorVersion="6"};Expected=true}
            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 6u};W={defaultWindowsVersion with Comparison="LessThanOrEqualTo";MajorVersion="6"};Expected=true}
            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 10u};W={defaultWindowsVersion with Comparison="LessThanOrEqualTo";MajorVersion="6"};Expected=false}

            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 5u};W={defaultWindowsVersion with Comparison="EqualTo";MajorVersion="6"};Expected=false}
            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 6u};W={defaultWindowsVersion with Comparison="EqualTo";MajorVersion="6"};Expected=true}
            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 10u};W={defaultWindowsVersion with Comparison="EqualTo";MajorVersion="6"};Expected=false}


            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 5u};W={defaultWindowsVersion with Comparison="GreaterThanOrEqualTo";MajorVersion="6"};Expected=false}
            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 6u};W={defaultWindowsVersion with Comparison="GreaterThanOrEqualTo";MajorVersion="6"};Expected=true}
            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 10u};W={defaultWindowsVersion with Comparison="GreaterThanOrEqualTo";MajorVersion="6"};Expected=true}

            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 5u};W={defaultWindowsVersion with Comparison="GreaterThan";MajorVersion="6"};Expected=false}
            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 6u};W={defaultWindowsVersion with Comparison="GreaterThan";MajorVersion="6"};Expected=false}
            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 10u};W={defaultWindowsVersion with Comparison="GreaterThan";MajorVersion="6"};Expected=true}



            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 5u;MinorVersion=1u};W={defaultWindowsVersion with Comparison="LessThan";MajorVersion="6";MinorVersion="1"};Expected=true}
            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 6u;MinorVersion=1u};W={defaultWindowsVersion with Comparison="LessThan";MajorVersion="6";MinorVersion="1"};Expected=false}
            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 10u;MinorVersion=1u};W={defaultWindowsVersion with Comparison="LessThan";MajorVersion="6";MinorVersion="1"};Expected=false}

            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 5u;MinorVersion=1u};W={defaultWindowsVersion with Comparison="LessThanOrEqualTo";MajorVersion="6";MinorVersion="1"};Expected=true}
            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 6u;MinorVersion=1u};W={defaultWindowsVersion with Comparison="LessThanOrEqualTo";MajorVersion="6";MinorVersion="1"};Expected=true}
            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 10u;MinorVersion=1u};W={defaultWindowsVersion with Comparison="LessThanOrEqualTo";MajorVersion="6";MinorVersion="1"};Expected=false}

            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 5u;MinorVersion=1u};W={defaultWindowsVersion with Comparison="EqualTo";MajorVersion="6";MinorVersion="1"};Expected=false}
            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 6u;MinorVersion=1u};W={defaultWindowsVersion with Comparison="EqualTo";MajorVersion="6";MinorVersion="1"};Expected=true}
            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 10u;MinorVersion=1u};W={defaultWindowsVersion with Comparison="EqualTo";MajorVersion="6";MinorVersion="1"};Expected=false}


            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 5u;MinorVersion=1u};W={defaultWindowsVersion with Comparison="GreaterThanOrEqualTo";MajorVersion="6";MinorVersion="1"};Expected=false}
            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 6u;MinorVersion=1u};W={defaultWindowsVersion with Comparison="GreaterThanOrEqualTo";MajorVersion="6";MinorVersion="1"};Expected=true}
            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 10u;MinorVersion=1u};W={defaultWindowsVersion with Comparison="GreaterThanOrEqualTo";MajorVersion="6";MinorVersion="1"};Expected=true}

            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 5u;MinorVersion=1u};W={defaultWindowsVersion with Comparison="GreaterThan";MajorVersion="6";MinorVersion="1"};Expected=false}
            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 6u;MinorVersion=1u};W={defaultWindowsVersion with Comparison="GreaterThan";MajorVersion="6";MinorVersion="1"};Expected=false}
            yield {WEx={defaultCurrentWindowsVersion with MajorVersion = 10u;MinorVersion=1u};W={defaultWindowsVersion with Comparison="GreaterThan";MajorVersion="6";MinorVersion="1"};Expected=true}
        ]
    

    [<Test>]
    [<TestCaseSource("currentWindowsVersionsTestData")>]
    let isWindowsVersionsTests(testData:TestData) = 
        let actual = isWindowsVersion testData.WEx testData.W 
        Assert.AreEqual(testData.Expected,actual,(sprintf "'%A' Not %s  '%A'" testData.WEx testData.W.Comparison testData.W))
        ()

