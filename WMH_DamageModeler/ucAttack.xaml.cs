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
    /// Interaction logic for ucAttack.xaml
    /// </summary>
    public partial class ucAttack : UserControl, INotifyPropertyChanged
    {
        public ucAttack(Attack attack)
        {
            _Attack = attack;
            InitializeComponent();
        }

        private Attack _Attack;
        public Attack Attack
        {
            get
            {
                return this._Attack;
            }
            set
            {
                if (value != this._Attack)
                {
                    this._Attack = value;
                    NotifyPropertyChanged("Attack");
                }
            }
        }

        protected void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool IsExpanded()
        {
            var b_return = false;

            if (grdAttackDetails.Visibility == Visibility.Visible)
                b_return = true;

            return b_return;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void btnVisibility_Click(object sender, RoutedEventArgs e)
        {
            if (grdAttackDetails.Visibility == Visibility.Visible)
            {
                Collapse();
            }
            else
            {
                grdAttackDetails.Visibility = Visibility.Visible;
                btnVisibility.Content = "-";
            }
        }

        public void Collapse()
        {
            grdAttackDetails.Visibility = Visibility.Collapsed;
            btnVisibility.Content = "+";
        }

        private void chkIsCharge_Checked(object sender, RoutedEventArgs e)
        {
            if (chkIsCharge.IsChecked.Value && this.chkDamageRollBoosted != null )  this.chkDamageRollBoosted.IsChecked = true;
        }
    }
}
