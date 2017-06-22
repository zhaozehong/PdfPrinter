using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Zehong.CSharp.Solution.HelperLib.Properties;

namespace Zehong.CSharp.Solution.HelperLib
{
  public static class Helper
  {
    public static String ForceCreateDirectoryEx(String filePath)
    {
      return ForceCreateDirectory(System.IO.Path.GetDirectoryName(filePath));
    }
    public static String ForceCreateDirectory(String directoryPath)
    {
      if (String.IsNullOrWhiteSpace(directoryPath))
        return String.Empty;

      var pathRoot = System.IO.Path.GetPathRoot(directoryPath);
      if (String.IsNullOrWhiteSpace(pathRoot))
        directoryPath = String.Format("{0}\\{1}", AppDataDirectory, directoryPath);

      var parentPath = System.IO.Path.GetDirectoryName(directoryPath);
      if (!System.IO.Directory.Exists(parentPath))
        ForceCreateDirectory(parentPath);

      if (!System.IO.Directory.Exists(directoryPath))
        System.IO.Directory.CreateDirectory(directoryPath);

      return directoryPath;
    }

    public static Boolean IsFileBeingUsed(String filePath)
    {
      if (!System.IO.File.Exists(filePath))
        return false;

      var isBeingUsed = true;
      FileStream fs = null;
      try
      {
        fs = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        isBeingUsed = !fs.CanWrite;
      }
      finally
      {
        if (fs != null)
          fs.Close();
      }
      return isBeingUsed;
    }
    public static String GetValueString(Nullable<Double> value, Int32 decimals)
    {
      if (value == null || Double.IsNaN(value.Value))
        return String.Empty;
      return String.Format("{0:F" + decimals.ToString() + "}", value);
    }
    public static List<String> GetSplitStrings(String strSource, Boolean isClearEmptyValue, params Char[] separator)
    {
      var splitedList = new List<String>();
      if (!String.IsNullOrWhiteSpace(strSource))
      {
        var splitedArray = strSource.Split(separator);
        if (isClearEmptyValue)
          splitedList = splitedArray.Where(p => String.IsNullOrWhiteSpace(p) == false).ToList();
        else
          splitedList = splitedArray.ToList();
      }
      return splitedList;
    }
    public static String ConvertStringByCapitalChar(String strSource)
    {
      StringBuilder sb = new StringBuilder();
      Char[] array = strSource.ToCharArray();
      bool prevChartIsCaptital = true;
      foreach (Char c in array)
      {
        if (Regex.IsMatch(c.ToString(), "[A-Z]"))
        {
          if (!prevChartIsCaptital)
            sb.Append(' ');
          prevChartIsCaptital = true;
        }
        else
        {
          prevChartIsCaptital = false;
        }
        sb.Append(c);
      }
      return sb.ToString();
    }
    public static List<String[]> ReadCSV(String filePathName)
    {
      var arrayList = new List<String[]>();
      StreamReader fileReader = null;
      try
      {
        fileReader = new StreamReader(filePathName, System.Text.UTF8Encoding.UTF8);
        String strLine = "";
        while (strLine != null)
        {
          strLine = fileReader.ReadLine();
          if (strLine != null && strLine.Length > 0)
            arrayList.Add(strLine.Split(','));
        }
      }
      finally
      {
        if (fileReader != null)
          fileReader.Close();
      }
      return arrayList;
    }
    public static String GetLocalizedName(object value)
    {
      if (value == null)
        return String.Empty;

      var strValue = value.ToString().Replace(" ", "");
      if (String.IsNullOrWhiteSpace(strValue))
        return String.Empty;

      var typeValue = value.GetType();
      if (typeValue == typeof(Int16) || typeValue == typeof(Int32) || typeValue == typeof(Int64))
        return strValue;
      //if (SpecialStrList.SpecialString.Contains(strValue))
      //  return strValue;
      //var statisticVar = SpecialStrList.SpecialStringDict.FirstOrDefault(p => p.Value == value.ToString()).Key;
      //if (!String.IsNullOrEmpty(statisticVar))
      //  strValue = statisticVar;

      var localizedName = String.Empty;
      if (typeValue.IsEnum)
        localizedName = GetAppResource(String.Format("strEnum_{0}_{1}", typeValue.Name, value));
      if (!String.IsNullOrWhiteSpace(localizedName))
        return localizedName;

      localizedName = GetAppResource("Common_" + strValue);
      if (!String.IsNullOrEmpty(localizedName))
        return localizedName;

      localizedName = GetAppResource("str" + strValue);
      if (!String.IsNullOrEmpty(localizedName))
        return localizedName;

#if DEBUG
      ExceptionHandler.ThrowException(String.Format("Can not find resource (Key={0})", strValue));
#endif
      return ConvertStringByCapitalChar(strValue);
    }
    public static String GetAppResource(String key)
    {
      var val = String.Empty;
      Resources.ResourceManager.IgnoreCase = true;
      val = Resources.ResourceManager.GetString(key, Resources.Culture);
      if (String.IsNullOrWhiteSpace(val))
        return string.Empty;

      val = val.Replace("\\r", "\r");
      val = val.Replace("\\n", "\n");
      val = val.Replace("\\t", "\t");
      return val;
    }

    public static SolidColorBrush ColorToBrush(Color color)
    {
      var brush = new SolidColorBrush(color);
      brush.Freeze();
      return brush;
    }
    public static Color BrushToColor(Brush brush)
    {
      var solidBrush = brush as SolidColorBrush;
      if (solidBrush == null)
      {
        var gradientBrush = brush as GradientBrush;
        if (gradientBrush == null)
          return Colors.Transparent;

        var collection = (GradientStopCollection)gradientBrush.SafeGetValue(GradientBrush.GradientStopsProperty);
        if (collection != null)
        {
          GradientStop gs = collection.SafeGetAt(collection.SafeGetCount() - 1);
          if (gs != null)
            return (Color)gs.SafeGetValue(GradientStop.ColorProperty);
        }
      }
      return (Color)solidBrush.SafeGetValue(SolidColorBrush.ColorProperty);
    }
    public static bool? ShowDialog(Window dlg)
    {
      return ShowDialog(dlg, true);
    }

    public static bool? ShowDialog(Window dlg, Boolean isDefaultPosition)
    {
      if (Application.Current != null && Application.Current.CheckAccess() == false)
      {
        bool? ret = null;
        Application.Current.Dispatcher.Invoke(new Action(() => ret = ShowDialog(dlg, isDefaultPosition)));
        return ret;
      }

      Debug.Assert(dlg != null);
      if (dlg == null)
        return null;
      bool? isEnabledOld = null;
      bool isTopmostOld = dlg.Topmost;
      if (IsBrowserApplication)
      {
        if (Application.Current != null && Application.Current.MainWindow != null)
        {
          isEnabledOld = OS.Win32.IsWindowEnabled(OS.Win32.GetActiveWindow());
          OS.Win32.EnableWindow(OS.Win32.GetActiveWindow(), false);

          if (isDefaultPosition)
            dlg.Loaded += new RoutedEventHandler(OnBrowserApplicationDialog_Loaded);
        }
        dlg.Topmost = true;
      }
      else
      {
        if (Application.Current != null && !dlg.Equals(Application.Current.MainWindow) && Application.Current.MainWindow.IsLoaded
          && Application.Current.MainWindow.Visibility == Visibility.Visible)
        {
          dlg.Owner = Application.Current.MainWindow;
          if (dlg.Icon == null)
            dlg.Icon = Application.Current.MainWindow.Icon;
        }
        if (isDefaultPosition)
          dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
      }

      dlg.ShowInTaskbar = false;  // model dialog should not show icon in the task bar.
      using (new CursorSetter(null))
      {
        WindowsManager.Current.AddDialog(dlg);
        bool? result = dlg.ShowDialog();
        WindowsManager.Current.RemoveDialog(dlg);

        if (isEnabledOld != null)
          OS.Win32.EnableWindow(OS.Win32.GetActiveWindow(), isEnabledOld ?? true);

        dlg.Loaded -= new RoutedEventHandler(OnBrowserApplicationDialog_Loaded);
        dlg.Topmost = isTopmostOld;

        return result;
      }
    }
    static void OnBrowserApplicationDialog_Loaded(object sender, RoutedEventArgs e)
    {
      Window dlg = sender as Window;
      if (dlg == null)
        return;
      if (!IsBrowserApplication)
        return;

      if (Application.Current != null && Application.Current.MainWindow != null)
      {
        Point orginalPoint = Application.Current.MainWindow.PointToScreen(new Point(0, 0));
        dlg.WindowStartupLocation = WindowStartupLocation.Manual;
        dlg.Left = orginalPoint.X + (Application.Current.MainWindow.Width - dlg.Width) / 2;
        dlg.Top = orginalPoint.Y + (Application.Current.MainWindow.Height - dlg.Height) / 2;
      }
    }

    public static Boolean IsNull(Nullable<Double> value)
    {
      return value == null || Double.IsNaN(value.Value);
    }
    public static Boolean IsNotNull(Nullable<Double> value)
    {
      return !IsNull(value);
    }
    public static Boolean IsNullOrZero(Nullable<Double> value)
    {
      return IsNull(value) || Math.Abs(value.Value) <= 0.0000000001;
    }
    public static Boolean IsNotNullOrZero(Nullable<Double> value)
    {
      return !IsNullOrZero(value);
    }
    public static Boolean IsNegative(Nullable<Double> value)
    {
      return IsNotNullOrZero(value) && value.Value < 0.0;
    }
    public static Int32 GetDecimals(Double value)
    {
      Int32 decimals = 0;
      while (IsNotNullOrZero(value % 1))
      {
        value *= 10;
        decimals++;
      }
      return decimals;
    }
    public static Boolean IsTransparent(Brush brush)
    {
      return IsTransparent(BrushToColor(brush));
    }
    public static Boolean IsTransparent(Color color)
    {
      return color == TransparentSolidBrush.Color || color == Colors.Transparent;
    }
    public static void RunAsAdministrator()
    {
      var wi = WindowsIdentity.GetCurrent();
      var wp = new WindowsPrincipal(wi);

      Boolean runAsAdmin = wp.IsInRole(WindowsBuiltInRole.Administrator);
      if (!runAsAdmin)
      {
        // It is not possible to launch a ClickOnce app as administrator directly,
        // so instead we launch the app as administrator in a new process.
        var processInfo = new ProcessStartInfo(Assembly.GetExecutingAssembly().CodeBase);

        // The following properties run the new process as administrator
        processInfo.UseShellExecute = true;
        processInfo.Verb = "runas";

        // Start the new process
        try
        {
          Process.Start(processInfo);
        }
        catch { }

        // Shut down the current process
        Environment.Exit(0);
      }
    }
    private static SortedDictionary<String, String> GetFontPathDictionary()
    {
      var dictionary = new SortedDictionary<String, String>();
      var regKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Fonts", false);
      var valueNames = regKey.GetValueNames();
      foreach (var name in valueNames)
      {
        var value = regKey.GetValue(name).ToString();
        if (value.Substring(1, 2).ToUpper() == @":\")
          continue;

        var ext = value.Substring(value.Length - 4).ToUpper();
        if (ext == ".TTF")
        {
          var key = name.Substring(0, name.Length - 11);
          if (!dictionary.ContainsKey(key))
            dictionary[key] = value;
        }
        else if (ext == ".TTC")
        {
          var ttc = new GlyphTypeface(new Uri(String.Format("{0}\\{1}", Environment.GetFolderPath(Environment.SpecialFolder.Fonts), value)));
          if (ttc != null)
          {
            for (int i = 0; i < ttc.FamilyNames.Values.Count; i++)
            {
              var key = ttc.FamilyNames.Values.ElementAt(i);
              if (!dictionary.ContainsKey(key))
                dictionary[key] = String.Format("{0},{1}", value, i);
            }
          }
        }
      }
      regKey.Close();
      return dictionary;
    }
    public static Boolean WaitForFileCompleted(String filePath, Boolean waitIfFileNotExist = true, int maxWaitMinutes = 30)
    {
      try
      {
        if (!File.Exists(filePath) && !waitIfFileNotExist)
          return false;

        var sleepTime = 2000;
        var maxLoopCount = (int)decimal.Ceiling(maxWaitMinutes * 60 * 1000 / sleepTime);

        int count = 0;
        var isCompleted = false;
        do
        {
          count++;
          Thread.Sleep(sleepTime);

          if (!File.Exists(filePath))
            continue;
          if (IsFileBeingUsed(filePath))
            continue;

          isCompleted = true;
        } while (!isCompleted && count <= maxLoopCount);
        return isCompleted;
      }
      catch (Exception ex)
      {
        ExceptionHandler.ThrowException(ex);
        return false;
      }
    }

    private static SortedDictionary<String, String> _fontPathDictionary;
    public static SortedDictionary<String, String> FontPathDictionary
    {
      get
      {
        if (_fontPathDictionary == null)
          _fontPathDictionary = GetFontPathDictionary();
        return _fontPathDictionary;
      }
    }

    private static List<String> _systemFontNames;
    public static List<String> SystemFontNames
    {
      get
      {
        if (_systemFontNames == null)
          _systemFontNames = Fonts.SystemFontFamilies.Select(p => p.ToString()).ToList();
        return _systemFontNames;
      }
    }












    private static SolidColorBrush _transparentSolidBrush;
    public static SolidColorBrush TransparentSolidBrush
    {
      get
      {
        if (_transparentSolidBrush == null)
        {
          try
          {
            _transparentSolidBrush = (SolidColorBrush)Application.Current.Resources["TransparentBrush"];
          }
          catch
          {
            _transparentSolidBrush = Brushes.Transparent;
          }
        }
        return _transparentSolidBrush;
      }
    }

    public static bool IsBrowserApplication
    {
      get { return System.Windows.Interop.BrowserInteropHelper.IsBrowserHosted; }
    }
    public static String CurrentDirectory
    {
      get { return System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); }
    }
    public static String AppDataDirectory
    {
      get
      {
        var ret = String.Empty;
        if (IsWindows2000 || IsWindowsXP || IsWindows2003)
          ret = String.Format("{0}\\PdfPrinter", Environment.GetEnvironmentVariable("ALLUSERSPROFILE") + "\\Documents\\Zehong");
        else
          ret = String.Format("{0}\\PdfPrinter", Environment.GetEnvironmentVariable("PUBLIC") + "\\Documents\\Zehong");
        ForceCreateDirectory(ret);
        return ret;
      }
    }

    public static Boolean IsWindows2000
    {
      get { return (Environment.OSVersion.Platform == PlatformID.Win32NT) && (Environment.OSVersion.Version.Major == 5) && (Environment.OSVersion.Version.Minor == 0); }
    }
    public static Boolean IsWindowsXP
    {
      get { return (Environment.OSVersion.Platform == PlatformID.Win32NT) && (Environment.OSVersion.Version.Major == 5) && (Environment.OSVersion.Version.Minor == 1); }
    }
    public static Boolean IsWindows2003
    {
      get { return (Environment.OSVersion.Platform == PlatformID.Win32NT) && (Environment.OSVersion.Version.Major == 5) && (Environment.OSVersion.Version.Minor == 2); }
    }
    public static Boolean IsWindowsVista
    {
      get { return (Environment.OSVersion.Platform == PlatformID.Win32NT) && (Environment.OSVersion.Version.Major == 6) && (Environment.OSVersion.Version.Minor == 0); }
    }
    public static Boolean IsWindows7
    {
      get { return (Environment.OSVersion.Platform == PlatformID.Win32NT) && (Environment.OSVersion.Version.Major == 6) && (Environment.OSVersion.Version.Minor == 1); }
    }
    public static Boolean IsWindows8
    {
      get { return (Environment.OSVersion.Platform == PlatformID.Win32NT) && (Environment.OSVersion.Version.Major == 6) && (Environment.OSVersion.Version.Minor == 2); }
    }



    public static readonly Color Red = ("#9FFF0000").SafeConvertInvariantStringTo<Color>();
    public static readonly Color Yellow = ("#9FFFFF00").SafeConvertInvariantStringTo<Color>();
    public static readonly Color Green = ("#9F00FF00").SafeConvertInvariantStringTo<Color>();
    public static readonly Color Purple = ("#9F0000FF").SafeConvertInvariantStringTo<Color>();
    public static readonly Brush BrushRed = ("#9FFF0000").SafeConvertInvariantStringTo<Brush>();
    public static readonly Brush BrushYellow = ("#9FFFFF00").SafeConvertInvariantStringTo<Brush>();
    public static readonly Brush BrushGreen = ("#9F00FF00").SafeConvertInvariantStringTo<Brush>();
    public static readonly Brush BrushPurple = ("#9F0000FF").SafeConvertInvariantStringTo<Brush>();
  }
}
