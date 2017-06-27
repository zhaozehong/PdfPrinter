using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Zehong.CSharp.Solution.HelperLib
{
  public static class PointExtension
  {
    public static readonly System.Windows.Point ZeroPoint = new System.Windows.Point(0, 0);
    public static readonly System.Windows.Point NaNPoint = new System.Windows.Point(double.NaN, double.NaN);

    public static bool IsZero(this System.Windows.Point point)
    {
      return MathEx.IsNullOrZero(point.X) && MathEx.IsNullOrZero(point.Y);
    }
    public static bool IsNaN(this System.Windows.Point point)
    {
      return double.IsNaN(point.X) || double.IsNaN(point.Y);
    }
    public static bool PointEquals(this System.Windows.Point point, System.Windows.Point value)
    {
      return MathEx.IsNullOrZero(point.X - value.X) && MathEx.IsNullOrZero(point.Y - value.Y);
    }
    public static Point3D ToPoint3D(this System.Windows.Point reference)
    {
      return new Point3D(reference.X, reference.Y, 0);
    }
    public static Vector ToVector(this System.Windows.Point reference)
    {
      return new Vector(reference.X, reference.Y);
    }
    public static System.Windows.Point Scale(this System.Windows.Point point, double scaleFactor)
    {
      return new System.Windows.Point(point.X * scaleFactor, point.Y * scaleFactor);
    }
    public static System.Windows.Point Scale(this System.Windows.Point point, System.Windows.Size scaleFactor)
    {
      return new System.Windows.Point(point.X * scaleFactor.Width, point.Y * scaleFactor.Height);
    }
  }
}
