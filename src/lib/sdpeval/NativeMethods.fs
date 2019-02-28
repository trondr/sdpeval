namespace sdpeval

module NativeMethods =
    open System
    open System.Runtime.InteropServices    
    open System.Text

    [<DllImport("shell32.dll", CharSet = CharSet.Auto, BestFitMapping = false)>]    
    extern int private SHGetFolderPath (IntPtr hwndOwner, int nFolder, IntPtr hToken, int dwFlags, [<Out>] StringBuilder lpszPath)

    /// <summary>
    /// Constant Special Item Id List
    /// </summary>
    [<System.FlagsAttribute>]
    type Csidl =
        // .NET Framework 4.0 and newer - all versions of windows ||| \val mutable\sdk\inc\shlobj.h
        |FlagCreate = 0x8000 // force folder creation in SHGetFolderPath
        |FlagDontVerify = 0x4000 // return an unverified folder path
        |Admintools = 0x0030 // <user name>\Start Menu\Programs\Administrative Tools
        |CdburnArea = 0x003b // USERPROFILE\Local Settings\Application Data\Microsoft\CD Burning
        |CommonAdmintools = 0x002f // All Users\Start Menu\Programs\Administrative Tools
        |CommonDocuments = 0x002e // All Users\Documents
        |CommonMusic = 0x0035 // All Users\My Music
        |CommonOemLinks = 0x003a // Links to All Users OEM specific apps
        |CommonPictures = 0x0036 // All Users\My Pictures
        |CommonStartmenu = 0x0016 // All Users\Start Menu
        |CommonPrograms = 0X0017 // All Users\Start Menu\Programs
        |CommonStartup = 0x0018 // All Users\Startup
        |CommonDesktopdirectory = 0x0019 // All Users\Desktop
        |CommonTemplates = 0x002d // All Users\Templates
        |CommonVideo = 0x0037 // All Users\My Video
        |Fonts = 0x0014 // windows\fonts
        |Myvideo = 0x000e // "My Videos" folder
        |Nethood = 0x0013 // %APPDATA%\Microsoft\Windows\Network Shortcuts
        |Printhood = 0x001b // %APPDATA%\Microsoft\Windows\Printer Shortcuts
        |Profile = 0x0028 // %USERPROFILE% (%SystemDrive%\Users\%USERNAME%)
        |ProgramFilesCommonx86 = 0x002c // x86 Program Files\Common on RISC
        |ProgramFilesx86 = 0x002a // x86 C:\Program Files on RISC
        |Resources = 0x0038 // %windir%\Resources
        |ResourcesLocalized = 0x0039 // %windir%\resources\0409 (code page)
        |Systemx86 = 0x0029 // %windir%\system32
        |Windows = 0x0024 // GetWindowsDirectory()
        // .NET Framework 3.5 and earlier - all versions of windows
        |Appdata = 0x001a
        |CommonAppdata = 0x0023
        |LocalAppdata = 0x001c
        |Cookies = 0x0021
        |Favorites = 0x0006
        |History = 0x0022
        |InternetCache = 0x0020
        |Programs = 0x0002
        |Recent = 0x0008
        |Sendto = 0x0009
        |Startmenu = 0x000b
        |Startup = 0x0007
        |System = 0x0025
        |Templates = 0x0015
        |Desktopdirectory = 0x0010
        |Personal = 0x0005
        |ProgramFiles = 0x0026
        |ProgramFilesCommon = 0x002b
        |Desktop = 0x0000
        |Drives = 0x0011
        |Mymusic = 0x000d
        |Mypictures = 0x0027

    let getFolderPath (csidlFolder:Csidl) =
        let sb = new StringBuilder(260);
        let returnCode = SHGetFolderPath(IntPtr.Zero, int csidlFolder, IntPtr.Zero, 0, sb);
        if (returnCode = -2146233031) then raise (new PlatformNotSupportedException());
        sb.ToString()
    
    [<StructLayout(LayoutKind.Sequential)>]
    type SystemInfo =
        struct        
            val mutable wProcessorArchitecture: UInt16
            val mutable wReserved: UInt16
            val mutable dwPageSize: Int32
            val mutable lpMinimumApplicationAddress: IntPtr
            val mutable lpMaximumApplicationAddress: IntPtr
            val mutable dwActiveProcessorMask: IntPtr
            val mutable dwNumberOfProcessors: Int32
            val mutable dwProcessorType: Int32
            val mutable dwAllocationGranularity: Int32
            val mutable wProcessorLevel: Int16
            val mutable wProcessorRevision: Int16
        end

    [<DllImport("kernel32.dll", SetLastError = true)>]
    extern void private GetSystemInfo([<Out>] SystemInfo& lpSystemInfo)

    [<DllImport("kernel32.dll", SetLastError = true)>]
    extern void private GetNativeSystemInfo([<Out>] SystemInfo& lpSystemInfo)

    let getSystemInfo () =
            let mutable systemInfo = Unchecked.defaultof<SystemInfo>
            GetSystemInfo (&systemInfo)
            systemInfo

    let getNativeSystemInfo () =        
        let mutable systemInfo = Unchecked.defaultof<SystemInfo>
        GetNativeSystemInfo(&systemInfo);
        systemInfo
    
    [<System.FlagsAttribute>]
    type  ProductType =
        |DomainController = 0x00000002us
        |Server = 0x00000003us
        |Workstation = 0x00000001us
     
     
    [<System.FlagsAttribute>]
    type SuiteMask =
        |SmallBusiness = 0x00000001us
        |Enterprise = 0x00000002us//Windows Server 2008 Enterprise, Windows Server 2003, Enterprise Edition, or Windows 2000 Advanced Server is installed. Refer to the Remarks section for more information about this bit flag.
        |Backoffice = 0x00000004us//Microsoft BackOffice components are installed.
        |Communications = 0x00000008us
        |Terminal = 0x00000010us//Terminal Services is installed. This value is always set.If VER_SUITE_TERMINAL is set but VER_SUITE_SINGLEUSERTS is not set, the system is running in application server mode.
        |SmallBusinessRestricted = 0x00000020us//Microsoft Small Business Server is installed with the restrictive client license in force. Refer to the Remarks section for more information about this bit flag.
        |EmbeddedNt = 0x00000040us //Windows XP Embedded is installed.
        |DataCenter = 0x00000080us//Windows Server 2008 Datacenter, Windows Server 2003, Datacenter Edition, or Windows 2000 Datacenter Server is installed.
        |SingleUserTs = 0x00000100us//Remote Desktop is supported, but only one interactive session is supported. This value is set unless the system is running in application server mode.
        |Personal = 0x00000200us//Windows Vista Home Premium, Windows Vista Home Basic, or Windows XP Home Edition is installed.
        |Blade = 0x00000400us//Windows Server 2003, Web Edition is installed.
        |StorageServer = 0x00002000us//Windows Storage Server 2003 R2 or Windows Storage Server 2003is installed.
        |ComputeServer = 0x00004000us//Windows Server 2003, Compute Cluster Edition is installed.
        |WhServer = 0x00008000us //Windows Home Server is installed.                    
    
    [<StructLayout(LayoutKind.Sequential)>]
    type OsVersionInfoEx =
        struct
            val mutable dwOSVersionInfoSize:UInt32
            val mutable dwMajorVersion:UInt32
            val mutable dwMinorVersion:UInt32
            val mutable dwBuildNumber:UInt32
            val mutable dwPlatformId:UInt32
            [<MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)>]
            val mutable szCSDVersion:string
            val mutable wServicePackMajor:UInt16
            val mutable wServicePackMinor:UInt16
            val mutable wSuiteMask:UInt16
            val mutable wProductType:byte
            val mutable wReserved:byte
        end
    
    [<DllImport("kernel32.dll", SetLastError = true)>]
    extern int private GetVersionEx([<MarshalAs(UnmanagedType.Struct)>]OsVersionInfoEx& lpVersionInfo);

    [<DllImport("ntdll.dll", SetLastError = true)>]
    extern int private RtlGetVersion([<MarshalAs(UnmanagedType.Struct)>]OsVersionInfoEx& lpVersionInfo);

    let getOsVersion () =
            let mutable osVersionInfoEx = Unchecked.defaultof<OsVersionInfoEx>
            osVersionInfoEx.dwOSVersionInfoSize <- uint32 (Marshal.SizeOf(typeof<OsVersionInfoEx>))
            GetVersionEx(&osVersionInfoEx) |> ignore
            RtlGetVersion(&osVersionInfoEx) |> ignore            
            osVersionInfoEx;
