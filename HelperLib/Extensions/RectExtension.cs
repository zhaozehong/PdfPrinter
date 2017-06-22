using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Zehong.CSharp.Solution.HelperLib
{
  public static class RectExtension
  {
    public static readonly Rect ZeroRect = new Rect(0, 0, 0, 0);
    public static readonly Rect NaNRect = new Rect(double.NaN, double.NaN, double.NaN, double.NaN);

    public static System.Windows.Point GetCenter(this Rect reference)
    {
      return new System.Windows.Point(reference.X + reference.Width / 2, reference.Y + reference.Height / 2);
    }
    public static double GetCenterX(this Rect reference)
    {
      return (reference.X + reference.Width / 2);
    }
    public static double GetCenterY(this Rect reference)
    {
      return (reference.Y + reference.Height / 2);
    }
    public static Rect Scale(this Rect reference, double scaleFactor)
    {
      return new Rect(reference.X * scaleFactor, reference.Y * scaleFactor, reference.Width * scaleFactor, reference.Height * scaleFactor);
    }
    public static Rect Scale(this Rect reference, System.Windows.Size scaleFactor)
    {
      return new Rect(reference.X * scaleFactor.Width, reference.Y * scaleFactor.Height, reference.Width * scaleFactor.Width, reference.Height * scaleFactor.Height);
    }
    public static bool OverlapWith(this Rect reference, Rect value)
    {
      if (reference.IsEmpty || reference.IsNaN() || value.IsEmpty || value.IsNaN())
        return false;
      if (reference.Contains(value))
        return true;
      if (value.Contains(reference))
        return true;
      if (reference.IntersectsWith(value))
        return true;
      return false;
    }
    public static bool IsNaN(this Rect reference)
    {
      return double.IsNaN(reference.X) || double.IsNaN(reference.Y) ||
        double.IsNaN(reference.Width) || double.IsNaN(reference.Height);
    }
    public static bool IsZero(this Rect rect)
    {
      return MathEx.IsNullOrZero(rect.X) && MathEx.IsNullOrZero(rect.Y) && MathEx.IsNullOrZero(rect.Width) && MathEx.IsNullOrZero(rect.Height);
    }
    public static bool IsNaNOrZeroOrEmpty(this Rect rect)
    {
      return rect.IsEmpty || rect.IsNaN() || rect.IsZero();
    }
    public static bool RectEquals(this Rect rect, Rect value)
    {
      return MathEx.IsNullOrZero(rect.X - value.X) && MathEx.IsNullOrZero(rect.Y - value.Y) &&
        MathEx.IsNullOrZero(rect.Width - value.Width) && MathEx.IsNullOrZero(rect.Height - value.Height);
    }
    public static Int32Rect ToInt32Rect(this Rect rect)
    {
      return new Int32Rect((int)rect.Left, (int)rect.Top, (int)rect.Width, (int)rect.Height);
    }
  }
}
