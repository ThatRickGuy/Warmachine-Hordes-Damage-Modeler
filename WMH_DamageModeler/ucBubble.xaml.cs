using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace WMH_DamageModeler
{
    /// <summary>
    /// Interaction logic for ucBubble.xaml
    /// </summary>
    public partial class ucBubble : UserControl, INotifyPropertyChanged
    {
        public ucBubble()
        {
            InitializeComponent();
        }
        private bool _isOpen = false;

        public BoxState BoxState = BoxState.Unused;
        private string _BoxSystem = string.Empty;
        public string BoxSystem
        {
            get
            {
                return this._BoxSystem;
            }
            set
            {
                if (value != this._BoxSystem)
                {
                    this._BoxSystem = value;
                    NotifyPropertyChanged("BoxSystem");
                }
            }
        }
        protected void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;


        private void btnToggle_Click(object sender, RoutedEventArgs e)
        {
            Toggle();
        }

        public void Toggle()
        {
            if (_isOpen)
            {
                var g = (Grid)btnToggle.Template.FindName("grdOpen", btnToggle);
                g.Visibility = Visibility.Collapsed;
                g = (Grid)btnToggle.Template.FindName("grdClosed", btnToggle);
                g.Visibility = Visibility.Visible;
                BoxState = BoxState.Unused;
                BoxSystem = string.Empty;
            }
            else
            {
                var g = (Grid)btnToggle.Template.FindName("grdOpen", btnToggle);
                g.Visibility = Visibility.Visible;
                g = (Grid)btnToggle.Template.FindName("grdClosed", btnToggle);
                g.Visibility = Visibility.Collapsed;
                BoxState = BoxState.Open;
            }
            _isOpen = !_isOpen;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var x = (MenuItem)sender;
            BoxSystem = x.Header.ToString();
        }
    }
}
