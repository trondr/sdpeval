using System;
using System.Runtime.InteropServices;

namespace spdeval.csharp
{
    public static class NativeMethods
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct SystemInfo
        {
            public ushort wProcessorArchitecture;
            public ushort wReserved;
            public int dwPageSize;
            public IntPtr lpMinimumApplicationAddress;
            public IntPtr lpMaximumApplicationAddress;
            public IntPtr dwActiveProcessorMask;
            public int dwNumberOfProcessors;
            public int dwProcessorType;
            public int dwAllocationGranularity;
            public short wProcessorLevel;
            public short wProcessorRevision;
        }
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern void GetSystemInfo(ref SystemInfo lpSystemInfo);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern void GetNativeSystemInfo(out SystemInfo lpSystemInfo);

        public static SystemInfo GetSystemInfo()
        {
            SystemInfo systemInfo = default(SystemInfo);
            GetSystemInfo(ref systemInfo);
            return systemInfo;
        }

        public static SystemInfo GetNativeSystemInfo()
        {
            GetNativeSystemInfo(out var systemInfo);
            return systemInfo;
        }

        public struct OsVersionInfoEx
        {
            public uint dwOSVersionInfoSize;
            public uint dwMajorVersion;
            public uint dwMinorVersion;
            public uint dwBuildNumber;
            public uint dwPlatformId;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szCSDVersion;
            public ushort wServicePackMajor;
            public ushort wServicePackMinor;
            public ushort wSuiteMask;
            public byte wProductType;
            public byte wReserved;
        }

        [Flags]
        public enum SuiteMask : UInt16
        {
            SmallBusiness = 0x00000001,
            Enterprise = 0x00000002,//Windows Server 2008 Enterprise, Windows Server 2003, Enterprise Edition, or Windows 2000 Advanced Server is installed. Refer to the Remarks section for more information about this bit flag.
            Backoffice = 0x00000004,//Microsoft BackOffice components are installed.
            Communications = 0x00000008,
            Terminal = 0x00000010,//Terminal Services is installed. This value is always set.If VER_SUITE_TERMINAL is set but VER_SUITE_SINGLEUSERTS is not set, the system is running in application server mode.
            SmallBusinessRestricted = 0x00000020,//Microsoft Small Business Server is installed with the restrictive client license in force. Refer to the Remarks section for more information about this bit flag.
            EmbeddedNt = 0x00000040, //Windows XP Embedded is installed.
            DataCenter = 0x00000080,//Windows Server 2008 Datacenter, Windows Server 2003, Datacenter Edition, or Windows 2000 Datacenter Server is installed.
            SingleUserTs = 0x00000100,//Remote Desktop is supported, but only one interactive session is supported. This value is set unless the system is running in application server mode.
            Personal = 0x00000200,//Windows Vista Home Premium, Windows Vista Home Basic, or Windows XP Home Edition is installed.
            Blade = 0x00000400,//Windows Server 2003, Web Edition is installed.
            StorageServer = 0x00002000,//Windows Storage Server 2003 R2 or Windows Storage Server 2003is installed.
            ComputeServer = 0x00004000,//Windows Server 2003, Compute Cluster Edition is installed.
            WhServer = 0x00008000, //Windows Home Server is installed.            
        }

        [Flags]
        public enum ProductType : UInt16
        {
            DomainController = 0x00000002,
            Server = 0x00000003,
            Workstation = 0x00000001
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int GetVersionEx([MarshalAs(UnmanagedType.Struct)] ref OsVersionInfoEx lpVersionInfo);

        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern int RtlGetVersion([MarshalAs(UnmanagedType.Struct)] ref OsVersionInfoEx lpVersionInfo);

        public static OsVersionInfoEx GetOsVersion()
        {
            var osVersionInfoEx = default(OsVersionInfoEx);
            osVersionInfoEx.dwOSVersionInfoSize = (uint)Marshal.SizeOf((object)osVersionInfoEx);
            GetVersionEx(ref osVersionInfoEx);
            RtlGetVersion(ref osVersionInfoEx);            
            return osVersionInfoEx;
        }
    }
}
