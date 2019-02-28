namespace sdpeval.tests

module NativeMethodsTests =
    
    open NUnit.Framework
    open sdpeval
    open sdpeval.NativeMethods

    [<Test>]
    [<Category(TestCategory.UnitTests)>]
    [<TestCase(Csidl.Admintools,@"Start Menu\Programs\Administrative Tools")>]
    [<TestCase(Csidl.CommonAppdata,@"C:\ProgramData")>]
    [<TestCase(Csidl.CommonMusic,@"\Music")>]
    [<TestCase(Csidl.CommonPrograms,@"C:\ProgramData\Microsoft\Windows\Start Menu\Programs")>]
    let getFolderTests(csidlFolder,expectedPath) =
        let actual = NativeMethods.getFolderPath csidlFolder
        Assert.IsTrue(actual.Contains(expectedPath), sprintf "'%s' is not contained in '%s'" expectedPath actual)

    
    [<Test>]
    [<Category(TestCategory.UnitTests)>]
    let getSystemInfoTest () =
        let actual = NativeMethods.getSystemInfo ()
        Assert.IsTrue((0us = actual.wProcessorArchitecture)||(9us = actual.wProcessorArchitecture))

    [<Test>]
    [<Category(TestCategory.UnitTests)>]
    let getNativeSystemInfoTest () =
        let actual = NativeMethods.getNativeSystemInfo ()
        Assert.AreEqual(9, actual.wProcessorArchitecture)

    [<Test>]
    [<Category(TestCategory.UnitTests)>]
    let getOsVersionTest () =
        let actual = NativeMethods.getOsVersion ()
        Assert.AreEqual(10, actual.dwMajorVersion)