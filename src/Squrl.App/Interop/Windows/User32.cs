using System.Runtime.InteropServices;

namespace Squrl.App.Interop.Windows;

internal static class User32
{
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    internal static extern int MessageBox(
        nint hWnd,
        string text,
        string caption,
        MessageBoxOptions type);
    
    [Flags]
    internal enum MessageBoxOptions : uint
    {
        Ok = 0x00000000,
        IconInformation = 0x00000040,
        IconWarning = 0x00000030,
        IconError = 0x00000010,
    }
}