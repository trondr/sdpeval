#r "paket:
nuget NUnit.ConsoleRunner
nuget Fake.IO.FileSystem
nuget Fake.DotNet.MSBuild
nuget Fake.DotNet.Testing.Nunit
nuget Fake.Testing.Common
nuget Fake.DotNet.NuGet
nuget Fake.IO.Zip
nuget Fake.Core.Target //"
#load "./.fake/build.fsx/intellisense.fsx"

open Fake.IO
open Fake.IO.Globbing.Operators //enables !! and globbing
open Fake.Core
open Fake.DotNet.Testing

//Properties
let buildFolder = System.IO.Path.GetFullPath("./build/")
let buildLibFolder = buildFolder + "lib"
let buildTestFolder = buildFolder + "test"
let artifactFolder = System.IO.Path.GetFullPath("./artifact/")
let artifactAppFolder = artifactFolder + "app"

let globalPackagesFolder =     
    System.Environment.ExpandEnvironmentVariables("%userprofile%\.nuget\packages")

let getVersion file = 
    System.Reflection.AssemblyName.GetAssemblyName(file).Version.ToString()

let nuspecTemplate () =
    !! "src/lib/**/sdpeval.nuspec"
    |> Seq.head

let nuspecTarget =
    System.IO.Path.Combine(buildLibFolder,"sdpeval.nuspec")    

let copyPackFiles () =
    !! "build/lib/**/sdpeval.dll"
    ++ "build/lib/**/sdpeval.pdb"
    ++ "build/lib/**/Microsoft.UpdateServices.Administration.dll"
    |> Shell.copy "Nuget"        

//Targets
Target.create "Clean" (fun _ ->
    Trace.trace "Clean build folder..."
    Shell.cleanDirs [ buildFolder; artifactFolder ]
)

Target.create "BuildLib" (fun _ -> 
    Trace.trace "Building lib..."
    !! "src/lib/**/*.fsproj"
        |> Fake.DotNet.MSBuild.runRelease id buildLibFolder "Build"
        |> Trace.logItems "BuildApp-Output: "
)

Target.create "BuildTest" (fun _ -> 
    Trace.trace "Building test..."
    !! "src/test/**/*.fsproj"
        |> Fake.DotNet.MSBuild.runRelease id buildTestFolder "Build"
        |> Trace.logItems "BuildTest-Output: "
)

let nunitConsoleRunner() =
    let consoleRunner = 
        let search = 
            !! (globalPackagesFolder + "/**/nunit3-console.exe")
        if (Seq.isEmpty search) then 
            (failwith "Could not locate nunit3-console.exe in ./packages folder.")
        else 
            (search |> Seq.head)        
    printfn "Console runner:  %s" consoleRunner
    consoleRunner

Target.create "Test" (fun _ -> 
    Trace.trace "Testing app..."    
    !! ("build/test/**/*.Tests.dll")    
    |> NUnit3.run (fun p ->
        {p with ToolPath = nunitConsoleRunner();Where = "cat==UnitTests";TraceLevel=NUnit3.NUnit3TraceLevel.Verbose})
)

let setNugetParameters (nugetParams:Fake.DotNet.NuGet.NuGet.NuGetParams) =
    let nparams = {nugetParams with Authors=["github.com/trondr"];Version="1.0.0.0";Description="Evaluatessss WSUS Software Distribution Package applicability rules as defined in WSUS schema reference https://docs.microsoft.com/en-us/previous-versions/windows/desktop/bb972752(v=vs.85)";Project="https://github.com/trondr/sdpeval"}
    printfn "NuGet Paramaters: %A" nparams
    nparams

Target.create "CreateNugetPackage" (fun _ ->
    Trace.trace "Creating nuget package..."
    copyPackFiles ()
    System.IO.File.Copy(nuspecTemplate(),nuspecTarget,true)
    Fake.DotNet.NuGet.NuGet.NuGetPack setNugetParameters nuspecTarget //@"C:\Dev\github.trondr\sdpeval\src\lib\sdpeval\sdpeval.fsproj"
)

Target.create "Publish" (fun _ ->
    Trace.trace "Publishing app..."
    let assemblyVersion = getVersion (System.IO.Path.Combine(buildLibFolder,"sdpeval.dll"))
    let files = 
        [|
            System.IO.Path.Combine(buildLibFolder,"sdpeval.dll")
            System.IO.Path.Combine(buildLibFolder,"sdpeval.pdb")            
            System.IO.Path.Combine(buildLibFolder,"FSharp.Core.dll")
        |]
    let zipFile = System.IO.Path.Combine(artifactFolder,sprintf "sdpeval.%s.zip" assemblyVersion)
    files
    |> Fake.IO.Zip.createZip buildLibFolder zipFile (sprintf "sdpeval %s" assemblyVersion) 9 false 
)

Target.create "Default" (fun _ ->
    Trace.trace "Hello world from FAKE"
)

//Dependencies
open Fake.Core.TargetOperators

"Clean" 
    ==> "BuildLib"
    ==> "BuildTest"
    ==> "Test"
    ==> "CreateNugetPackage"
    ==> "Publish"
    ==> "Default"

//Start build
Target.runOrDefault "Default"