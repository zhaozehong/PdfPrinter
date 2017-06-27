using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Markup.Primitives;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace Zehong.CSharp.Solution.HelperLib
{
  public static class DispatcherObjectExternsion
  {
    #region Thread safe functions
    public static object SafeConvertInvariantStringTo(this string text, Type type)
    {
      if (Application.Current == null || Application.Current.CheckAccess() || !type.IsSubclassOf(typeof(DispatcherObject)))
        return System.ComponentModel.TypeDescriptor.GetConverter(type).ConvertFromInvariantString(text);

      object ret = null;
      Application.Current.Dispatcher.Invoke(
        new Action(() => ret = System.ComponentModel.TypeDescriptor.GetConverter(type).ConvertFromInvariantString(text)));
      return ret;
    }

    public static T SafeConvertInvariantStringTo<T>(this string text)
    {
      return (T)text.SafeConvertInvariantStringTo(typeof(T));
    }

    public static object SafeCreateInstance(this Type type)
    {
      try
      {
        var dispatcherObject = Application.Current;

        if (dispatcherObject == null || dispatcherObject.CheckAccess())
          return Activator.CreateInstance(type);

        object ret = default(object);
        dispatcherObject.Dispatcher.Invoke(new Action(delegate { ret = Activator.CreateInstance(type); }));
        return ret;
      }
      catch (Exception ex)
      {
        ExceptionHandler.ThrowException(ex);
        throw;
      }
    }

    public static object SafeCreateInstance(this Assembly assembly, string typeName)
    {
      try
      {
        var dispatcherObject = Application.Current;

        if (dispatcherObject == null || dispatcherObject.CheckAccess())
          return assembly.CreateInstance(typeName);

        object ret = default(object);
        dispatcherObject.Dispatcher.Invoke(new Action(delegate { ret = assembly.CreateInstance(typeName); }));
        return ret;
      }
      catch (Exception ex)
      {
        ExceptionHandler.ThrowException(ex);
        throw;
      }
    }

    public static T SafeCreateInstance<T>(params object[] args)
    {
      try
      {
        Type type = typeof(T);

        var dispatcherObject = Application.Current;

        const BindingFlags bf =
          BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance | BindingFlags.NonPublic;
        if (dispatcherObject == null || dispatcherObject.CheckAccess())
          return (T)Activator.CreateInstance(type, bf, null, args, null);

        T ret = default(T);
        dispatcherObject.Dispatcher.Invoke(
          new Action(delegate { ret = (T)Activator.CreateInstance(type, bf, null, args, null); }));
        return ret;
      }
      catch (Exception ex)
      {
        ExceptionHandler.ThrowException(ex);
        throw;
      }
    }

    public static T SafeCreateInstance<T>(this DispatcherObject dispatcherObject, params object[] args)
    {
      try
      {
        Type type = typeof(T);

        const BindingFlags bf =
          BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance | BindingFlags.NonPublic;
        if (dispatcherObject.CheckAccess())
          return (T)Activator.CreateInstance(type, bf, null, args, null);

        T ret = default(T);
        dispatcherObject.Dispatcher.Invoke(
          new Action(delegate { ret = (T)Activator.CreateInstance(type, bf, null, args, null); }));
        return ret;
      }
      catch (Exception ex)
      {
        ExceptionHandler.ThrowException(ex);
        throw;
      }
    }

    public static object SafeGetValue(this DependencyObject dependencyObject, DependencyProperty dp)
    {
      if (dependencyObject.CheckAccess())
        return dependencyObject.GetValue(dp);

      object ret = null;
      dependencyObject.Dispatcher.Invoke(new Action(delegate { ret = dependencyObject.GetValue(dp); }));
      return ret;
    }

    public static object SafeGetValue(this DependencyObject dependencyObject, string propertyName)
    {
      var ppInfo = dependencyObject.GetType().GetProperty(propertyName);
      if (ppInfo == null)
        return null;

      if (dependencyObject.CheckAccess())
        return ppInfo.GetValue(dependencyObject, new object[] { });

      object ret = null;
      dependencyObject.Dispatcher.Invoke(
        new Action(delegate { ret = ppInfo.GetValue(dependencyObject, new object[] { }); }));
      return ret;
    }

    public static object SafeGetValue(this PropertyInfo ppInfo, object obj)
    {
      var dispatcherObject = obj as DispatcherObject;
      if (dispatcherObject == null || dispatcherObject.CheckAccess())
      {
        return ppInfo.GetValue(obj, new object[] { });
      }

      object ret = null;
      dispatcherObject.Dispatcher.Invoke(new Action(delegate { ret = ppInfo.GetValue(obj, new object[] { }); }));
      return ret;
    }


    public static UIElement SafeGetChild(this Decorator decorator)
    {
      if (decorator.CheckAccess())
        return decorator.Child;

      UIElement ret = null;
      decorator.Dispatcher.Invoke(new Action(delegate { ret = decorator.Child; }));
      return ret;
    }

    public static void SafeSetChild(this Decorator decorator, UIElement value)
    {
      if (decorator.CheckAccess())
      {
        decorator.Child = value;
        return;
      }

      decorator.Dispatcher.Invoke(new Action(delegate { decorator.Child = value; }));
    }

    public static object SafeGetContent(this ContentControl reference)
    {
      if (reference.CheckAccess())
        return reference.Content;

      object ret = null;
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.Content; }));
      return ret;
    }

    public static void SafeSetContent(this ContentControl reference, object content)
    {
      if (reference.CheckAccess())
      {
        reference.Content = content;
        return;
      }

      reference.Dispatcher.Invoke(new Action(delegate { reference.Content = content; }));
    }

    public static Model3D SafeGetContent(this ModelVisual3D reference)
    {
      if (reference.CheckAccess())
        return reference.Content;

      Model3D ret = null;
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.Content; }));
      return ret;
    }

    public static void SafeSetContent(this ModelVisual3D reference, Model3D content)
    {
      if (reference.CheckAccess())
      {
        reference.Content = content;
        return;
      }

      reference.Dispatcher.Invoke(new Action(delegate { reference.Content = content; }));
    }

    public static List<Visual3D> SafeGetChildren(this Viewport3D reference)
    {
      if (reference.CheckAccess())
        return reference.Children.ToList();

      List<Visual3D> ret = null;
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.Children.ToList(); }));
      return ret;
    }

    public static int SafeGetChildrenCount(this Viewport3D reference)
    {
      if (reference.CheckAccess())
        return reference.Children.Count;

      int ret = 0;
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.Children.Count; }));
      return ret;
    }

    public static void SafeClearChildren(this Viewport3D reference)
    {
      if (reference.CheckAccess())
      {
        reference.Children.Clear();
        return;
      }

      reference.Dispatcher.Invoke(new Action(delegate { reference.Children.Clear(); }));
    }

    public static void SafeAddChild(this Viewport3D reference, Visual3D child)
    {
      if (reference.CheckAccess())
      {
        reference.Children.Add(child);
        return;
      }

      reference.Dispatcher.Invoke(new Action(delegate { reference.Children.Add(child); }));
    }

    public static void SafeRemoveChild(this Viewport3D reference, Visual3D child)
    {
      if (reference.CheckAccess())
      {
        reference.Children.Remove(child);
        return;
      }

      reference.Dispatcher.Invoke(new Action(delegate { reference.Children.Remove(child); }));
    }

    public static bool SafeContainsChild(this Viewport3D reference, Visual3D child)
    {
      if (reference.CheckAccess())
      {
        return reference.Children.Contains(child);
      }

      bool ret = false;
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.Children.Contains(child); }));
      return ret;
    }

    public static List<UIElement> SafeGetChildren(this Panel reference)
    {
      if (reference.CheckAccess())
        return reference.Children.OfType<UIElement>().ToList();

      List<UIElement> ret = null;
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.Children.OfType<UIElement>().ToList(); }));
      return ret;
    }

    public static int SafeGetChildrenCount(this Panel reference)
    {
      if (reference.CheckAccess())
        return reference.Children.Count;

      int ret = 0;
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.Children.Count; }));
      return ret;
    }

    public static void SafeClearChildren(this Panel reference)
    {
      if (reference.CheckAccess())
      {
        reference.Children.Clear();
        return;
      }

      reference.Dispatcher.Invoke(new Action(delegate { reference.Children.Clear(); }));
    }

    public static void SafeAddChild(this Panel reference, UIElement child)
    {
      if (reference.CheckAccess())
      {
        reference.Children.Add(child);
        return;
      }

      reference.Dispatcher.Invoke(new Action(() => reference.Children.Add(child)));
    }

    public static void SafeRemoveChild(this Panel reference, UIElement child)
    {
      if (reference.CheckAccess())
      {
        reference.Children.Remove(child);
        return;
      }

      reference.Dispatcher.Invoke(new Action(delegate { reference.Children.Remove(child); }));
    }

    public static bool SafeContainsChild(this Panel reference, UIElement child)
    {
      if (reference.CheckAccess())
      {
        return reference.Children.Contains(child);
      }

      bool ret = false;
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.Children.Contains(child); }));
      return ret;
    }

    public static Brush SafeGetBackground(this Panel reference)
    {
      if (reference.CheckAccess())
        return reference.Background;

      Brush ret = null;
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.Background; }));
      return ret;
    }

    public static void SafeSetBackground(this Panel reference, Brush value)
    {
      if (reference.CheckAccess())
      {
        reference.Background = value;
        return;
      }

      reference.Dispatcher.Invoke(new Action(delegate { reference.Background = value; }));
    }

    public static void SafeAddChild(this IAddChild reference, UIElement child)
    {
      var dispatcherObject = reference as DispatcherObject;

      if (dispatcherObject == null || dispatcherObject.CheckAccess())
      {
        reference.AddChild(child);
        return;
      }

      dispatcherObject.Dispatcher.Invoke(new Action(() => reference.AddChild(child)));
    }

    public static List<Model3D> SafeGetChildren(this Model3DGroup reference)
    {
      if (reference.CheckAccess())
        return reference.Children.ToList();

      List<Model3D> ret = null;
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.Children.ToList(); }));
      return ret;
    }

    public static int SafeGetChildrenCount(this Model3DGroup reference)
    {
      if (reference.CheckAccess())
        return reference.Children.Count;

      int ret = 0;
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.Children.Count; }));
      return ret;
    }

    public static void SafeClearChildren(this Model3DGroup reference)
    {
      if (reference.CheckAccess())
      {
        reference.Children.Clear();
        return;
      }

      reference.Dispatcher.Invoke(new Action(delegate { reference.Children.Clear(); }));
    }

    public static void SafeAddChild(this Model3DGroup reference, Model3D child)
    {
      if (reference.CheckAccess())
      {
        reference.Children.Add(child);
        return;
      }

      reference.Dispatcher.Invoke(new Action(delegate { reference.Children.Add(child); }));
    }

    public static void SafeRemoveChild(this FixedPage reference, UIElement child)
    {
      if (reference.CheckAccess())
      {
        reference.Children.Remove(child);
        return;
      }

      reference.Dispatcher.Invoke(new Action(delegate { reference.Children.Remove(child); }));
    }

    public static void SafeAddChild(this FixedPage reference, UIElement child)
    {
      if (reference.CheckAccess())
      {
        reference.Children.Add(child);
        return;
      }

      reference.Dispatcher.Invoke(new Action(delegate { reference.Children.Add(child); }));
    }

    public static void SafeRemoveChild(this Model3DGroup reference, Model3D child)
    {
      if (reference.CheckAccess())
      {
        reference.Children.Remove(child);
        return;
      }

      reference.Dispatcher.Invoke(new Action(delegate { reference.Children.Remove(child); }));
    }

    public static bool SafeContainsChild(this Model3DGroup reference, Model3D child)
    {
      if (reference.CheckAccess())
      {
        return reference.Children.Contains(child);
      }

      bool ret = false;
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.Children.Contains(child); }));
      return ret;
    }

    public static List<Transform> SafeGetChildren(this TransformGroup reference)
    {
      if (reference.CheckAccess())
        return reference.Children.ToList();

      List<Transform> ret = null;
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.Children.ToList(); }));
      return ret;
    }

    public static int SafeGetChildrenCount(this TransformGroup reference)
    {
      if (reference.CheckAccess())
        return reference.Children.Count;

      int ret = 0;
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.Children.Count; }));
      return ret;
    }

    public static void SafeClearChildren(this TransformGroup reference)
    {
      if (reference.CheckAccess())
      {
        reference.Children.Clear();
        return;
      }

      reference.Dispatcher.Invoke(new Action(delegate { reference.Children.Clear(); }));
    }

    public static void SafeAddChild(this TransformGroup reference, Transform child)
    {
      if (reference.CheckAccess())
      {
        reference.Children.Add(child);
        return;
      }

      reference.Dispatcher.Invoke(new Action(delegate { reference.Children.Add(child); }));
    }

    public static void SafeRemoveChild(this TransformGroup reference, Transform child)
    {
      if (reference.CheckAccess())
      {
        reference.Children.Remove(child);
        return;
      }

      reference.Dispatcher.Invoke(new Action(delegate { reference.Children.Remove(child); }));
    }

    public static bool SafeContainsChild(this TransformGroup reference, Transform child)
    {
      if (reference.CheckAccess())
      {
        return reference.Children.Contains(child);
      }

      bool ret = false;
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.Children.Contains(child); }));
      return ret;
    }

    public static List<Transform3D> SafeGetChildren(this Transform3DGroup reference)
    {
      if (reference.CheckAccess())
        return reference.Children.ToList();

      List<Transform3D> ret = null;
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.Children.ToList(); }));
      return ret;
    }

    public static int SafeGetChildrenCount(this Transform3DGroup reference)
    {
      if (reference.CheckAccess())
        return reference.Children.Count;

      int ret = 0;
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.Children.Count; }));
      return ret;
    }

    public static void SafeClearChildren(this Transform3DGroup reference)
    {
      if (reference.CheckAccess())
      {
        reference.Children.Clear();
        return;
      }

      reference.Dispatcher.Invoke(new Action(delegate { reference.Children.Clear(); }));
    }

    public static void SafeAddChild(this Transform3DGroup reference, Transform3D child)
    {
      if (reference.CheckAccess())
      {
        reference.Children.Add(child);
        return;
      }

      reference.Dispatcher.Invoke(new Action(delegate { reference.Children.Add(child); }));
    }

    public static void SafeRemoveChild(this Transform3DGroup reference, Transform3D child)
    {
      if (reference.CheckAccess())
      {
        reference.Children.Remove(child);
        return;
      }

      reference.Dispatcher.Invoke(new Action(delegate { reference.Children.Remove(child); }));
    }

    public static bool SafeContainsChild(this Transform3DGroup reference, Transform3D child)
    {
      if (reference.CheckAccess())
      {
        return reference.Children.Contains(child);
      }

      bool ret = false;
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.Children.Contains(child); }));
      return ret;
    }

    public static System.Windows.Point SafeTransform(this GeneralTransform reference, System.Windows.Point point)
    {
      if (reference.CheckAccess())
      {
        return reference.Transform(point);
      }

      var ret = default(System.Windows.Point);
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.Transform(point); }));
      return ret;
    }
    public static GeneralTransform SafeGetInverse(this GeneralTransform reference)
    {
      if (reference.CheckAccess())
      {
        return reference.Inverse;
      }

      var ret = default(GeneralTransform);
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.Inverse; }));
      return ret;
    }
    public static double SafeGetCenterX(this RotateTransform reference)
    {
      if (reference.CheckAccess())
      {
        return reference.CenterX;
      }
      var ret = default(double);
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.CenterX; }));
      return ret;
    }
    public static void SafeSetCenterX(this RotateTransform reference, double value)
    {
      if (reference.CheckAccess())
      {
        reference.CenterX = value;
        return;
      }

      reference.Dispatcher.Invoke(new Action(delegate { reference.CenterX = value; }));
    }

    public static double SafeGetCenterY(this RotateTransform reference)
    {
      if (reference.CheckAccess())
      {
        return reference.CenterY;
      }
      var ret = default(double);
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.CenterY; }));
      return ret;
    }

    public static void SafeSetCenterY(this RotateTransform reference, double value)
    {
      if (reference.CheckAccess())
      {
        reference.CenterY = value;
        return;
      }

      reference.Dispatcher.Invoke(new Action(delegate { reference.CenterY = value; }));
    }

    public static double SafeGetAngle(this RotateTransform reference)
    {
      if (reference.CheckAccess())
      {
        return reference.Angle;
      }
      var ret = default(double);
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.Angle; }));
      return ret;
    }

    public static void SafeSetAngle(this RotateTransform reference, double value)
    {
      if (reference.CheckAccess())
      {
        reference.Angle = value;
        return;
      }

      reference.Dispatcher.Invoke(new Action(delegate { reference.Angle = value; }));
    }

    public static double SafeGetX(this TranslateTransform reference)
    {
      if (reference.CheckAccess())
      {
        return reference.X;
      }
      var ret = default(double);
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.X; }));
      return ret;
    }

    public static void SafeSetX(this TranslateTransform reference, double value)
    {
      if (reference.CheckAccess())
      {
        reference.X = value;
        return;
      }

      reference.Dispatcher.Invoke(new Action(delegate { reference.X = value; }));
    }

    public static double SafeGetY(this TranslateTransform reference)
    {
      if (reference.CheckAccess())
      {
        return reference.Y;
      }
      var ret = default(double);
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.Y; }));
      return ret;
    }

    public static void SafeSetY(this TranslateTransform reference, double value)
    {
      if (reference.CheckAccess())
      {
        reference.Y = value;
        return;
      }

      reference.Dispatcher.Invoke(new Action(delegate { reference.Y = value; }));
    }

    public static Matrix SafeGetValue(this TranslateTransform reference)
    {
      if (reference.CheckAccess())
      {
        return reference.Value;
      }
      var ret = default(Matrix);
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.Value; }));
      return ret;
    }

    public static double SafeGetCenterX(this ScaleTransform reference)
    {
      if (reference.CheckAccess())
      {
        return reference.CenterX;
      }
      var ret = default(double);
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.CenterX; }));
      return ret;
    }

    public static void SafeSetCenterX(this ScaleTransform reference, double value)
    {
      if (reference.CheckAccess())
      {
        reference.CenterX = value;
        return;
      }

      reference.Dispatcher.Invoke(new Action(delegate { reference.CenterX = value; }));
    }

    public static double SafeGetCenterY(this ScaleTransform reference)
    {
      if (reference.CheckAccess())
      {
        return reference.CenterY;
      }
      var ret = default(double);
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.CenterY; }));
      return ret;
    }

    public static void SafeSetCenterY(this ScaleTransform reference, double value)
    {
      if (reference.CheckAccess())
      {
        reference.CenterY = value;
        return;
      }

      reference.Dispatcher.Invoke(new Action(delegate { reference.CenterY = value; }));
    }

    public static double SafeGetScaleX(this ScaleTransform reference)
    {
      if (reference.CheckAccess())
      {
        return reference.ScaleX;
      }
      var ret = default(double);
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.ScaleX; }));
      return ret;
    }

    public static void SafeSetScaleX(this ScaleTransform reference, double value)
    {
      if (reference.CheckAccess())
      {
        reference.ScaleX = value;
        return;
      }

      reference.Dispatcher.Invoke(new Action(delegate { reference.ScaleX = value; }));
    }

    public static double SafeGetScaleY(this ScaleTransform reference)
    {
      if (reference.CheckAccess())
      {
        return reference.ScaleY;
      }
      var ret = default(double);
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.ScaleY; }));
      return ret;
    }
    public static void SafeSetScaleY(this ScaleTransform reference, double value)
    {
      if (reference.CheckAccess())
      {
        reference.ScaleY = value;
        return;
      }

      reference.Dispatcher.Invoke(new Action(delegate { reference.ScaleY = value; }));
    }

    public static Point3D SafeGetCenter(this ScaleTransform3D reference)
    {
      if (reference.CheckAccess())
      {
        return new Point3D(reference.CenterX, reference.CenterY, reference.CenterZ);
      }
      var ret = default(Point3D);
      reference.Dispatcher.Invoke(new Action(delegate { ret = new Point3D(reference.CenterX, reference.CenterY, reference.CenterZ); }));
      return ret;
    }
    public static void SafeSetCenter(this ScaleTransform3D reference, Point3D value)
    {
      if (reference.CheckAccess())
      {
        reference.CenterX = value.X;
        reference.CenterY = value.Y;
        reference.CenterZ = value.Z;
        return;
      }

      reference.Dispatcher.Invoke(new Action(delegate
      {
        reference.CenterX = value.X;
        reference.CenterY = value.Y;
        reference.CenterZ = value.Z;
      }));
    }
    public static double SafeGetCenterX(this ScaleTransform3D reference)
    {
      if (reference.CheckAccess())
      {
        return reference.CenterX;
      }
      var ret = default(double);
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.CenterX; }));
      return ret;
    }
    public static void SafeSetCenterX(this ScaleTransform3D reference, double value)
    {
      if (reference.CheckAccess())
      {
        reference.CenterX = value;
        return;
      }

      reference.Dispatcher.Invoke(new Action(delegate { reference.CenterX = value; }));
    }
    public static double SafeGetCenterY(this ScaleTransform3D reference)
    {
      if (reference.CheckAccess())
      {
        return reference.CenterY;
      }
      var ret = default(double);
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.CenterY; }));
      return ret;
    }
    public static void SafeSetCenterY(this ScaleTransform3D reference, double value)
    {
      if (reference.CheckAccess())
      {
        reference.CenterY = value;
        return;
      }

      reference.Dispatcher.Invoke(new Action(delegate { reference.CenterY = value; }));
    }
    public static double SafeGetCenterZ(this ScaleTransform3D reference)
    {
      if (reference.CheckAccess())
      {
        return reference.CenterZ;
      }
      var ret = default(double);
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.CenterZ; }));
      return ret;
    }
    public static void SafeSetCenterZ(this ScaleTransform3D reference, double value)
    {
      if (reference.CheckAccess())
      {
        reference.CenterZ = value;
        return;
      }

      reference.Dispatcher.Invoke(new Action(delegate { reference.CenterZ = value; }));
    }

    public static Vector3D SafeGetScale(this ScaleTransform3D reference)
    {
      if (reference.CheckAccess())
      {
        return new Vector3D(reference.ScaleX, reference.ScaleY, reference.ScaleZ);
      }
      var ret = default(Vector3D);
      reference.Dispatcher.Invoke(new Action(delegate { ret = new Vector3D(reference.ScaleX, reference.ScaleY, reference.ScaleZ); }));
      return ret;
    }
    public static void SafeSetScale(this ScaleTransform3D reference, Vector3D value)
    {
      if (reference.CheckAccess())
      {
        reference.ScaleX = value.X;
        reference.ScaleY = value.Y;
        reference.ScaleZ = value.Z;
        return;
      }

      reference.Dispatcher.Invoke(new Action(delegate
      {
        reference.ScaleX = value.X;
        reference.ScaleY = value.Y;
        reference.ScaleZ = value.Z;
      }));
    }
    public static double SafeGetScaleX(this ScaleTransform3D reference)
    {
      if (reference.CheckAccess())
      {
        return reference.ScaleX;
      }
      var ret = default(double);
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.ScaleX; }));
      return ret;
    }
    public static void SafeSetScaleX(this ScaleTransform3D reference, double value)
    {
      if (reference.CheckAccess())
      {
        reference.ScaleX = value;
        return;
      }

      reference.Dispatcher.Invoke(new Action(delegate { reference.ScaleX = value; }));
    }
    public static double SafeGetScaleY(this ScaleTransform3D reference)
    {
      if (reference.CheckAccess())
      {
        return reference.ScaleY;
      }
      var ret = default(double);
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.ScaleY; }));
      return ret;
    }
    public static void SafeSetScaleY(this ScaleTransform3D reference, double value)
    {
      if (reference.CheckAccess())
      {
        reference.ScaleY = value;
        return;
      }

      reference.Dispatcher.Invoke(new Action(delegate { reference.ScaleY = value; }));
    }
    public static double SafeGetScaleZ(this ScaleTransform3D reference)
    {
      if (reference.CheckAccess())
      {
        return reference.ScaleZ;
      }
      var ret = default(double);
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.ScaleZ; }));
      return ret;
    }
    public static void SafeSetScaleZ(this ScaleTransform3D reference, double value)
    {
      if (reference.CheckAccess())
      {
        reference.ScaleZ = value;
        return;
      }

      reference.Dispatcher.Invoke(new Action(delegate { reference.ScaleZ = value; }));
    }

    public static Point3D SafeGetCenter(this RotateTransform3D reference)
    {
      if (reference.CheckAccess())
      {
        return new Point3D(reference.CenterX, reference.CenterY, reference.CenterZ);
      }
      var ret = default(Point3D);
      reference.Dispatcher.Invoke(new Action(delegate { ret = new Point3D(reference.CenterX, reference.CenterY, reference.CenterZ); }));
      return ret;
    }
    public static void SafeSetCenter(this RotateTransform3D reference, Point3D value)
    {
      if (reference.CheckAccess())
      {
        reference.CenterX = value.X;
        reference.CenterY = value.Y;
        reference.CenterZ = value.Z;
        return;
      }

      reference.Dispatcher.Invoke(new Action(delegate
      {
        reference.CenterX = value.X;
        reference.CenterY = value.Y;
        reference.CenterZ = value.Z;
      }));
    }

    public static Vector3D SafeGetOffset(this TranslateTransform3D reference)
    {
      if (reference.CheckAccess())
      {
        return new Vector3D(reference.OffsetX, reference.OffsetY, reference.OffsetZ);
      }
      var ret = default(Vector3D);
      reference.Dispatcher.Invoke(new Action(delegate { ret = new Vector3D(reference.OffsetX, reference.OffsetY, reference.OffsetZ); }));
      return ret;
    }
    public static void SafeSetOffset(this TranslateTransform3D reference, Vector3D value)
    {
      if (reference.CheckAccess())
      {
        reference.OffsetX = value.X;
        reference.OffsetY = value.Y;
        reference.OffsetZ = value.Z;
        return;
      }

      reference.Dispatcher.Invoke(new Action(delegate
      {
        reference.OffsetX = value.X;
        reference.OffsetY = value.Y;
        reference.OffsetZ = value.Z;
      }));
    }

    public static double SafeGetOffsetX(this TranslateTransform3D reference)
    {
      if (reference.CheckAccess())
      {
        return reference.OffsetX;
      }
      var ret = default(double);
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.OffsetX; }));
      return ret;
    }

    public static void SafeSetOffsetX(this TranslateTransform3D reference, double value)
    {
      if (reference.CheckAccess())
      {
        reference.OffsetX = value;
        return;
      }

      reference.Dispatcher.Invoke(new Action(delegate { reference.OffsetX = value; }));
    }

    public static double SafeGetOffsetY(this TranslateTransform3D reference)
    {
      if (reference.CheckAccess())
      {
        return reference.OffsetY;
      }
      var ret = default(double);
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.OffsetY; }));
      return ret;
    }

    public static void SafeSetOffsetY(this TranslateTransform3D reference, double value)
    {
      if (reference.CheckAccess())
      {
        reference.OffsetY = value;
        return;
      }

      reference.Dispatcher.Invoke(new Action(delegate { reference.OffsetY = value; }));
    }

    public static double SafeGetOffsetZ(this TranslateTransform3D reference)
    {
      if (reference.CheckAccess())
      {
        return reference.OffsetZ;
      }
      var ret = default(double);
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.OffsetZ; }));
      return ret;
    }

    public static void SafeSetOffsetZ(this TranslateTransform3D reference, double value)
    {
      if (reference.CheckAccess())
      {
        reference.OffsetZ = value;
        return;
      }

      reference.Dispatcher.Invoke(new Action(delegate { reference.OffsetZ = value; }));
    }

    public static double SafeGetAngle(this AxisAngleRotation3D reference)
    {
      if (reference.CheckAccess())
      {
        return reference.Angle;
      }
      var ret = default(double);
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.Angle; }));
      return ret;
    }

    public static void SafeSetAngle(this AxisAngleRotation3D reference, double value)
    {
      if (reference.CheckAccess())
      {
        reference.Angle = value;
        return;
      }

      reference.Dispatcher.Invoke(new Action(delegate { reference.Angle = value; }));
    }

    public static Vector3D SafeGetAxis(this AxisAngleRotation3D reference)
    {
      if (reference.CheckAccess())
      {
        return reference.Axis;
      }
      var ret = default(Vector3D);
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.Axis; }));
      return ret;
    }

    public static void SafeSetAxis(this AxisAngleRotation3D reference, Vector3D value)
    {
      if (reference.CheckAccess())
      {
        reference.Axis = value;
        return;
      }

      reference.Dispatcher.Invoke(new Action(delegate { reference.Axis = value; }));
    }

    public static Matrix SafeGetMatrix(this MatrixTransform reference)
    {
      if (reference.CheckAccess())
      {
        return reference.Matrix;
      }
      var ret = default(Matrix);
      reference.Dispatcher.Invoke(new Action(delegate { ret = reference.Matrix; }));
      return ret;
    }

    public static void SafeSetMatrix(this MatrixTransform reference, Matrix value)
    {
      if (reference.CheckAccess())
      {
        reference.Matrix = value;
        return;
      }

      reference.Dispatcher.Invoke(new Action(delegate { reference.Matrix = value; }));
    }

    public static DependencyObject SafeGetVisualParent(this DependencyObject dependencyObject)
    {
      var dispatcherObject = dependencyObject as DispatcherObject;
      if (dispatcherObject == null || dispatcherObject.CheckAccess())
        return VisualTreeHelper.GetParent(dependencyObject);

      DependencyObject ret = null;
      dispatcherObject.Dispatcher.Invoke(new Action(delegate { ret = VisualTreeHelper.GetParent(dependencyObject); }));
      return ret;
    }

    public static DependencyObject SafeGetVisualParent(this Visual3D visual3D)
    {
      if (visual3D.CheckAccess())
        return VisualTreeHelper.GetParent(visual3D);

      DependencyObject ret = null;
      visual3D.Dispatcher.Invoke(new Action(delegate { ret = VisualTreeHelper.GetParent(visual3D); }));
      return ret;
    }

    public static DependencyObject SafeGetLogicalParent(this DependencyObject dependencyObject)
    {
      var dispatcherObject = dependencyObject as DispatcherObject;
      if (dispatcherObject == null || dispatcherObject.CheckAccess())
        return LogicalTreeHelper.GetParent(dependencyObject);

      DependencyObject ret = null;
      dispatcherObject.Dispatcher.Invoke(new Action(delegate { ret = LogicalTreeHelper.GetParent(dependencyObject); }));
      return ret;
    }

    public static int SafeGetVisualChildrenCount(this DependencyObject reference)
    {
      var dispatcherObject = reference as DispatcherObject;
      if (dispatcherObject == null || dispatcherObject.CheckAccess())
        return VisualTreeHelper.GetChildrenCount(reference);

      int ret = 0;
      dispatcherObject.Dispatcher.Invoke(new Action(delegate { ret = VisualTreeHelper.GetChildrenCount(reference); }));
      return ret;
    }

    public static DependencyObject SafeGetVisualChild(this DependencyObject reference, int childIndex)
    {
      var dispatcherObject = reference as DispatcherObject;
      if (dispatcherObject == null || dispatcherObject.CheckAccess())
        return VisualTreeHelper.GetChild(reference, childIndex);

      DependencyObject ret = null;
      dispatcherObject.Dispatcher.Invoke(new Action(delegate { ret = VisualTreeHelper.GetChild(reference, childIndex); }));
      return ret;
    }

    public static void SafeSetValue(this DependencyObject dependencyObject, DependencyProperty dp, object value)
    {
      if (dependencyObject.CheckAccess())
      {
        dependencyObject.SetValue(dp, value);
        return;
      }

      dependencyObject.Dispatcher.Invoke(new Action(delegate { dependencyObject.SetValue(dp, value); }));
    }

    public static void SafeSetValue(this DependencyObject dependencyObject, string propertyName, object value)
    {
      var ppInfo = dependencyObject.GetType().GetProperty(propertyName);
      if (ppInfo == null)
        return;

      if (dependencyObject.CheckAccess())
      {
        ppInfo.SetValue(dependencyObject, value, new object[] { });
        return;
      }

      dependencyObject.Dispatcher.Invoke(
        new Action(delegate { ppInfo.SetValue(dependencyObject, value, new object[] { }); }));
    }

    public static void SafeSetValue(this PropertyInfo ppInfo, object obj, object value)
    {
      var dispatcherObject = obj as DispatcherObject;
      if (dispatcherObject == null || dispatcherObject.CheckAccess())
      {
        if (value != null)
        {
          var valueType = value.GetType();
          if (!(valueType == ppInfo.PropertyType || ppInfo.PropertyType.IsAssignableFrom(valueType) ||
                ppInfo.PropertyType.IsSubclassOf(valueType)))
          {
            string objstr = (value is string) ? (value as string) : value.ToString();
            value = (ppInfo.PropertyType == typeof(Type))
              ? Type.GetType(objstr)
              : objstr.SafeConvertInvariantStringTo(ppInfo.PropertyType);
          }
        }
        ppInfo.SetValue(obj, value, new object[] { });
        return;
      }

      dispatcherObject.Dispatcher.Invoke(new Action(delegate { ppInfo.SafeSetValue(obj, value); }));
    }

    #endregion

    #region Change the dispatcher index of DispatcherObject

    public static void DetachFromDispatcher(this DispatcherObject dispatcherObject)
    {
      Type t = dispatcherObject.GetType();
      MethodInfo m = t.GetMethod("DetachFromDispatcher", BindingFlags.NonPublic | BindingFlags.Instance);
      m.Invoke(dispatcherObject, new object[] { });
    }

    public static void SetDispatcher(this DispatcherObject dispatcherObject, Dispatcher targetDispatcher)
    {
      targetDispatcher.VerifyAccess();

      try
      {
        var relations = new List<DispatcherObject>();
        DoSetDispatcher(dispatcherObject, targetDispatcher, relations);
        relations.Clear();

        if (dispatcherObject is FrameworkElement)
        {
          var f = dispatcherObject as FrameworkElement;
          if (f.IsLoaded == false)
            f.Loaded += FrameworkElement_Loaded;
          else
            f.RecreateBindings();
        }
        else if (dispatcherObject is DependencyObject)
        {
          var d = dispatcherObject as DependencyObject;
          d.RecreateBindings();
        }
      }
      catch (Exception ex)
      {
        ExceptionHandler.ThrowException(ex);
      }
    }

    private static void DoSetDispatcher(DispatcherObject dispatcherObject, Dispatcher targetDispatcher,
                                        List<DispatcherObject> relations)
    {
      try
      {
        if (dispatcherObject == null)
          return;

        lock (dispatcherObject)
        {
          System.Diagnostics.Debug.WriteLine(string.Format("{0} ~~DoSetDispatcher locked~~ {1}",
                                                           Thread.CurrentThread.ManagedThreadId,
                                                           dispatcherObject.GetType()));

          if (relations.Contains(dispatcherObject) || dispatcherObject.Dispatcher == null ||
              dispatcherObject.Dispatcher == targetDispatcher)
            return;
          relations.Add(dispatcherObject);

          #region DispatcherObject

          var m = typeof(DispatcherObject).GetField("_dispatcher", BindingFlags.NonPublic | BindingFlags.Instance);
          if (m != null)
            m.SetValue(dispatcherObject, targetDispatcher);

          #endregion

          #region Properties

          var props =
            dispatcherObject.GetType()
                            .GetProperties(BindingFlags.Public | /*BindingFlags.NonPublic |*/ BindingFlags.Instance);
          foreach (var p in props)
          {
            if (p.PropertyType.IsValueType)
              continue;

            if ((p.Name == "Inverse") || p.Name == "TemplateControl")
              continue;

            if (p.GetIndexParameters().Any())
            {
              try
              {
                for (int n = 0; ; n++)
                {
                  var v = p.GetValue(dispatcherObject, new object[] { n }) as DispatcherObject;
                  if (v == null)
                    break;
                  DoSetDispatcher(v, targetDispatcher, relations);
                }
              }
              catch (Exception ex)
              {
                if (!(ex is ArgumentOutOfRangeException || ex.InnerException is ArgumentOutOfRangeException))
                  ExceptionHandler.ThrowException(ex);
              }
            }
            else
            {
              try
              {
                if (p.PropertyType.IsGenericType)
                {
                  var iEnum = p.GetValue(dispatcherObject, new object[] { }) as System.Collections.IEnumerable;

                  if (iEnum != null)
                  {
                    foreach (var iv in iEnum)
                    {
                      var v = iv as DispatcherObject;
                      if (v != null)
                        DoSetDispatcher(v, targetDispatcher, relations);
                    }
                  }
                }
                else
                {
                  var v = p.GetValue(dispatcherObject, new object[] { }) as DispatcherObject;
                  if (v != null)
                    DoSetDispatcher(v, targetDispatcher, relations);
                }
              }
              catch (Exception ex)
              {
                ExceptionHandler.ThrowException(ex);
              }
            }
          }

          #endregion

          #region Children

          var d = dispatcherObject as DependencyObject;
          if (d != null && (d is Visual || d is Visual3D))
          {
            foreach (var item in LogicalTreeHelper.GetChildren(d))
            {
              var child = item as DispatcherObject;
              if (child != null)
                DoSetDispatcher(child, targetDispatcher, relations);
            }

            var childCount = VisualTreeHelper.GetChildrenCount(d);
            for (int i = 0; i < childCount; i++)
            {
              var child = VisualTreeHelper.GetChild(d, i);

              DoSetDispatcher(child, targetDispatcher, relations);
            }
          }

          #endregion
        }
      }
      catch (Exception ex)
      {
        ExceptionHandler.ThrowException(ex);
      }
    }

    private static void FrameworkElement_Loaded(object sender, RoutedEventArgs e)
    {
      var f = sender as FrameworkElement;
      if (f != null && f.IsLoaded)
      {
        f.Loaded -= FrameworkElement_Loaded;
        f.RecreateBindings();
      }
    }

    private static void RecreateBindings(this DependencyObject dispatcherObject)
    {
      var dpList = new List<DependencyProperty>();
      dpList.AddRange(dispatcherObject.GetDependencyProperties());
      dpList.AddRange(dispatcherObject.GetAttachedProperties());

      foreach (var dp in dpList)
      {
        var bd = BindingOperations.GetBindingBase(dispatcherObject, dp);
        if (bd == null)
          continue;

        BindingOperations.ClearBinding(dispatcherObject, dp);
        BindingOperations.SetBinding(dispatcherObject, dp, bd);
      }

      #region children

      if (dispatcherObject is Visual || dispatcherObject is Visual3D)
      {
        int childrenCount = VisualTreeHelper.GetChildrenCount(dispatcherObject);
        for (int i = 0; i < childrenCount; i++)
        {
          DependencyObject child = VisualTreeHelper.GetChild(dispatcherObject, i);
          child.RecreateBindings();
        }
      }

      #endregion
    }

    public static List<BindingBase> GetBindingsRecursive(this DependencyObject dispatcherObject)
    {
      var ret = new List<BindingBase>();
      ret.AddRange(dispatcherObject.GetBindings());

      int childrenCount = VisualTreeHelper.GetChildrenCount(dispatcherObject);
      for (int i = 0; i < childrenCount; i++)
      {
        DependencyObject child = VisualTreeHelper.GetChild(dispatcherObject, i);
        ret.AddRange(child.GetBindingsRecursive());
      }

      return ret;
    }

    public static List<BindingBase> GetBindings(this DependencyObject dispatcherObject)
    {
      var dpList = new List<DependencyProperty>();
      dpList.AddRange(dispatcherObject.GetDependencyProperties());
      dpList.AddRange(dispatcherObject.GetAttachedProperties());

      return dpList.Select(dp => BindingOperations.GetBindingBase(dispatcherObject, dp)).Where(b => b != null).ToList();
    }

    public static List<DependencyProperty> GetDependencyProperties(this Object obj)
    {
      var markupObject = MarkupWriter.GetMarkupObjectFor(obj);

      return
        (from mp in markupObject.Properties where mp.DependencyProperty != null select mp.DependencyProperty).ToList();
    }

    public static List<DependencyProperty> GetAttachedProperties(this Object obj)
    {
      var markupObject = MarkupWriter.GetMarkupObjectFor(obj);

      return (from mp in markupObject.Properties where mp.IsAttached select mp.DependencyProperty).ToList();
    }

    #endregion
  }
}
