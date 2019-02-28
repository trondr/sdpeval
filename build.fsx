#r "paket:
nuget NUnit.ConsoleRunner
nuget Fake.IO.FileSystem
nuget Fake.DotNet.MSBuild
nuget Fake.DotNet.Testing.Nunit
nuget Fake.Testing.Common
nuget Fake.IO.Zip
nuget Fake.Core.Target //"
#load "./.fake/build.fsx/intellisense.fsx"

open Fake.IO
open Fake.IO.Globbing.Operators //enables !! and globbing
open Fake.DotNet
open Fake.Core
open Fake.Testing
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

//Targets
Target.create "Clean" (fun _ ->
    Trace.trace "Clean build folder..."
    Shell.cleanDirs [ buildFolder; artifactFolder ]
)

Target.create "BuildLib" (fun _ -> 
    Trace.trace "Building lib..."
    !! "src/lib/**/*.fsproj"
        |> MSBuild.runRelease id buildLibFolder "Build"
        |> Trace.logItems "BuildApp-Output: "
)

Target.create "BuildTest" (fun _ -> 
    Trace.trace "Building test..."
    !! "src/test/**/*.fsproj"
        |> MSBuild.runRelease id buildTestFolder "Build"
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
    ==> "Publish"
    ==> "Default"

//Start build
Target.runOrDefault "Default"