using System.Runtime.InteropServices;

namespace Squrl.WindowsHost;

public static class NativeTrayMenu
{
    private const uint TpmRightButton = 0x0002;
    private const uint TpmReturnCmd = 0x0100;
    private const uint TpmNoNotify = 0x0080;

    private const uint MfString = 0x0000;
    private const uint MfSeparator = 0x0800;
    private const uint MfByCommand = 0x0000;
    private const uint MfGrayed = 0x0001;

    [DllImport("user32.dll")]
    private static extern IntPtr CreatePopupMenu();

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern bool AppendMenu(
        IntPtr hMenu,
        uint flags,
        UIntPtr id,
        string? text);
    
    [DllImport("user32.dll")]
    private static extern bool EnableMenuItem(
        IntPtr hMenu,
        uint id,
        uint enable);

    [DllImport("user32.dll")]
    private static extern uint TrackPopupMenu(
        IntPtr hMenu,
        uint flags,
        int x,
        int y,
        int reserved,
        IntPtr hwnd,
        IntPtr rect);

    [DllImport("user32.dll")]
    private static extern bool DestroyMenu(IntPtr hMenu);

    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool PostMessage(
        IntPtr hWnd,
        uint msg,
        IntPtr wParam,
        IntPtr lParam);

    public static void Show(
        IntPtr owner,
        params Item[] items)
    {
        IntPtr menu = CreatePopupMenu();

        if (menu == IntPtr.Zero)
        {
            return;
        }

        try
        {
            uint id = 1;

            foreach (Item item in items)
            {
                if (item.IsSeparator)
                {
                    AppendMenu(
                        menu,
                        MfSeparator,
                        UIntPtr.Zero,
                        null);

                    continue;
                }
                
                uint itemId = id++;

                AppendMenu(
                    menu,
                    MfString,
                    itemId,
                    item.Text);
                
                if (!item.Enabled)
                {
                    EnableMenuItem(
                        menu,
                        itemId,
                        MfByCommand | MfGrayed);
                }
            }

            GetCursorPos(out Point point);

            SetForegroundWindow(owner);

            uint command = TrackPopupMenu(
                menu,
                TpmRightButton |
                TpmReturnCmd |
                TpmNoNotify,
                point.X,
                point.Y,
                0,
                owner,
                IntPtr.Zero);

            PostMessage(
                owner,
                0,
                IntPtr.Zero,
                IntPtr.Zero);

            id = 1;

            foreach (Item item in items)
            {
                if (item.IsSeparator)
                {
                    continue;
                }

                uint itemId = id++;

                if (command == itemId)
                {
                    if (item.Enabled)
                    {
                        item.Action?.Invoke();
                    }

                    break;
                }
            }
        }
        finally
        {
            DestroyMenu(menu);
        }
    }
    
    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out Point point);

    public sealed record Item(
        string Text,
        Action? Action = null,
        bool Enabled = true,
        bool IsSeparator = false);

    [StructLayout(LayoutKind.Sequential)]
    private struct Point(int x, int y)
    {
        public readonly int X = x;
        public readonly int Y = y;
    }
}