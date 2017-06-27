using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zehong.CSharp.Solution.HelperLib
{
  public static class SizeExtension
  {
    public static readonly System.Windows.Size ZeroSize = new System.Windows.Size(0, 0);
    public static readonly System.Windows.Size NaNSize = new System.Windows.Size(double.NaN, double.NaN);

    static public double MaxDimension(this System.Windows.Size reference)
    {
      return Math.Max(reference.Width, reference.Height);
    }
    static public double MinDimension(this System.Windows.Size reference)
    {
      return Math.Min(reference.Width, reference.Height);
    }
    static public bool IsNaN(this System.Windows.Size reference)
    {
      return double.IsNaN(reference.Width) || double.IsNaN(reference.Height);
    }
    static public bool IsZero(this System.Windows.Size reference)
    {
      return reference.Width.IsZero() && reference.Height.IsZero();
    }
  }
}
