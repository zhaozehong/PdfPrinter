using System;
using System.Runtime.InteropServices;
using System.Text;

namespace OS
{
  public static class Win32
  {
    #region Delegates
    public delegate bool EnumWindowsCallBack(IntPtr hWnd, int lParam);
    #endregion

    #region USER32
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr GetParent(IntPtr hWnd);
    [DllImport("User32.Dll")]
    public static extern int GetDlgCtrlID(IntPtr hWndCtl);
    [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    public static extern int MapWindowPoints(IntPtr hWnd, IntPtr hWndTo, ref POINT pt, int cPoints);
    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool GetWindowInfo(IntPtr hwnd, out WINDOWINFO pwi);
    [DllImport("User32.Dll")]
    public static extern void GetWindowText(IntPtr hWnd, StringBuilder param, int length);
    [DllImport("User32.Dll")]
    public static extern void GetClassName(IntPtr hWnd, StringBuilder param, int length);
    [DllImport("user32.Dll")]
    public static extern bool EnumChildWindows(IntPtr hWndParent, EnumWindowsCallBack lpEnumFunc, int lParam);
    [DllImport("user32.Dll")]
    public static extern bool EnumWindows(EnumWindowsCallBack lpEnumFunc, int lParam);
    [DllImport("user32.dll")]
    public static extern bool EnableWindow(IntPtr hWnd, bool bEnable);
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetForegroundWindow(IntPtr hWnd);
    [DllImport("User32.dll", CharSet = CharSet.Auto)]
    public static extern bool ReleaseCapture();
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr SetCapture(IntPtr hWnd);
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr ChildWindowFromPointEx(IntPtr hParent, POINT pt, ChildFromPointFlags flags);
    [DllImport("user32.dll", EntryPoint = "FindWindowExA", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
    [DllImport("user32.dll")]
    public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern int PostMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern int SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, StringBuilder param);
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, char[] chars);
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr BeginDeferWindowPos(int nNumWindows);
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr DeferWindowPos(IntPtr hWinPosInfo, IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int Width, int Height, SetWindowPosFlags flags);
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern bool EndDeferWindowPos(IntPtr hWinPosInfo);
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int Width, int Height, SetWindowPosFlags flags);
    [DllImport("user32.dll")]
    public static extern bool GetWindowRect(IntPtr hwnd, ref RECT rect);
    [DllImport("user32.dll")]
    public static extern bool GetClientRect(IntPtr hwnd, ref RECT rect);
    [DllImport("user32")]
    public static extern int RegisterWindowMessage(string message);

    public delegate int HookProc(int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern bool UnhookWindowsHookEx(int idHook);
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern int CallNextHookEx(int idHook, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr GetActiveWindow();
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr SetActiveWindow(IntPtr hwnd);
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern bool IsWindowEnabled(IntPtr hwnd);
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern bool BringWindowToTop(IntPtr hwnd);

    #endregion

    #region const
    public const int HWND_BROADCAST = 0xffff;
    public const int WM_DEVICE_CHANGE = 0x219;

    #endregion

    // Set default printer
    [DllImport("winspool.drv")]
    public static extern Boolean SetDefaultPrinter(String Name);
  }
}
