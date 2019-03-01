namespace sdpeval.AssemblyInfo

open System.Reflection
open System.Runtime.CompilerServices
open System.Runtime.InteropServices

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[<assembly: AssemblyTitle("sdpeval")>]
[<assembly: AssemblyDescription("Evaluate WSUS Software Distribution Package applicability rules as defined in WSUS schema reference https://docs.microsoft.com/en-us/previous-versions/windows/desktop/bb972752(v=vs.85)")>]
[<assembly: AssemblyConfiguration("")>]
[<assembly: AssemblyCompany("github/trondr")>]
[<assembly: AssemblyProduct("sdpeval")>]
[<assembly: AssemblyCopyright("Copyright © github/trondr 2019")>]
[<assembly: AssemblyTrademark("")>]
[<assembly: AssemblyCulture("")>]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[<assembly: ComVisible(false)>]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[<assembly: Guid("59f9fa7c-b4a9-422c-8c52-958bbd1c6688")>]

// Version information for an assembly consists of the following four values:
//
//       Major Version
//       Minor Version
//       Build Number
//       Revision
//
// You can specify all the values or you can default the Build and Revision Numbers
// by using the '*' as shown below:
// [<assembly: AssemblyVersion("1.0.*")>]
[<assembly: AssemblyVersion("1.0.0.0")>]
[<assembly: AssemblyFileVersion("1.0.0.0")>]

[<assembly: InternalsVisibleTo("spdeval.tests")>]

do
    ()