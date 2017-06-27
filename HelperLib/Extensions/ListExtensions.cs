using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Zehong.CSharp.Solution.HelperLib
{
  public static class ListExtensions
  {
    public static int SafeGetCount<T>(this IList<T> reference)
    {
      var dispatcherObject = reference as DispatcherObject;
      if (dispatcherObject == null)
        dispatcherObject = Application.Current;
      if (dispatcherObject == null || dispatcherObject.CheckAccess())
        return reference.Count;

      int ret = 0;
      dispatcherObject.Dispatcher.Invoke(new Action(delegate { ret = reference.Count; }));
      return ret;
    }

    public static T SafeGetAt<T>(this IList<T> reference, int index)
    {
      var dispatcherObject = reference as DispatcherObject;
      if (dispatcherObject == null)
        dispatcherObject = Application.Current;
      if (dispatcherObject == null || dispatcherObject.CheckAccess())
        return reference[index];

      T ret = default(T);
      dispatcherObject.Dispatcher.Invoke(new Action(delegate { ret = reference[index]; }));
      return ret;
    }

    public static void SafeSetAt<T>(this IList<T> reference, int index, T value)
    {
      var dispatcherObject = reference as DispatcherObject;
      if (dispatcherObject == null)
        dispatcherObject = Application.Current;
      if (dispatcherObject == null || dispatcherObject.CheckAccess())
      {
        reference[index] = value;
        return;
      }

      dispatcherObject.Dispatcher.Invoke(new Action(delegate { reference[index] = value; }));
    }

    public static int SafeGetIndexOf<T>(this IList<T> reference, T value)
    {
      var dispatcherObject = reference as DispatcherObject;
      if (dispatcherObject == null)
        dispatcherObject = Application.Current;
      if (dispatcherObject == null || dispatcherObject.CheckAccess())
        return reference.IndexOf(value);

      int ret = default(int);
      dispatcherObject.Dispatcher.Invoke(new Action(delegate { ret = reference.IndexOf(value); }));
      return ret;
    }

    public static bool SafeContains<T>(this IList<T> reference, T value)
    {
      var dispatcherObject = reference as DispatcherObject;
      if (dispatcherObject == null)
        dispatcherObject = Application.Current;
      if (dispatcherObject == null || dispatcherObject.CheckAccess())
        return reference.Contains(value);

      bool ret = default(bool);
      dispatcherObject.Dispatcher.Invoke(new Action(delegate { ret = reference.Contains(value); }));
      return ret;
    }

    public static void SafeClear<T>(this IList<T> reference)
    {
      var dispatcherObject = reference as DispatcherObject;
      if (dispatcherObject == null)
        dispatcherObject = Application.Current;
      if (dispatcherObject == null || dispatcherObject.CheckAccess())
      {
        reference.Clear();
        return;
      }

      dispatcherObject.Dispatcher.Invoke(new Action(reference.Clear));
    }

    public static void SafeAdd<T>(this IList<T> reference, T value)
    {
      var dispatcherObject = reference as DispatcherObject;
      if (dispatcherObject == null)
        dispatcherObject = Application.Current;
      if (dispatcherObject == null || dispatcherObject.CheckAccess())
      {
        reference.Add(value);
        return;
      }

      dispatcherObject.Dispatcher.Invoke(new Action(() => reference.Add(value)));
    }

    public static void SafeRemove<T>(this IList<T> reference, T value)
    {
      var dispatcherObject = reference as DispatcherObject;
      if (dispatcherObject == null)
        dispatcherObject = Application.Current;
      if (dispatcherObject == null || dispatcherObject.CheckAccess())
      {
        reference.Remove(value);
        return;
      }

      dispatcherObject.Dispatcher.Invoke(new Action(() => reference.Remove(value)));
    }
    public static void SafeInsert<T>(this IList<T> reference, int index, T value)
    {
      var dispatcherObject = reference as DispatcherObject;
      if (dispatcherObject == null)
        dispatcherObject = Application.Current;
      if (dispatcherObject == null || dispatcherObject.CheckAccess())
      {
        reference.Insert(index, value);
        return;
      }
      dispatcherObject.Dispatcher.Invoke(new Action(() => reference.Insert(index, value)));
    }
    public static void SafeRemoveAt<T>(this IList<T> reference, int index)
    {
      var dispatcherObject = reference as DispatcherObject;
      if (dispatcherObject == null)
        dispatcherObject = Application.Current;
      if (dispatcherObject == null || dispatcherObject.CheckAccess())
      {
        reference.RemoveAt(index);
        return;
      }

      dispatcherObject.Dispatcher.Invoke(new Action(() => reference.RemoveAt(index)));
    }

    public static List<T> SafeToList<T>(this IList<T> reference)
    {
      var dispatcherObject = reference as DispatcherObject;
      if (dispatcherObject == null)
        dispatcherObject = Application.Current;
      if (dispatcherObject == null || dispatcherObject.CheckAccess())
        return reference.ToList();

      List<T> ret = default(List<T>);
      dispatcherObject.Dispatcher.Invoke(new Action(delegate { ret = reference.ToList(); }));
      return ret;
    }

    public static T SafeFirstOrDefault<T>(this IList<T> reference)
    {
      var dispatcherObject = reference as DispatcherObject;
      if (dispatcherObject == null)
        dispatcherObject = Application.Current;
      if (dispatcherObject == null || dispatcherObject.CheckAccess())
        return reference.FirstOrDefault();

      T ret = default(T);
      dispatcherObject.Dispatcher.Invoke(new Action(delegate { ret = reference.FirstOrDefault(); }));
      return ret;
    }
    public static T SafeFirstOrDefault<T>(this IList<T> reference, Func<T, bool> predicate)
    {
      var dispatcherObject = reference as DispatcherObject;
      if (dispatcherObject == null)
        dispatcherObject = Application.Current;
      if (dispatcherObject == null || dispatcherObject.CheckAccess())
        return reference.FirstOrDefault(predicate);

      T ret = default(T);
      dispatcherObject.Dispatcher.Invoke(new Action(delegate { ret = reference.FirstOrDefault(predicate); }));
      return ret;
    }
    public static T SafeLastOrDefault<T>(this IList<T> reference)
    {
      var dispatcherObject = reference as DispatcherObject;
      if (dispatcherObject == null)
        dispatcherObject = Application.Current;
      if (dispatcherObject == null || dispatcherObject.CheckAccess())
        return reference.LastOrDefault();

      T ret = default(T);
      dispatcherObject.Dispatcher.Invoke(new Action(delegate { ret = reference.LastOrDefault(); }));
      return ret;
    }
    public static T SafeLastOrDefault<T>(this IList<T> reference, Func<T, bool> predicate)
    {
      var dispatcherObject = reference as DispatcherObject;
      if (dispatcherObject == null)
        dispatcherObject = Application.Current;
      if (dispatcherObject == null || dispatcherObject.CheckAccess())
        return reference.LastOrDefault(predicate);

      T ret = default(T);
      dispatcherObject.Dispatcher.Invoke(new Action(delegate { ret = reference.LastOrDefault(predicate); }));
      return ret;
    }
  }
}
