using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Zehong.CSharp.Solution.HelperLib
{
  public static class FrameworkElementExtensions
  {
    public static string SafeGetName(this FrameworkElement reference)
    {
      if (reference.CheckAccess())
        return reference.Name;

      string ret = default(string);
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.Name; }));
      return ret;
    }

    public static void SafeSetName(this FrameworkElement reference, string value)
    {
      if (reference.CheckAccess())
      {
        reference.Name = value;
        return;
      }

      reference.Dispatcher.Invoke(new Action(delegate { reference.Name = value; }));
    }

    public static double SafeGetWidth(this FrameworkElement reference)
    {
      if (reference.CheckAccess())
        return reference.Width;

      double ret = 0;
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.Width; }));
      return ret;
    }

    public static double SafeGetHeight(this FrameworkElement reference)
    {
      if (reference.CheckAccess())
        return reference.Height;

      double ret = 0;
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.Height; }));
      return ret;
    }

    public static double SafeGetMinWidth(this FrameworkElement reference)
    {
      if (reference.CheckAccess())
        return reference.MinWidth;

      double ret = 0;
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.MinWidth; }));
      return ret;
    }

    public static double SafeGetMinHeight(this FrameworkElement reference)
    {
      if (reference.CheckAccess())
        return reference.MinHeight;

      double ret = 0;
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.MinHeight; }));
      return ret;
    }

    public static void SafeSetWidth(this FrameworkElement reference, double value)
    {
      if (reference.CheckAccess())
      {
        reference.Width = value;
        return;
      }

      reference.Dispatcher.Invoke(new Action(delegate { reference.Width = value; }));
    }

    public static void SafeSetHeight(this FrameworkElement reference, double value)
    {
      if (reference.CheckAccess())
      {
        reference.Height = value;
        return;
      }

      reference.Dispatcher.Invoke(new Action(delegate { reference.Height = value; }));
    }

    public static System.Windows.Size SafeGetSize(this FrameworkElement reference)
    {
      if (reference.CheckAccess())
      {
        var width = double.IsNaN(reference.Width) ? reference.ActualWidth : reference.Width;
        var height = double.IsNaN(reference.Height) ? reference.ActualHeight : reference.Height;

        return new System.Windows.Size(width, height);
      }

      var ret = System.Windows.Size.Empty;
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.SafeGetSize(); }));
      return ret;
    }

    public static void SafeSetSize(this FrameworkElement reference, System.Windows.Size value)
    {
      if (reference.CheckAccess())
      {
        reference.Width = value.Width;
        reference.Height = value.Height;
        return;
      }

      reference.Dispatcher.Invoke(new Action(() => reference.SafeSetSize(value)));
    }

    public static void SafeSetBinding(this FrameworkElement reference, DependencyProperty dp, BindingBase binding)
    {
      if (reference.CheckAccess())
      {
        reference.SetBinding(dp, binding);
        return;
      }

      reference.Dispatcher.Invoke(new Action(() => reference.SetBinding(dp, binding)));

    }

    public static Rect SafeGetCanvasBounds(this FrameworkElement reference)
    {
      if (reference.CheckAccess())
      {
        var left = (double)reference.GetValue(Canvas.LeftProperty);
        var top = (double)reference.GetValue(Canvas.TopProperty);
        var width = double.IsNaN(reference.Width) ? reference.ActualWidth : reference.Width;
        var height = double.IsNaN(reference.Height) ? reference.ActualHeight : reference.Height;

        return new Rect(left, top, width, height);
      }

      Rect ret = Rect.Empty;
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.SafeGetCanvasBounds(); }));
      return ret;
    }

    public static void SafeSetCanvasBounds(this FrameworkElement reference, Rect value)
    {
      if (reference.CheckAccess())
      {
        reference.SetValue(Canvas.LeftProperty, value.X);
        reference.SetValue(Canvas.TopProperty, value.Y);
        reference.Width = value.Width;
        reference.Height = value.Height;
        return;
      }

      reference.Dispatcher.Invoke(new Action(() => reference.SafeSetCanvasBounds(value)));
    }

    public static double SafeGetActualWidth(this FrameworkElement reference)
    {
      if (reference.CheckAccess())
        return reference.ActualWidth;

      double ret = 0;
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.ActualWidth; }));
      return ret;
    }

    public static double SafeGetActualHeight(this FrameworkElement reference)
    {
      if (reference.CheckAccess())
        return reference.ActualHeight;

      double ret = 0;
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.ActualHeight; }));
      return ret;
    }

    public static object SafeGetToolTip(this FrameworkElement reference)
    {
      if (reference.CheckAccess())
        return reference.ToolTip;

      object ret = 0;
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.ToolTip; }));
      return ret;
    }

    public static void SafeSetToolTip(this FrameworkElement reference, object value)
    {
      if (reference.CheckAccess())
      {
        reference.ToolTip = value;
        return;
      }

      reference.Dispatcher.Invoke(new Action(delegate { reference.ToolTip = value; }));
    }

    public static object SafeGetTag(this FrameworkElement reference)
    {
      if (reference.CheckAccess())
        return reference.Tag;

      object ret = 0;
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.Tag; }));
      return ret;
    }

    public static void SafeSetTag(this FrameworkElement reference, object value)
    {
      if (reference.CheckAccess())
      {
        reference.Tag = value;
        return;
      }

      reference.Dispatcher.Invoke(new Action(delegate { reference.Tag = value; }));
    }

    public static object SafeGetDataContext(this FrameworkElement reference)
    {
      if (reference.CheckAccess())
        return reference.DataContext;

      object ret = 0;
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.DataContext; }));
      return ret;
    }

    public static void SafeSetDataContext(this FrameworkElement reference, object value)
    {
      if (reference.CheckAccess())
      {
        reference.DataContext = value;
        return;
      }

      reference.Dispatcher.Invoke(new Action(delegate { reference.DataContext = value; }));
    }

    public static bool SafeGetIsLoaded(this FrameworkElement reference)
    {
      if (reference.CheckAccess())
        return reference.IsLoaded;

      bool ret = default(bool);
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.IsLoaded; }));
      return ret;
    }

    public static Transform SafeGetLayoutTransform(this FrameworkElement reference)
    {
      if (reference.CheckAccess())
        return reference.LayoutTransform;

      Transform ret = default(Transform);
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.LayoutTransform; }));
      return ret;
    }

    public static void SafeSetLayoutTransform(this FrameworkElement reference, Transform value)
    {
      if (reference.CheckAccess())
      {
        reference.LayoutTransform = value;
        return;
      }

      reference.Dispatcher.Invoke(new Action(delegate { reference.LayoutTransform = value; }));
    }

    public static void SafeForceUpdateLayout(this FrameworkElement refrence)
    {
      if (!refrence.CheckAccess())
      {
        refrence.Dispatcher.Invoke(new Action(refrence.SafeForceUpdateLayout));
        return;
      }

      if (refrence.ActualWidth.IsZero() || refrence.ActualHeight.IsZero())
      {
        var availableSize = new System.Windows.Size(refrence.Width, refrence.Height);
        if (availableSize.IsNaN())
          availableSize = new System.Windows.Size(1000, 1000);
        refrence.Measure(availableSize);
        refrence.Arrange(new Rect(PointExtension.ZeroPoint, availableSize));
        refrence.UpdateLayout();
      }
    }

    public static void SafeSaveIntoPng(this FrameworkElement reference, string filename, double zoom = 1)
    {
      SafeSaveIntoPng(reference, filename, RectExtension.NaNRect, zoom);
    }

    public static void SafeSaveIntoPng(this FrameworkElement reference, string filename, Rect sourceRect, double zoom = 1)
    {
      try
      {
        if (!reference.CheckAccess())
        {
          reference.Dispatcher.Invoke(new Action(delegate { reference.SafeSaveIntoPng(filename, sourceRect); }));
          return;
        }

        var bs = reference.SafeGetBitmapSource(sourceRect, zoom);

        var enc = new PngBitmapEncoder();
        enc.Frames.Add(BitmapFrame.Create(bs));

        using (var stm = System.IO.File.Create(filename))
        {
          enc.Save(stm);
        }
      }
      catch (System.Exception ex)
      {
        ExceptionHandler.ThrowException(ex);
      }
    }
    public static BitmapSource SafeGetBitmapSource(this FrameworkElement reference, double zoom = 1)
    {
      return SafeGetBitmapSource(reference, RectExtension.NaNRect, zoom);
    }
    public static BitmapSource SafeGetBitmapSource(this FrameworkElement reference, Rect sourceRect, double zoom = 1)
    {
      try
      {
        if (!reference.CheckAccess())
        {
          BitmapSource ret = null;
          reference.Dispatcher.Invoke(new Action(delegate { ret = reference.SafeGetBitmapSource(sourceRect); }));
          return ret;
        }

        var width = double.IsNaN(reference.Width) ? reference.ActualWidth : reference.Width;
        var height = double.IsNaN(reference.Height) ? reference.ActualHeight : reference.Height;
        var dpiX = 96.0;
        var dpiY = 96.0;

        var rtb = new RenderTargetBitmap(
          (int)(width * zoom), //width 
          (int)(height * zoom), //height 
          dpiX * zoom, //dpi x 
          dpiY * zoom, //dpi y 
          PixelFormats.Pbgra32 // pixelformat 
          );
        rtb.Render(reference);

        BitmapSource bs = sourceRect.IsEmpty || sourceRect.IsNaN()
                            ? (BitmapSource)rtb
                            : new CroppedBitmap(rtb, sourceRect.ToInt32Rect());

        return bs;
      }
      catch (System.Exception ex)
      {
        ExceptionHandler.ThrowException(ex);
        return null;
      }
    }

    public static void SafeBeginInit(this FrameworkElement reference)
    {
      if (reference.CheckAccess())
        reference.BeginInit();
      else
        reference.Dispatcher.Invoke(new Action(delegate { reference.BeginInit(); }));

    }

    public static void SafeEndInit(this FrameworkElement reference)
    {
      if (reference.CheckAccess())
        reference.EndInit();
      else
        reference.Dispatcher.Invoke(new Action(delegate { reference.EndInit(); }));
    }

    public static void SafeSetDocument(this DocumentReference reference, FixedDocument doc)
    {
      if (reference.CheckAccess())
        reference.SetDocument(doc);
      else
        reference.Dispatcher.Invoke(new Action(delegate { reference.SetDocument(doc); }));
    }
  }
}
