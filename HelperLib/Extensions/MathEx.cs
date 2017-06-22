using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zehong.CSharp.Solution.HelperLib
{
  public static class MathEx
  {
    static public T Min<T>(params T[] values)
    {
      return values.Min();
    }
    static public T Max<T>(params T[] values)
    {
      return values.Max();
    }
    public static string ToFormtString(this double value, int decimals)
    {
      return !double.IsNaN(value) ? string.Format("{0:F" + decimals.ToString() + "}", value) : String.Empty;
    }
    public static bool IsNotNullOrZero(this double? value)
    {
      return !IsNullOrZero(value);
    }
    public static bool IsNullOrZero(this double? value)
    {
      return value == null || Math.Abs(value.Value) <= 1.0e-12;
    }
    public static bool IsZero(this double reference)
    {
      return Math.Abs(reference) <= 1.0e-12;
    }
  }
}
