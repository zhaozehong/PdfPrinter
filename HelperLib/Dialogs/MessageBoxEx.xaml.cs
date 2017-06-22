using System;
using System.Windows;
using System.ComponentModel;

namespace Zehong.CSharp.Solution.HelperLib
{
  public partial class MessageBoxEx : Window, INotifyPropertyChanged
  {
    public MessageBoxEx()
    {
      InitializeComponent();
    }

    private void btnOk_Click(object sender, RoutedEventArgs e)
    {
      this.Close();
    }

    #region INotifyPropertyChanged Members
    public event PropertyChangedEventHandler PropertyChanged;
    private void SendPropertyChanged(String propertyName)
    {
      if (PropertyChanged != null)
        this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion

    public String Prompt
    {
      get { return _prompt; }
      set
      {
        if (_prompt != value)
        {
          _prompt = value;
          this.SendPropertyChanged("Prompt");
        }      }
    }
    public String Details
    {
      get { return _details; }
      set
      {
        if (_details != value)
        {
          _details = value;
          this.SendPropertyChanged("Details");
          runDetails.Text = value;
        }
      }
    }
    public Boolean HasDetails
    {
      get { return _hasDetails; }
      set
      {
        if(_hasDetails != value)
        {
          _hasDetails = value;
          this.SendPropertyChanged("HasDetails");
        }
      }
    }


    private String _prompt = null;
    private String _details = null;
    private Boolean _hasDetails = false;
  }
}

