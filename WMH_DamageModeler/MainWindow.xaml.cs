using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
using System.Windows.Threading;
using System.Xml.Serialization;

namespace WMH_DamageModeler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            _Attacker = new Attacker();
            _Defender = new Defender();
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);

            InitializeComponent();

            dispatcherTimer.Start();
        }


        private DispatcherTimer dispatcherTimer;

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            dispatcherTimer.Stop();
            tabGrid.SelectedIndex = 0;
        }

        private Attacker _Attacker;
        private Defender _Defender;

        public Attacker Attacker
        {
            get
            {
                return this._Attacker;
            }
            set
            {
                if (value != this._Attacker)
                {
                    this._Attacker = value;
                    NotifyPropertyChanged("Attacker");
                }
            }
        }

        public Defender Defender
        {
            get
            {
                return this._Defender;
            }
            set
            {
                if (value != this._Defender)
                {
                    this._Defender = value;
                    NotifyPropertyChanged("Defender");
                }
            }
        }

        protected void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void button_Click(object sender, RoutedEventArgs e)
        {
            var dm = new DamageModeler();

            int Iterations = 1;
            int.TryParse(txtIterations.Text, out Iterations);
            var Simulations = new List<List<AttackResolution>>();
            for (int i = 1; i <= Iterations; i++)
            {
                _Defender.Grid = ((iUCDamageGrid)((tabGrid.SelectedItem as TabItem).Content)).GetGrid();
                int Focus = 0;
                int.TryParse(txtFocus.Text, out Focus);
                _Defender.Focus = Focus;
                int Fury = 0;
                int.TryParse(txtFury.Text, out Fury);
                _Defender.Fury = Fury;
                var results = dm.ModelDamage(_Attacker, _Defender);

                Simulations.Add(results);
            }

            var report = new wReporting();
            report.GenerateReport(Simulations, _Attacker, _Defender);
            report.Show();
        }

        #region Add / Remove Attacks
        private void btnAddAttack_Click(object sender, RoutedEventArgs e)
        {
            foreach (var a in stkAttacks.Children.OfType<ucAttack>())
                a.Collapse();
            Attack attack;
            int WeaponNumber = 0;
            int.TryParse(((Button)sender).Content.ToString(), out WeaponNumber);


            var cloneAttack = (from p in _Attacker.Attacks where p.WeaponSystem == WeaponNumber orderby p.Sequence select p).FirstOrDefault();
            if (cloneAttack != null)
            {
                var ms = new MemoryStream();
                XmlSerializer x = new XmlSerializer(cloneAttack.GetType());
                x.Serialize(ms, cloneAttack);

                ms.Flush();
                ms.Position = 0;

                attack = (Attack)x.Deserialize(ms);
                Attacker.AddAttack(attack);
                ms.Dispose();
            }
            else
            {
                attack = _Attacker.AddAttack();
                attack.WeaponSystem = WeaponNumber;
            }
            var ucAttack = new ucAttack(attack);
            this.stkAttacks.Children.Add(ucAttack);
        }

        private void btnRemoveAttack_Click(object sender, RoutedEventArgs e)
        {
            var oc = MessageBox.Show("This will remove all expanded attacks, click OK to continue.", "Remove Attack", MessageBoxButton.OKCancel);
            if (oc == MessageBoxResult.OK)
            {
                var attackControls = this.stkAttacks.Children.OfType<ucAttack>().ToList();
                foreach (var attack in attackControls)
                {
                    if (attack.IsExpanded())
                    {
                        _Attacker.RemoveAttack(attack.Attack);
                        stkAttacks.Children.Remove(attack);
                    }
                }
            }
        }
        #endregion

        #region Save and Load
        private void btnSaveDefender_Click(object sender, RoutedEventArgs e)
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = "Defender XML files (*.dxml)|*.dxml";
            var response = sfd.ShowDialog();
            if (response.HasValue && response.Value)
            {
                Defender.Grid = ((iUCDamageGrid)((tabGrid.SelectedItem as TabItem).Content)).GetGrid();
                if (tabGrid.SelectedIndex == 0)
                    Defender.Grid.GridType = GridType.Grid;
                else
                    Defender.Grid.GridType = GridType.Spiral;
                Defender.Save(sfd.FileName);
            }
        }

        private void btnLoadDefender_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "Defender XML files (*.dxml)|*.dxml";
            ofd.RestoreDirectory = true;
            var response = ofd.ShowDialog();

            if (response.HasValue && response.Value)
            {
                Defender = Defender.Load(ofd.FileName);

                if (Defender.Grid.GridType == GridType.Grid)
                {
                    tabGrid.SelectedIndex = 0;
                    GridControl.LoadGrid(Defender.Grid);
                }
                else
                {
                    tabGrid.SelectedIndex = 1;
                    SpiralControl.LoadGrid(Defender.Grid);
                }
            }
        }

        private void btnSaveAttacker_Click(object sender, RoutedEventArgs e)
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = "Attacker XML files (*.axml)|*.axml";
            var response = sfd.ShowDialog(this);
            if (response.HasValue && response.Value)
            {
                Attacker.Save(sfd.FileName);
            }
        }

        private void btnLoadAttacker_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Attacker XML files (*.axml)|*.axml";
            ofd.RestoreDirectory = true;
            var response = ofd.ShowDialog();

            if (response.HasValue && response.Value)
            {
                stkAttacks.Children.Clear();
                Attacker = Attacker.Load(ofd.FileName);
                foreach (var attack in Attacker.Attacks)
                {
                    var ucAttack = new ucAttack(attack);
                    this.stkAttacks.Children.Add(ucAttack);
                }
            }
        }
        #endregion

        private void tabGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabGrid.SelectedIndex == 0)
                Defender.Grid.GridType = GridType.Grid;
            else
                Defender.Grid.GridType = GridType.Spiral;
        }
    }
}
