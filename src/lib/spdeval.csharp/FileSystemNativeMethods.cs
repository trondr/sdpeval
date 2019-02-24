using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;

namespace spdeval.csharp
{
    public static class FileSystemNativeMethods
    {
        [DllImport("shell32.dll", CharSet = CharSet.Auto, BestFitMapping = false)]
        [ResourceExposure(ResourceScope.Machine)]
        private static extern int SHGetFolderPath(IntPtr hwndOwner, int nFolder, IntPtr hToken, int dwFlags, [Out]StringBuilder lpszPath);

        public static string GetFolderPath(int folder)
        {
            var sb = new StringBuilder(260);
            var result = SHGetFolderPath(IntPtr.Zero, folder, IntPtr.Zero, 0, sb);
            if (result == -2146233031) throw new PlatformNotSupportedException();
            return sb.ToString();
        }
    }

    /// <summary>
    /// Constant Special Item Id List
    /// </summary>
    public static class Csidl
    {
        // .NET Framework 4.0 and newer - all versions of windows ||| \public\sdk\inc\shlobj.h
        public const int CsidlFlagCreate = 0x8000; // force folder creation in SHGetFolderPath
        public const int CsidlFlagDontVerify = 0x4000; // return an unverified folder path
        public const int CsidlAdmintools = 0x0030; // <user name>\Start Menu\Programs\Administrative Tools
        public const int CsidlCdburnArea = 0x003b; // USERPROFILE\Local Settings\Application Data\Microsoft\CD Burning
        public const int CsidlCommonAdmintools = 0x002f; // All Users\Start Menu\Programs\Administrative Tools
        public const int CsidlCommonDocuments = 0x002e; // All Users\Documents
        public const int CsidlCommonMusic = 0x0035; // All Users\My Music
        public const int CsidlCommonOemLinks = 0x003a; // Links to All Users OEM specific apps
        public const int CsidlCommonPictures = 0x0036; // All Users\My Pictures
        public const int CsidlCommonStartmenu = 0x0016; // All Users\Start Menu
        public const int CsidlCommonPrograms = 0X0017; // All Users\Start Menu\Programs
        public const int CsidlCommonStartup = 0x0018; // All Users\Startup
        public const int CsidlCommonDesktopdirectory = 0x0019; // All Users\Desktop
        public const int CsidlCommonTemplates = 0x002d; // All Users\Templates
        public const int CsidlCommonVideo = 0x0037; // All Users\My Video
        public const int CsidlFonts = 0x0014; // windows\fonts
        public const int CsidlMyvideo = 0x000e; // "My Videos" folder
        public const int CsidlNethood = 0x0013; // %APPDATA%\Microsoft\Windows\Network Shortcuts
        public const int CsidlPrinthood = 0x001b; // %APPDATA%\Microsoft\Windows\Printer Shortcuts
        public const int CsidlProfile = 0x0028; // %USERPROFILE% (%SystemDrive%\Users\%USERNAME%)
        public const int CsidlProgramFilesCommonx86 = 0x002c; // x86 Program Files\Common on RISC
        public const int CsidlProgramFilesx86 = 0x002a; // x86 C:\Program Files on RISC
        public const int CsidlResources = 0x0038; // %windir%\Resources
        public const int CsidlResourcesLocalized = 0x0039; // %windir%\resources\0409 (code page)
        public const int CsidlSystemx86 = 0x0029; // %windir%\system32
        public const int CsidlWindows = 0x0024; // GetWindowsDirectory()
        // .NET Framework 3.5 and earlier - all versions of windows
        public const int CsidlAppdata = 0x001a;
        public const int CsidlCommonAppdata = 0x0023;
        public const int CsidlLocalAppdata = 0x001c;
        public const int CsidlCookies = 0x0021;
        public const int CsidlFavorites = 0x0006;
        public const int CsidlHistory = 0x0022;
        public const int CsidlInternetCache = 0x0020;
        public const int CsidlPrograms = 0x0002;
        public const int CsidlRecent = 0x0008;
        public const int CsidlSendto = 0x0009;
        public const int CsidlStartmenu = 0x000b;
        public const int CsidlStartup = 0x0007;
        public const int CsidlSystem = 0x0025;
        public const int CsidlTemplates = 0x0015;
        public const int CsidlDesktopdirectory = 0x0010;
        public const int CsidlPersonal = 0x0005;
        public const int CsidlProgramFiles = 0x0026;
        public const int CsidlProgramFilesCommon = 0x002b;
        public const int CsidlDesktop = 0x0000;
        public const int CsidlDrives = 0x0011;
        public const int CsidlMymusic = 0x000d;
        public const int CsidlMypictures = 0x0027;
    }

}
