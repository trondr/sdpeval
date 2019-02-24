namespace sdpeval.tests

open NUnit.Framework

[<TestFixture>]
module FileVersionTest =
    open sdpeval.BaseApplicabilityRules
    open sdpeval.BaseTypes
        
    let defaultFileVersion = {Csidl=Some "37";Path="drivers\\some.sys";Comparison="EqualTo";Version="1.0.0.0"}
    type TestData ={CurrentFileVersion:FileVersion;FileVersion:FileVersion;Expected:bool}

    let currentFileVersionTestData =
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
    [<TestCaseSource("currentFileVersionTestData")>]
    let isFileVersionTests(testData:TestData) = 
        let actual = sdpeval.FileVersion.isFileVersion testData.CurrentFileVersion testData.FileVersion
        Assert.AreEqual(testData.Expected,actual,(sprintf "'%A' Not %s  '%A'" testData.CurrentFileVersion testData.FileVersion.Comparison testData.FileVersion))
        ()
