namespace sdpeval.tests

module MsiTests =
    open NUnit.Framework
    open sdpeval.Msi
    open sdpeval.BaseApplicabilityRules
    open System
    
    [<Test>]
    [<TestCase("01.Installed product->True","AllModels","{1784A8CD-F7FE-47E2-A87D-1F31E7242D0D}","1.6.7.1","Microsoft .NET Framework 4.7.2 Targeting Pack",true)>]
    [<TestCase("02.Not installed product->False","AllModels","{1216B4E6-D66B-46c3-BFA5-A170274B5BE2}","1.6.7.1","Some unknown product that should not be installed",false)>]    
    [<Category(TestCategory.ManualTests)>]
    let isMsiProductInstalledTest(testName,validModel,productCode:string, versionMin:string,displayName, expected:bool) =
        let testIsRelevant = TestHelper.IsTestRelevant testName validModel
        match testIsRelevant with
        |false ->
            printfn "WARNING: Skipped test '%s'" testName
            Assert.Inconclusive()
        |true -> 
            printfn "Process size: %A" IntPtr.Size       
            let msiProductInstalled = {ProductCode=productCode;VersionMin=Some versionMin;VersionMax=None;ExcludeVersionMin=None;ExcludeVersionMax=None;Languange=None}
            let actual = sdpeval.Msi.isMsiProductInstalled msiProductInstalled
            Assert.AreEqual(expected,actual,sprintf "%s %s (%s) %b" productCode versionMin displayName expected)

    type internal TestData =
        {
            TestName:string
            InstalledProductInfo:MsiProductInfo;
            ProductInstalledRule: MsiProductInstalled
            Expected:bool
        }

    let internal testData = 
        [
            yield {        
                TestName="1 Language rule is defined, but language is not found on installed product. Return false."
                InstalledProductInfo={ProductCode=msiProductCode "{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";IsInstalled=true;ProductVersion=toProductVersion (Some "1.6.7.2"); ProductLanguage=None};
                ProductInstalledRule={ProductCode="{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";VersionMin=(Some "1.6.7.1");VersionMax=(Some "1.6.7.2");ExcludeVersionMin=None;ExcludeVersionMax=None;Languange=Some "1033"}                
                Expected=false}

            yield {                
                TestName="2 Version rules are defined but version is not found on installed product. Return false."
                InstalledProductInfo={ProductCode=msiProductCode "{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";IsInstalled=true;ProductVersion=toProductVersion None; ProductLanguage=None};
                ProductInstalledRule={ProductCode="{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";VersionMin=(Some "1.6.7.1");VersionMax=(Some "1.6.7.2");ExcludeVersionMin=None;ExcludeVersionMax=None;Languange=None}                
                Expected=false}

            yield {                
                TestName="3 Version rules are defined but EXCLUDED. Version is not found on installed product. Return false."
                InstalledProductInfo={ProductCode=msiProductCode "{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";IsInstalled=true;ProductVersion=toProductVersion None; ProductLanguage=None};
                ProductInstalledRule={ProductCode="{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";VersionMin=(Some "1.6.7.1");VersionMax=(Some "1.6.7.2");ExcludeVersionMin=Some "true";ExcludeVersionMax=Some "true";Languange=None}                
                Expected=false}
            
            yield {                
                TestName="3.1 Version rules are defined but EXCLUDED(false). Version is not found on installed product. Return false."
                InstalledProductInfo={ProductCode=msiProductCode "{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";IsInstalled=true;ProductVersion=toProductVersion None; ProductLanguage=None};
                ProductInstalledRule={ProductCode="{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";VersionMin=(Some "1.6.7.1");VersionMax=(Some "1.6.7.2");ExcludeVersionMin=Some "false";ExcludeVersionMax=Some "false";Languange=None}                
                Expected=false}

            yield {
                TestName="4 Version rules are defined. Installed product is matching on max version. Return true."
                InstalledProductInfo={ProductCode=msiProductCode "{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";IsInstalled=true;ProductVersion=toProductVersion (Some "1.6.7.2"); ProductLanguage=Some (msiProductLanguage 1033)};
                ProductInstalledRule={ProductCode="{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";VersionMin=(Some "1.6.7.1");VersionMax=(Some "1.6.7.2");ExcludeVersionMin=None;ExcludeVersionMax=None;Languange=None}                
                Expected=true}

            yield {
                TestName="5 Version rules are defined but EXCLUDED. Installed product is matching on max version. Return false."
                InstalledProductInfo={ProductCode=msiProductCode "{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";IsInstalled=true;ProductVersion=toProductVersion (Some "1.6.7.2"); ProductLanguage=Some (msiProductLanguage 1033)};
                ProductInstalledRule={ProductCode="{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";VersionMin=(Some "1.6.7.1");VersionMax=(Some "1.6.7.2");ExcludeVersionMin=Some "true";ExcludeVersionMax=Some "true";Languange=None}                
                Expected=false}

            yield {
                TestName="5.1 Version rules are defined but EXCLUDED(false). Installed product is matching on max version. Return true."
                InstalledProductInfo={ProductCode=msiProductCode "{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";IsInstalled=true;ProductVersion=toProductVersion (Some "1.6.7.2"); ProductLanguage=Some (msiProductLanguage 1033)};
                ProductInstalledRule={ProductCode="{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";VersionMin=(Some "1.6.7.1");VersionMax=(Some "1.6.7.2");ExcludeVersionMin=Some "false";ExcludeVersionMax=Some "false";Languange=None}                
                Expected=true}

            yield {
                TestName="6 Version rules are defined. Installed product is matching on min version. Return true."
                InstalledProductInfo={ProductCode=msiProductCode "{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";IsInstalled=true;ProductVersion=toProductVersion (Some "1.6.7.1"); ProductLanguage=Some (msiProductLanguage 1033)};
                ProductInstalledRule={ProductCode="{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";VersionMin=(Some "1.6.7.1");VersionMax=(Some "1.6.7.2");ExcludeVersionMin=None;ExcludeVersionMax=None;Languange=None}                
                Expected=true}

            yield {
                TestName="7 Version rules are defined but EXCLUDED. Installed product is matching on min version. Return false."
                InstalledProductInfo={ProductCode=msiProductCode "{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";IsInstalled=true;ProductVersion=toProductVersion (Some "1.6.7.1"); ProductLanguage=Some (msiProductLanguage 1033)};
                ProductInstalledRule={ProductCode="{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";VersionMin=(Some "1.6.7.1");VersionMax=(Some "1.6.7.2");ExcludeVersionMin=Some "true";ExcludeVersionMax=Some "true";Languange=None}                
                Expected=false}

            yield {
                TestName="7.1 Version rules are defined but EXCLUDED(false). Installed product is matching on min version. Return true."
                InstalledProductInfo={ProductCode=msiProductCode "{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";IsInstalled=true;ProductVersion=toProductVersion (Some "1.6.7.1"); ProductLanguage=Some (msiProductLanguage 1033)};
                ProductInstalledRule={ProductCode="{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";VersionMin=(Some "1.6.7.1");VersionMax=(Some "1.6.7.2");ExcludeVersionMin=Some "false";ExcludeVersionMax=Some "false";Languange=None}                
                Expected=true}
            
            yield {
                TestName="8 Version rules are defined. Installed product is lower than min version. Return false."
                InstalledProductInfo={ProductCode=msiProductCode "{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";IsInstalled=true;ProductVersion=toProductVersion (Some "1.6.7.0"); ProductLanguage=Some (msiProductLanguage 1033)};
                ProductInstalledRule={ProductCode="{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";VersionMin=(Some "1.6.7.1");VersionMax=(Some "1.6.7.2");ExcludeVersionMin=None;ExcludeVersionMax=None;Languange=None}                
                Expected=false}
            
            yield {
                TestName="9 Version rules are defined but EXCLUDED. Installed product is lower than min version. Return false."
                InstalledProductInfo={ProductCode=msiProductCode "{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";IsInstalled=true;ProductVersion=toProductVersion (Some "1.6.7.0"); ProductLanguage=Some (msiProductLanguage 1033)};
                ProductInstalledRule={ProductCode="{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";VersionMin=(Some "1.6.7.1");VersionMax=(Some "1.6.7.2");ExcludeVersionMin=Some "true";ExcludeVersionMax=Some "true";Languange=None}                
                Expected=false}
            
            yield {
                TestName="10 Version rules are defined. Installed product is higher than max version. Return false."
                InstalledProductInfo={ProductCode=msiProductCode "{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";IsInstalled=true;ProductVersion=toProductVersion (Some "1.6.7.3"); ProductLanguage=Some (msiProductLanguage 1033)};
                ProductInstalledRule={ProductCode="{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";VersionMin=(Some "1.6.7.1");VersionMax=(Some "1.6.7.2");ExcludeVersionMin=None;ExcludeVersionMax=None;Languange=None}                
                Expected=false}

            yield {
                TestName="11 Version rules are defined but EXCLUDED. Installed product is higher than max version. Return false."
                InstalledProductInfo={ProductCode=msiProductCode "{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";IsInstalled=true;ProductVersion=toProductVersion (Some "1.6.7.3"); ProductLanguage=Some (msiProductLanguage 1033)};
                ProductInstalledRule={ProductCode="{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";VersionMin=(Some "1.6.7.1");VersionMax=(Some "1.6.7.2");ExcludeVersionMin=Some "true";ExcludeVersionMax=Some "true";Languange=None}                
                Expected=false}
            
            yield {        
                TestName="12 Version rules are defined and equal. Installed product is higher than max version. Return false."
                InstalledProductInfo={ProductCode=msiProductCode "{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";IsInstalled=true;ProductVersion=toProductVersion (Some "1.6.7.2"); ProductLanguage=Some (msiProductLanguage 1033)};
                ProductInstalledRule={ProductCode="{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";VersionMin=(Some "1.6.7.1");VersionMax=(Some "1.6.7.1");ExcludeVersionMin=None;ExcludeVersionMax=None;Languange=None}                
                Expected=false}

            yield {        
                TestName="13 Version rules are defined and equal but EXCLUDED. Installed product is higher than max version. Return false."
                InstalledProductInfo={ProductCode=msiProductCode "{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";IsInstalled=true;ProductVersion=toProductVersion (Some "1.6.7.2"); ProductLanguage=Some (msiProductLanguage 1033)};
                ProductInstalledRule={ProductCode="{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";VersionMin=(Some "1.6.7.1");VersionMax=(Some "1.6.7.1");ExcludeVersionMin=Some "true";ExcludeVersionMax=Some "true";Languange=None}                
                Expected=false}
            
            yield {        
                TestName="14 Version rules are defined and equal. Installed product is lower than min version. Return false."
                InstalledProductInfo={ProductCode=msiProductCode "{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";IsInstalled=true;ProductVersion=toProductVersion (Some "1.6.7.0"); ProductLanguage=Some (msiProductLanguage 1033)};
                ProductInstalledRule={ProductCode="{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";VersionMin=(Some "1.6.7.1");VersionMax=(Some "1.6.7.1");ExcludeVersionMin=None;ExcludeVersionMax=None;Languange=None}                
                Expected=false}
            
            yield {        
                TestName="15 Version rules are defined and equal but EXCLUDED. Installed product is lower than min version. Return false."
                InstalledProductInfo={ProductCode=msiProductCode "{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";IsInstalled=true;ProductVersion=toProductVersion (Some "1.6.7.0"); ProductLanguage=Some (msiProductLanguage 1033)};
                ProductInstalledRule={ProductCode="{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";VersionMin=(Some "1.6.7.1");VersionMax=(Some "1.6.7.1");ExcludeVersionMin=Some "true";ExcludeVersionMax=Some "true";Languange=None}                
                Expected=false}

            yield {        
                TestName="16 Min version rule is defined. Installed product has the same version. Return true."
                InstalledProductInfo={ProductCode=msiProductCode "{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";IsInstalled=true;ProductVersion=toProductVersion (Some "1.6.7.1"); ProductLanguage=Some (msiProductLanguage 1033)};
                ProductInstalledRule={ProductCode="{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";VersionMin=(Some "1.6.7.1");VersionMax=None;ExcludeVersionMin=None;ExcludeVersionMax=None;Languange=None}                
                Expected=true}
            
            yield {        
                TestName="17 Min version rule is defined but EXCLUDED. Installed product has the same version. Return false."
                InstalledProductInfo={ProductCode=msiProductCode "{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";IsInstalled=true;ProductVersion=toProductVersion (Some "1.6.7.1"); ProductLanguage=Some (msiProductLanguage 1033)};
                ProductInstalledRule={ProductCode="{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";VersionMin=(Some "1.6.7.1");VersionMax=None;ExcludeVersionMin=Some "true";ExcludeVersionMax=None;Languange=None}                
                Expected=false}

            yield {        
                TestName="18 Min version rule is defined. Installed product has higher version. Return true."
                InstalledProductInfo={ProductCode=msiProductCode "{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";IsInstalled=true;ProductVersion=toProductVersion (Some "1.6.7.2"); ProductLanguage=Some (msiProductLanguage 1033)};
                ProductInstalledRule={ProductCode="{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";VersionMin=(Some "1.6.7.1");VersionMax=None;ExcludeVersionMin=None;ExcludeVersionMax=None;Languange=None}                
                Expected=true}

            yield {        
                TestName="19 Min version rule is defined but EXCLUDED. Installed product has higher version. Return true."
                InstalledProductInfo={ProductCode=msiProductCode "{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";IsInstalled=true;ProductVersion=toProductVersion (Some "1.6.7.2"); ProductLanguage=Some (msiProductLanguage 1033)};
                ProductInstalledRule={ProductCode="{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";VersionMin=(Some "1.6.7.1");VersionMax=None;ExcludeVersionMin=Some "true";ExcludeVersionMax=None;Languange=None}                
                Expected=true}

            yield {        
                TestName="20 Min version rule is defined. Installed product has lower version. Return false."
                InstalledProductInfo={ProductCode=msiProductCode "{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";IsInstalled=true;ProductVersion=toProductVersion (Some "1.6.7.0"); ProductLanguage=Some (msiProductLanguage 1033)};
                ProductInstalledRule={ProductCode="{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";VersionMin=(Some "1.6.7.1");VersionMax=None;ExcludeVersionMin=None;ExcludeVersionMax=None;Languange=None}                
                Expected=false}

            yield {        
                TestName="21 Min version rule is defined but EXCLUDED. Installed product has lower version. Return false."
                InstalledProductInfo={ProductCode=msiProductCode "{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";IsInstalled=true;ProductVersion=toProductVersion (Some "1.6.7.0"); ProductLanguage=Some (msiProductLanguage 1033)};
                ProductInstalledRule={ProductCode="{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";VersionMin=(Some "1.6.7.1");VersionMax=None;ExcludeVersionMin=Some "true";ExcludeVersionMax=None;Languange=None}                
                Expected=false}

            yield {        
                TestName="22 Max version rule is defined. Installed product has the same version. Return true."
                InstalledProductInfo={ProductCode=msiProductCode "{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";IsInstalled=true;ProductVersion=toProductVersion (Some "1.6.7.1"); ProductLanguage=Some (msiProductLanguage 1033)};
                ProductInstalledRule={ProductCode="{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";VersionMin=None;VersionMax=(Some "1.6.7.1");ExcludeVersionMin=None;ExcludeVersionMax=None;Languange=None}                
                Expected=true}

            yield {        
                TestName="23 Max version rule is defined but EXCLUDED. Installed product has the same version. Return false."
                InstalledProductInfo={ProductCode=msiProductCode "{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";IsInstalled=true;ProductVersion=toProductVersion (Some "1.6.7.1"); ProductLanguage=Some (msiProductLanguage 1033)};
                ProductInstalledRule={ProductCode="{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";VersionMin=None;VersionMax=(Some "1.6.7.1");ExcludeVersionMin=None;ExcludeVersionMax=Some "true";Languange=None}                
                Expected=false}
            
            yield {        
                TestName="24 Max version rule is defined. Installed product has higher version. Return false."
                InstalledProductInfo={ProductCode=msiProductCode "{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";IsInstalled=true;ProductVersion=toProductVersion (Some "1.6.7.2"); ProductLanguage=Some (msiProductLanguage 1033)};
                ProductInstalledRule={ProductCode="{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";VersionMin=None;VersionMax=(Some "1.6.7.1");ExcludeVersionMin=None;ExcludeVersionMax=None;Languange=None}                
                Expected=false}
            
            yield {        
                TestName="25 Max version rule is defined but EXCLUDED. Installed product has higher version. Return false."
                InstalledProductInfo={ProductCode=msiProductCode "{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";IsInstalled=true;ProductVersion=toProductVersion (Some "1.6.7.2"); ProductLanguage=Some (msiProductLanguage 1033)};
                ProductInstalledRule={ProductCode="{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";VersionMin=None;VersionMax=(Some "1.6.7.1");ExcludeVersionMin=None;ExcludeVersionMax=Some "true";Languange=None}
                Expected=false}
            
            yield {        
                TestName="26 Max version rule is defined. Installed product has lower version. Return true."
                InstalledProductInfo={ProductCode=msiProductCode "{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";IsInstalled=true;ProductVersion=toProductVersion (Some "1.6.7.0"); ProductLanguage=Some (msiProductLanguage 1033)};
                ProductInstalledRule={ProductCode="{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";VersionMin=None;VersionMax=(Some "1.6.7.1");ExcludeVersionMin=None;ExcludeVersionMax=None;Languange=None}                
                Expected=true}
           
            yield {        
                TestName="27 Max version rule is defined but EXCLUDED. Installed product has lower version. Return true."
                InstalledProductInfo={ProductCode=msiProductCode "{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";IsInstalled=true;ProductVersion=toProductVersion (Some "1.6.7.0"); ProductLanguage=Some (msiProductLanguage 1033)};
                ProductInstalledRule={ProductCode="{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";VersionMin=None;VersionMax=(Some "1.6.7.1");ExcludeVersionMin=None;ExcludeVersionMax=Some "true";Languange=None}
                Expected=true}

            yield {        
                TestName="28 Version and language rules are defined. Product is installed but different language. Return false."
                InstalledProductInfo={ProductCode=msiProductCode "{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";IsInstalled=true;ProductVersion=toProductVersion (Some "1.6.7.1"); ProductLanguage=Some (msiProductLanguage 1044)};
                ProductInstalledRule={ProductCode="{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";VersionMin=(Some "1.6.7.1");VersionMax=(Some "1.6.7.1");ExcludeVersionMin=None;ExcludeVersionMax=None;Languange=Some "1033"}                
                Expected=false}

            yield {        
                TestName="29 Version and language rules are defined. Product is installed with same version and language. Return true."
                InstalledProductInfo={ProductCode=msiProductCode "{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";IsInstalled=true;ProductVersion=toProductVersion (Some "1.6.7.1"); ProductLanguage=Some (msiProductLanguage 1033)};
                ProductInstalledRule={ProductCode="{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";VersionMin=(Some "1.6.7.1");VersionMax=(Some "1.6.7.1");ExcludeVersionMin=None;ExcludeVersionMax=None;Languange=Some "1033"}                
                Expected=true}
            
            yield {        
                TestName="30 Version and language rules are defined. Product is not installed. Return false."
                InstalledProductInfo={ProductCode=msiProductCode "{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";IsInstalled=false;ProductVersion=None; ProductLanguage=None};
                ProductInstalledRule={ProductCode="{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";VersionMin=(Some "1.6.7.1");VersionMax=(Some "1.6.7.1");ExcludeVersionMin=None;ExcludeVersionMax=None;Languange=Some "1033"}                
                Expected=false}
            
            yield {        
                TestName="31 No rules are defined. Product is not installed. Return false."
                InstalledProductInfo={ProductCode=msiProductCode "{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";IsInstalled=false;ProductVersion=None; ProductLanguage=None};
                ProductInstalledRule={ProductCode="{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";VersionMin=(Some "1.6.7.1");VersionMax=(Some "1.6.7.1");ExcludeVersionMin=None;ExcludeVersionMax=None;Languange=Some "1033"}                
                Expected=false}
            
            yield {        
                TestName="32 No rules are defined. Product is installed. Return true."
                InstalledProductInfo={ProductCode=msiProductCode "{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";IsInstalled=true;ProductVersion=toProductVersion (Some "1.6.7.1"); ProductLanguage=Some (msiProductLanguage 1033)};
                ProductInstalledRule={ProductCode="{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";VersionMin=None;VersionMax=None;ExcludeVersionMin=None;ExcludeVersionMax=None;Languange=None}                
                Expected=true}
            
            yield {        
                TestName="33 No rules are defined. Product is installed with unknown version. Return true."
                InstalledProductInfo={ProductCode=msiProductCode "{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";IsInstalled=true;ProductVersion=None; ProductLanguage=Some (msiProductLanguage 1033)};
                ProductInstalledRule={ProductCode="{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";VersionMin=None;VersionMax=None;ExcludeVersionMin=None;ExcludeVersionMax=None;Languange=None}                
                Expected=true}
            
            yield {        
                TestName="34 No rules are defined. Product is installed with unknown version and unknown language. Return true."
                InstalledProductInfo={ProductCode=msiProductCode "{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";IsInstalled=true;ProductVersion=None; ProductLanguage=None};
                ProductInstalledRule={ProductCode="{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";VersionMin=None;VersionMax=None;ExcludeVersionMin=None;ExcludeVersionMax=None;Languange=None}                
                Expected=true}

            yield {        
                TestName="35 Invalid language rule is defined. Product is installed with unknown version and empty language. Return true."
                InstalledProductInfo={ProductCode=msiProductCode "{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";IsInstalled=true;ProductVersion=None; ProductLanguage=None};
                ProductInstalledRule={ProductCode="{00D7C76E-B036-4DF5-AA89-D36A3D494BC6}";VersionMin=None;VersionMax=None;ExcludeVersionMin=None;ExcludeVersionMax=None;Languange=Some "abc"}
                Expected=true}
             
          ]

    [<Test>]
    [<TestCaseSource("testData")>]
    [<Category(TestCategory.UnitTests)>]
    let isMsiProductInstalledBaseTest (testData:obj) =
        let testDataR = (testData:?>TestData)

        let getProductInfoFunc (productCode:MsiProductCode) =
            testDataR.InstalledProductInfo

        let actual = sdpeval.Msi.isMsiProductInstalledBase getProductInfoFunc testDataR.ProductInstalledRule
        Assert.AreEqual(testDataR.Expected,actual,sprintf "%A" testDataR)