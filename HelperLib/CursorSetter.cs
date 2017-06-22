using System;
using System.Windows.Input;
using System.Windows.Threading;

namespace Zehong.CSharp.Solution.HelperLib
{
  public class CursorSetter : IDisposable
  {
    public CursorSetter(Cursor newCursor, Dispatcher dispatcher = null)
    {
      this.Dispatcher = dispatcher;

      if (this.Dispatcher != null)
      {
        this.Dispatcher.Invoke((Action)delegate
        {
          this.PreviousCursor = Mouse.OverrideCursor;
          Mouse.OverrideCursor = newCursor;
        });
      }
      else
      {
        this.PreviousCursor = Mouse.OverrideCursor;
        Mouse.OverrideCursor = newCursor;
      }
    }

    #region IDisposable Members

    public void Dispose()
    {
      if (this.Dispatcher != null)
      {
        this.Dispatcher.Invoke((Action)delegate
        {
          Mouse.OverrideCursor = this.PreviousCursor;
        });
      }
      else
      {
        Mouse.OverrideCursor = this.PreviousCursor;
      }
    }

    #endregion

    public Cursor PreviousCursor { get; private set; }
    public Dispatcher Dispatcher { get; private set; }
  }
}
