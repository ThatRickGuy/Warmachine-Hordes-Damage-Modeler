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
using System.Windows.Shapes;

namespace WMH_DamageModeler
{
    /// <summary>
    /// Interaction logic for wReporting.xaml
    /// </summary>
    public partial class wReporting : Window
    {
        public wReporting()
        {
            InitializeComponent();
        }

        public void GenerateReport(List<List<AttackResolution>> Simulations, Attacker Attacker, Defender Defender)
        {
            int totalDamageDone = 0;
            int totalBoxesBoxed = 0;
            int totalDefendersWrecked = 0;
            int totalDefendersKnockedDown = 0;
            int totalCrippledSystemsCount = 0;

            var totalSystems = (from p in Defender.Grid.Boxes where p.State != BoxState.Unused select p.System).Distinct().Count();

            foreach (var Simulation in Simulations)
            {
                var BoxesChanged = new List<Box>();
                foreach (var attack in Simulation)
                {
                    totalDamageDone += attack.DamageDone;
                    if (attack.BoxesChanged != null)
                    {
                        totalBoxesBoxed += attack.BoxesChanged.Count();
                        BoxesChanged.AddRange(attack.BoxesChanged);
                    }
                }
                if ((from p in Simulation where p.BoxesChanged !=null  select p.BoxesChanged.Count()).Sum() >= (from p in Defender.Grid.Boxes where p.State != BoxState.Unused select p).Count() )
                    totalDefendersWrecked += 1;

                if ((from p in Simulation where p.DefenderIsKnockedDown  select p).Count() > 0)
                    totalDefendersKnockedDown += 1;
                
                var qBoxed = from pOriginal in Defender.Grid.Boxes
                             join pBoxed in BoxesChanged
                                on new { p1 = pOriginal.Column.First(), p2 = pOriginal.Index } equals new { p1 = pBoxed.Column.First(), p2 = pBoxed.Index }
                             select pOriginal;
                var UnboxedSystems = (from p in Defender.Grid.Boxes where !qBoxed.Contains(p) && p.System != string.Empty && p.State != BoxState.Unused  select p.System).Distinct().Count ();

                totalCrippledSystemsCount += totalSystems - UnboxedSystems;
            }
            decimal averageDamageDone = totalDamageDone / (decimal)Simulations.Count();
            decimal averageBoxesBoxed = totalBoxesBoxed / (decimal)Simulations.Count();
            decimal oddsOfWrecking = totalDefendersWrecked / (decimal)Simulations.Count();
            decimal oddsOfKnockingDown = totalDefendersKnockedDown / (decimal)Simulations.Count();
            decimal CrippledSystemsCountAverage = totalCrippledSystemsCount / (decimal)Simulations.Count();


            var report = "Average Damage Done: " + averageDamageDone.ToString("N02") + 
                         "\r\nAverage Boxes Boxed: " + averageBoxesBoxed.ToString("N02") +
                         "\r\nAverage Systems Destroyed: " + CrippledSystemsCountAverage.ToString("N02") + 
                         "\r\nOdds of Autohitting (KD/Sationary): " + oddsOfKnockingDown.ToString("P02") + 
                         "\r\nOdds of Wrecking: " + oddsOfWrecking.ToString("P02");
            this.txtReport.Text = report;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
