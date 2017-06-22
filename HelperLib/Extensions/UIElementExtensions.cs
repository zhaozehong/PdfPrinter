using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Zehong.CSharp.Solution.HelperLib
{
  public static class UIElementExtensions
  {
    public static Visibility SafeGetVisibility(this UIElement uiElement)
    {
      if (uiElement.CheckAccess())
        return uiElement.Visibility;

      var ret = Visibility.Collapsed;
      uiElement.Dispatcher.Invoke(new Action(delegate { ret = uiElement.Visibility; }));
      return ret;
    }
    public static void SafeSetVisibility(this UIElement uiElement, Visibility value)
    {
      if (uiElement.CheckAccess())
      {
        uiElement.Visibility = value;
        return;
      }

      uiElement.Dispatcher.Invoke(new Action(delegate { uiElement.Visibility = value; }));
    }

    public static bool SafeGetIsHitTestVisible(this UIElement uiElement)
    {
      if (uiElement.CheckAccess())
        return uiElement.IsHitTestVisible;

      bool ret = default(bool);
      uiElement.Dispatcher.Invoke(new Action(delegate { ret = uiElement.IsHitTestVisible; }));
      return ret;
    }
    public static void SafeSetIsHitTestVisible(this UIElement uiElement, bool value)
    {
      if (uiElement.CheckAccess())
      {
        uiElement.IsHitTestVisible = value;
        return;
      }

      uiElement.Dispatcher.Invoke(new Action(delegate { uiElement.IsHitTestVisible = value; }));
    }

    public static Transform SafeGetRenderTransform(this UIElement reference)
    {
      if (reference.CheckAccess())
        return reference.RenderTransform;

      Transform ret = default(Transform);
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.RenderTransform; }));
      return ret;
    }
    public static void SafeSetRenderTransform(this UIElement reference, Transform value)
    {
      if (reference.CheckAccess())
      {
        reference.RenderTransform = value;
        return;
      }

      reference.Dispatcher.Invoke(new Action(delegate { reference.RenderTransform = value; }));
    }

    public static bool SafeFocus(this UIElement reference)
    {
      if (reference.CheckAccess())
      {
        return reference.Focus();
      }

      bool ret = false;
      reference.Dispatcher.Invoke(new Action(() => ret = reference.Focus()));
      return ret;
    }
    public static void SafeMeasure(this UIElement uiElement, System.Windows.Size availableSize)
    {
      if (uiElement.CheckAccess())
      {
        uiElement.Measure(availableSize);
        return;
      }

      uiElement.Dispatcher.Invoke(new Action(() => uiElement.Measure(availableSize)));
    }
    public static void SafeArrange(this UIElement uiElement, Rect finalRect)
    {
      if (uiElement.CheckAccess())
      {
        uiElement.Arrange(finalRect);
        return;
      }

      uiElement.Dispatcher.Invoke(new Action(() => uiElement.Arrange(finalRect)));
    }
    public static void SafeUpdateLayout(this UIElement uiElement)
    {
      if (uiElement.CheckAccess())
      {
        uiElement.UpdateLayout();
        return;
      }

      uiElement.Dispatcher.Invoke(new Action(uiElement.UpdateLayout));
    }
    public static void SafeInvalidateMeasure(this UIElement uiElement)
    {
      if (uiElement.CheckAccess())
      {
        uiElement.InvalidateMeasure();
        return;
      }

      uiElement.Dispatcher.Invoke(new Action(uiElement.InvalidateMeasure));
    }
    public static void SafeInvalidateArrange(this UIElement uiElement)
    {
      if (uiElement.CheckAccess())
      {
        uiElement.InvalidateArrange();
        return;
      }

      uiElement.Dispatcher.Invoke(new Action(uiElement.InvalidateArrange));
    }
    public static void SafeInvalidateArrange(this UIElement uiElement, DependencyProperty dp)
    {
      if (uiElement.CheckAccess())
      {
        uiElement.InvalidateProperty(dp);
        return;
      }

      uiElement.Dispatcher.Invoke(new Action(() => uiElement.InvalidateProperty(dp)));
    }
    public static bool SafeApplyTemplate(this FrameworkElement reference)
    {
      if (reference.CheckAccess())
      {
        return reference.ApplyTemplate();
      }

      bool ret = false;
      reference.Dispatcher.Invoke(new Action(() => ret = reference.ApplyTemplate()));
      return ret;
    }

    public static int SafeGetGridRow(this UIElement uiElement)
    {
      if (uiElement.CheckAccess())
        return (int)uiElement.GetValue(Grid.RowProperty);

      int ret = 0;
      uiElement.Dispatcher.Invoke(new Action(delegate { ret = (int)uiElement.GetValue(Grid.RowProperty); }));
      return ret;
    }
    public static int SafeGetGridColumn(this UIElement uiElement)
    {
      if (uiElement.CheckAccess())
        return (int)uiElement.GetValue(Grid.ColumnProperty);

      int ret = 0;
      uiElement.Dispatcher.Invoke(new Action(delegate { ret = (int)uiElement.GetValue(Grid.ColumnProperty); }));
      return ret;
    }
    public static int SafeGetGridRowSpan(this UIElement uiElement)
    {
      if (uiElement.CheckAccess())
        return (int)uiElement.GetValue(Grid.RowSpanProperty);

      int ret = 0;
      uiElement.Dispatcher.Invoke(new Action(delegate { ret = (int)uiElement.GetValue(Grid.RowSpanProperty); }));
      return ret;
    }
    public static int SafeGetGridColumnSpan(this UIElement uiElement)
    {
      if (uiElement.CheckAccess())
        return (int)uiElement.GetValue(Grid.ColumnSpanProperty);

      int ret = 0;
      uiElement.Dispatcher.Invoke(new Action(delegate { ret = (int)uiElement.GetValue(Grid.ColumnSpanProperty); }));
      return ret;
    }
    public static double SafeGetCanvasLeft(this UIElement uiElement)
    {
      if (uiElement.CheckAccess())
        return (double)uiElement.GetValue(Canvas.LeftProperty);

      double ret = 0;
      uiElement.Dispatcher.Invoke(new Action(delegate { ret = (double)uiElement.GetValue(Canvas.LeftProperty); }));
      return ret;
    }
    public static double SafeGetCanvasTop(this UIElement uiElement)
    {
      if (uiElement.CheckAccess())
        return (double)uiElement.GetValue(Canvas.TopProperty);

      double ret = 0;
      uiElement.Dispatcher.Invoke(new Action(delegate { ret = (double)uiElement.GetValue(Canvas.TopProperty); }));
      return ret;
    }
    public static double SafeGetCanvasRight(this UIElement uiElement)
    {
      if (uiElement is FrameworkElement)
        return uiElement.SafeGetCanvasLeft() + (uiElement as FrameworkElement).SafeGetWidth();

      if (uiElement.CheckAccess())
        return (double)uiElement.GetValue(Canvas.RightProperty);

      double ret = 0;
      uiElement.Dispatcher.Invoke(new Action(delegate { ret = (double)uiElement.GetValue(Canvas.RightProperty); }));
      return ret;
    }
    public static double SafeGetCanvasBottom(this UIElement uiElement)
    {
      if (uiElement is FrameworkElement)
        return uiElement.SafeGetCanvasTop() + (uiElement as FrameworkElement).SafeGetHeight();

      if (uiElement.CheckAccess())
        return (double)uiElement.GetValue(Canvas.BottomProperty);

      double ret = 0;
      uiElement.Dispatcher.Invoke(new Action(delegate { ret = (double)uiElement.GetValue(Canvas.BottomProperty); }));
      return ret;
    }
    public static int SafeGetZIndex(this UIElement uiElement)
    {
      if (uiElement.CheckAccess())
        return (int)uiElement.GetValue(Panel.ZIndexProperty);

      int ret = 0;
      uiElement.Dispatcher.Invoke(new Action(delegate { ret = (int)uiElement.GetValue(Panel.ZIndexProperty); }));
      return ret;
    }
    public static Rect SafeGetCanvasBounds(this UIElement uiElement)
    {
      if (uiElement.CheckAccess())
      {
        var left = (double)uiElement.GetValue(Canvas.LeftProperty);
        var top = (double)uiElement.GetValue(Canvas.TopProperty);
        var right = (double)uiElement.GetValue(Canvas.RightProperty);
        var bottom = (double)uiElement.GetValue(Canvas.BottomProperty);
        return new Rect(left, top, right - left, bottom - top);
      }

      Rect ret = Rect.Empty;
      uiElement.Dispatcher.Invoke(new Action(delegate { ret = uiElement.SafeGetCanvasBounds(); }));
      return ret;
    }

    public static void SafeSetGridRow(this UIElement uiElement, int value)
    {
      if (uiElement.CheckAccess())
      {
        uiElement.SetValue(Grid.RowProperty, value);
        return;
      }

      uiElement.Dispatcher.Invoke(new Action(() => uiElement.SetValue(Grid.RowProperty, value)));
    }
    public static void SafeSetGridColumn(this UIElement uiElement, int value)
    {
      if (uiElement.CheckAccess())
      {
        uiElement.SetValue(Grid.ColumnProperty, value);
        return;
      }

      uiElement.Dispatcher.Invoke(new Action(() => uiElement.SetValue(Grid.ColumnProperty, value)));
    }
    public static void SafeSetGridRowSpan(this UIElement uiElement, int value)
    {
      if (uiElement.CheckAccess())
      {
        uiElement.SetValue(Grid.RowSpanProperty, value);
        return;
      }

      uiElement.Dispatcher.Invoke(new Action(() => uiElement.SetValue(Grid.RowSpanProperty, value)));
    }
    public static void SafeSetGridColumnSpan(this UIElement uiElement, int value)
    {
      if (uiElement.CheckAccess())
      {
        uiElement.SetValue(Grid.ColumnSpanProperty, value);
        return;
      }

      uiElement.Dispatcher.Invoke(new Action(() => uiElement.SetValue(Grid.ColumnSpanProperty, value)));
    }
    public static void SafeSetCanvasLeft(this UIElement uiElement, double value)
    {
      if (uiElement.CheckAccess())
      {
        uiElement.SetValue(Canvas.LeftProperty, value);
        return;
      }

      uiElement.Dispatcher.Invoke(new Action(() => uiElement.SetValue(Canvas.LeftProperty, value)));
    }
    public static void SafeSetCanvasTop(this UIElement uiElement, double value)
    {
      if (uiElement.CheckAccess())
      {
        uiElement.SetValue(Canvas.TopProperty, value);
        return;
      }

      uiElement.Dispatcher.Invoke(new Action(() => uiElement.SetValue(Canvas.TopProperty, value)));
    }
    public static void SafeSetCanvasRight(this UIElement uiElement, double value)
    {
      if (uiElement is FrameworkElement)
      {
        (uiElement as FrameworkElement).SafeSetWidth(value - uiElement.SafeGetCanvasLeft());
        return;
      }

      if (uiElement.CheckAccess())
      {
        uiElement.SetValue(Canvas.RightProperty, value);
        return;
      }

      uiElement.Dispatcher.Invoke(new Action(() => uiElement.SetValue(Canvas.RightProperty, value)));
    }
    public static void SafeSetCanvasBottom(this UIElement uiElement, double value)
    {
      if (uiElement is FrameworkElement)
      {
        (uiElement as FrameworkElement).SafeSetHeight(value - uiElement.SafeGetCanvasTop());
        return;
      }

      if (uiElement.CheckAccess())
      {
        uiElement.SetValue(Canvas.BottomProperty, value);
        return;
      }

      uiElement.Dispatcher.Invoke(new Action(() => uiElement.SetValue(Canvas.BottomProperty, value)));
    }
    public static void SafeSetZIndex(this UIElement uiElement, int value)
    {
      if (uiElement.CheckAccess())
      {
        uiElement.SetValue(Panel.ZIndexProperty, value);
        return;
      }

      uiElement.Dispatcher.Invoke(new Action(() => uiElement.SetValue(Panel.ZIndexProperty, value)));
    }
  }
}
