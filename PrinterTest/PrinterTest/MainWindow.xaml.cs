using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Zehong.CSharp.Solution.PdfPrinter;

namespace PrinterTest
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
    }

    private void btnPrint_Click(object sender, RoutedEventArgs e)
    {
      var path = String.Format("{0}\\{1}.pdf",
        Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
        DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"));
      PdfPrinter.Print(new List<Canvas> { canvasObj }, path);
    }
  }
}
