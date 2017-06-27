using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace OS
{
  #region WINDOWINFO
  [StructLayout(LayoutKind.Sequential)]
  public struct WINDOWINFO
  {
    public UInt32 cbSize;
    public RECT rcWindow;
    public RECT rcClient;
    public UInt32 dwStyle;
    public UInt32 dwExStyle;
    public UInt32 dwWindowStatus;
    public UInt32 cxWindowBorders;
    public UInt32 cyWindowBorders;
    public UInt16 atomWindowType;
    public UInt16 wCreatorVersion;
  }
  #endregion

  #region POINT
  [StructLayout(LayoutKind.Sequential)]
  public struct POINT
  {
    public int x;
    public int y;

    #region Constructors
    public POINT(int x, int y)
    {
      this.x = x;
      this.y = y;
    }

    public POINT(Point point)
    {
      x = point.X;
      y = point.Y;
    }
    #endregion
  }
  #endregion

  #region RECT
  [StructLayout(LayoutKind.Sequential)]
  public struct RECT
  {
    public uint left;
    public uint top;
    public uint right;
    public uint bottom;

    #region Properties
    public POINT Location
    {
      get { return new POINT((int)left, (int)top); }
      set
      {
        right -= (left - (uint)value.x);
        bottom -= (bottom - (uint)value.y);
        left = (uint)value.x;
        top = (uint)value.y;
      }
    }

    public uint Width
    {
      get { return right - left; }
      set { right = left + value; }
    }

    public uint Height
    {
      get { return bottom - top; }
      set { bottom = top + value; }
    }
    #endregion

    #region Overrides
    public override string ToString()
    {
      return left + ":" + top + ":" + right + ":" + bottom;
    }
    #endregion
  }
  #endregion

  #region WINDOWPOS
  [StructLayout(LayoutKind.Sequential)]
  public struct WINDOWPOS
  {
    public IntPtr hwnd;
    public IntPtr hwndAfter;
    public int x;
    public int y;
    public int cx;
    public int cy;
    public uint flags;

    #region Overrides
    public override string ToString()
    {
      return x + ":" + y + ":" + cx + ":" + cy + ":" + ((SWP_Flags)flags).ToString();
    }
    #endregion
  }
  #endregion

  #region NCCALCSIZE_PARAMS
  [StructLayout(LayoutKind.Sequential)]
  public struct NCCALCSIZE_PARAMS
  {
    public RECT rgrc1;
    public RECT rgrc2;
    public RECT rgrc3;
    public IntPtr lppos;
  }
  #endregion

  #region NMHDR
  [StructLayout(LayoutKind.Sequential)]
  public struct NMHDR
  {
    public IntPtr hwndFrom;
    public uint idFrom;
    public uint code;
  }
  #endregion

  #region OFNOTIFY
  [StructLayout(LayoutKind.Sequential)]
  public struct OFNOTIFY
  {
    public NMHDR hdr;
    public IntPtr OPENFILENAME;
    public IntPtr fileNameShareViolation;
  }
  #endregion

  [StructLayout(LayoutKind.Sequential)]
  public class MouseHookStruct
  {
    public POINT pt;
    public int hwnd;
    public int wHitTestCode;
    public int dwExtraInfo;
  }

  public enum MouseMessages
  {
    WM_CLOSE = 0x0010,
    WM_QUIT = 0x0012,
    WM_NCLBUTTONDOWN = 0x00A1,
    WM_NCLBUTTONUP = 0x00A2,
    WM_LBUTTONDOWN = 0x0201,
    WM_LBUTTONUP = 0x0202,
    WM_MOUSEMOVE = 0x0200,
    WM_MOUSEWHEEL = 0x020A,
    WM_RBUTTONDOWN = 0x0204,
    WM_RBUTTONUP = 0x0205
  }
}
