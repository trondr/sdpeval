namespace sdpeval.tests

open NUnit.Framework

[<TestFixture>]
module FileVersionTest =
    open sdpeval.BaseApplicabilityRules
    open sdpeval.BaseTypes
        
    let internal defaultFileVersion = {Csidl=Some "37";Path="drivers\\some.sys";Comparison="EqualTo";Version="1.0.0.0"}
    type internal TestData ={CurrentFileVersion:FileVersion;FileVersion:FileVersion;Expected:bool}

    let internal currentFileVersionTestData =
        [
            yield {CurrentFileVersion={defaultFileVersion with Version = "1.0.0.0"};FileVersion={defaultFileVersion with Version="1.0.0.0";Comparison=(toScalarComparisonString ScalarComparison.EqualTo)};Expected=true}
            yield {CurrentFileVersion={defaultFileVersion with Version = "2.0.0.0"};FileVersion={defaultFileVersion with Version="1.0.0.0";Comparison=(toScalarComparisonString ScalarComparison.EqualTo)};Expected=false}
            yield {CurrentFileVersion={defaultFileVersion with Version = "1.0.0.0"};FileVersion={defaultFileVersion with Version="2.0.0.0";Comparison=(toScalarComparisonString ScalarComparison.EqualTo)};Expected=false}

            yield {CurrentFileVersion={defaultFileVersion with Version = "1.0.0.0"};FileVersion={defaultFileVersion with Version="1.0.0.0";Comparison=(toScalarComparisonString ScalarComparison.GreaterThan)};Expected=false}
            yield {CurrentFileVersion={defaultFileVersion with Version = "2.0.0.0"};FileVersion={defaultFileVersion with Version="1.0.0.0";Comparison=(toScalarComparisonString ScalarComparison.GreaterThan)};Expected=true}
            yield {CurrentFileVersion={defaultFileVersion with Version = "1.0.0.0"};FileVersion={defaultFileVersion with Version="2.0.0.0";Comparison=(toScalarComparisonString ScalarComparison.GreaterThan)};Expected=false}

            yield {CurrentFileVersion={defaultFileVersion with Version = "1.0.0.0"};FileVersion={defaultFileVersion with Version="1.0.0.0";Comparison=(toScalarComparisonString ScalarComparison.GreaterThanOrEqualTo)};Expected=true}
            yield {CurrentFileVersion={defaultFileVersion with Version = "2.0.0.0"};FileVersion={defaultFileVersion with Version="1.0.0.0";Comparison=(toScalarComparisonString ScalarComparison.GreaterThanOrEqualTo)};Expected=true}
            yield {CurrentFileVersion={defaultFileVersion with Version = "1.0.0.0"};FileVersion={defaultFileVersion with Version="2.0.0.0";Comparison=(toScalarComparisonString ScalarComparison.GreaterThanOrEqualTo)};Expected=false}

            yield {CurrentFileVersion={defaultFileVersion with Version = "1.0.0.0"};FileVersion={defaultFileVersion with Version="1.0.0.0";Comparison=(toScalarComparisonString ScalarComparison.LessThan)};Expected=false}
            yield {CurrentFileVersion={defaultFileVersion with Version = "2.0.0.0"};FileVersion={defaultFileVersion with Version="1.0.0.0";Comparison=(toScalarComparisonString ScalarComparison.LessThan)};Expected=false}
            yield {CurrentFileVersion={defaultFileVersion with Version = "1.0.0.0"};FileVersion={defaultFileVersion with Version="2.0.0.0";Comparison=(toScalarComparisonString ScalarComparison.LessThan)};Expected=true}

            yield {CurrentFileVersion={defaultFileVersion with Version = "1.0.0.0"};FileVersion={defaultFileVersion with Version="1.0.0.0";Comparison=(toScalarComparisonString ScalarComparison.LessThanOrEqualTo)};Expected=true}
            yield {CurrentFileVersion={defaultFileVersion with Version = "2.0.0.0"};FileVersion={defaultFileVersion with Version="1.0.0.0";Comparison=(toScalarComparisonString ScalarComparison.LessThanOrEqualTo)};Expected=false}
            yield {CurrentFileVersion={defaultFileVersion with Version = "1.0.0.0"};FileVersion={defaultFileVersion with Version="2.0.0.0";Comparison=(toScalarComparisonString ScalarComparison.LessThanOrEqualTo)};Expected=true}

            yield {CurrentFileVersion={defaultFileVersion with Version = "1.0.010.1"};FileVersion={defaultFileVersion with Version="1.0.10.1";Comparison=(toScalarComparisonString ScalarComparison.EqualTo)};Expected=true}

            yield {CurrentFileVersion={defaultFileVersion with Version = "10.0.010.1"};FileVersion={defaultFileVersion with Version="1.0.10.1";Comparison=(toScalarComparisonString ScalarComparison.GreaterThan)};Expected=true}
            yield {CurrentFileVersion={defaultFileVersion with Version = "10.0.010.1"};FileVersion={defaultFileVersion with Version="10.0.010.2";Comparison=(toScalarComparisonString ScalarComparison.LessThan)};Expected=true}

        ]
    [<Test>]
    [<Category(TestCategory.UnitTests)>]
    [<TestCaseSource("currentFileVersionTestData")>]
    let isFileVersionTests(testData:obj) = 
        let testDataR = (testData:?>TestData)
        let actual = sdpeval.FileVersion.isFileVersionBase testDataR.CurrentFileVersion testDataR.FileVersion
        Assert.AreEqual(testDataR.Expected,actual,(sprintf "'%A' Not %s  '%A'" testDataR.CurrentFileVersion testDataR.FileVersion.Comparison testDataR.FileVersion))
        ()
