using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Zehong.CSharp.Solution.HelperLib
{
  public sealed class WindowsManager : DispatcherObject
  {
    private static WindowsManager _current = null;
    public static WindowsManager Current
    {
      get
      {
        if (_current == null)
        {
          _current = new WindowsManager();
          if (Application.Current != null && Application.Current.MainWindow != null)
            _current.AddDialog(Application.Current.MainWindow);
        }
        return _current;
      }
    }

    private void _idleTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
      try
      {
        this.Dispatcher.Invoke((Action)delegate
        {
          DateTime now = DateTime.Now;
          double nIdleSeconds = (now - this._timeLastUserInput).TotalSeconds;
          foreach (var item in this._idleHandlers)
          {
            if (Monitor.TryEnter(item))
            {
              if (item.IsEnabled && item.MaxIdleInterval <= nIdleSeconds)
              {
                double nSecondInterval = (now - item.DateTimeLastExccute).TotalSeconds;
                if (item.MaxIdleInterval <= nSecondInterval)
                {
                  item.DealWithIdle(nIdleSeconds);

                  item.DateTimeLastExccute = now;
                }
              }
              Monitor.Exit(item);
            }
          }
        });
      }
      catch (System.Exception ex)
      {
        ExceptionHandler.ThrowException(ex);
      }
    }
    public void StartIdle()
    {
      if (_idleTimer == null)
      {
        _timeLastUserInput = DateTime.Now;
        _idleTimer = new System.Timers.Timer(1000);
        _idleTimer.Elapsed += _idleTimer_Elapsed;
        _idleTimer.Start();
      }
      if (Application.Current != null && Application.Current.MainWindow != null)
        this.AddDialog(Application.Current.MainWindow);
    }
    public void CloseAllDialog()
    {
      while (_applicationDialogs.Count > 0)
      {
        Window dlg = _applicationDialogs.LastOrDefault();
        if (dlg == null || ReferenceEquals(dlg, Application.Current.MainWindow))
          break;

        this.RemoveDialog(dlg);
        dlg.Close();
      }
    }

    public void AddDialog(Window dlg)
    {
      if (dlg == null || _applicationDialogs.Contains(dlg))
        return;

      _applicationDialogs.Add(dlg);
      Mouse.AddPreviewMouseMoveHandler(dlg, OnPreviewMouseMove);
      Keyboard.AddPreviewKeyDownHandler(dlg, OnPreviewKeyDown);

      dlg.Activated += OnWindowActivated;
      dlg.Deactivated += OnWindowDeactivated;
    }
    public void RemoveDialog(Window dlg)
    {
      dlg.Activated -= OnWindowActivated;
      dlg.Deactivated -= OnWindowDeactivated;

      Mouse.RemoveMouseMoveHandler(dlg, OnPreviewMouseMove);
      Keyboard.RemovePreviewKeyDownHandler(dlg, OnPreviewKeyDown);

      _applicationDialogs.Remove(dlg);
    }

    public void AddIdleHandler(IdleHandler handler)
    {
      if (handler == null || _idleHandlers.Contains(handler))
        return;

      _idleHandlers.Add(handler);
    }
    public void RemoveIdleHandler(IdleHandler handler)
    {
      _idleHandlers.Remove(handler);
    }

    private System.Timers.Timer _idleTimer = null;
    private readonly List<Window> _applicationDialogs = new List<Window>();
    private readonly List<IdleHandler> _idleHandlers = new List<IdleHandler>();
    private DateTime _timeLastUserInput;
    private void OnPreviewMouseMove(object sender, MouseEventArgs e)
    {
      Window dlg = sender as Window;
      if (dlg != null && dlg.IsActive)
        _timeLastUserInput = DateTime.Now;
    }
    private void OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
      _timeLastUserInput = DateTime.Now;
    }
    private void OnWindowActivated(object sender, EventArgs e)
    {
      var window = sender as Window;
      if (window != null)
        Debug.Print("OnWindowActivated : sender is \"{0}\"", window.Title);

      if (!Helper.IsBrowserApplication)
        return;

      foreach (var item in this._applicationDialogs)
      {
        if (object.ReferenceEquals(item, Application.Current.MainWindow))
          continue;
        item.Topmost = true;
        item.UpdateLayout();
      }
    }

    private void OnWindowDeactivated(object sender, EventArgs e)
    {
      var window = sender as Window;
      if (window != null)
        Debug.Print("OnWindowDeactivated : sender is \"{0}\"", window.Title);

      if (!Helper.IsBrowserApplication)
        return;

      if (this._applicationDialogs.Exists(p => p.IsActive))
        return;

      foreach (var item in this._applicationDialogs)
      {
        if (object.ReferenceEquals(item, Application.Current.MainWindow))
          continue;
        item.Topmost = false;
        item.UpdateLayout();
      }
    }
  }
  public abstract class IdleHandler : DispatcherObject
  {
    protected IdleHandler()
    {
      WindowsManager.Current.AddIdleHandler(this);
    }
    public void Start()
    {
      WindowsManager.Current.StartIdle();
    }
    public DateTime DateTimeLastExccute = DateTime.MinValue;
    public abstract void DealWithIdle(double nIdleSeconds);
    public abstract bool IsEnabled { get; }
    public abstract double MaxIdleInterval { get; }
  }
}
