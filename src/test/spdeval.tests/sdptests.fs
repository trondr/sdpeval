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