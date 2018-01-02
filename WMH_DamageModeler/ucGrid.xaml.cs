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

namespace WMH_DamageModeler
{
    /// <summary>
    /// Interaction logic for ucGrid.xaml
    /// </summary>
    public partial class ucGrid : UserControl, iUCDamageGrid
    {
        public ucGrid()
        {
            InitializeComponent();
        }

        public DamageGrid GetGrid()
        {
            var g_return = new DamageGrid();

            foreach (var box in grdBoxes.Children.OfType<ucBox>())
            {
                var DamageBox = new Box();
                DamageBox.State = box.BoxState;
                DamageBox.System = box.BoxSystem;
                DamageBox.Index = Grid.GetRow(box) - 1;
                DamageBox.Column = new List<int>() { Grid.GetColumn(box) - 1 };
                g_return.Boxes.Add(DamageBox);
            }

            return g_return;
        }

        public void LoadGrid(DamageGrid DamageGrid)
        {
            foreach (var box in grdBoxes.Children.OfType<ucBox>())
            {
                var Column = Grid.GetColumn(box) - 1;
                var Index = Grid.GetRow(box) - 1;

                var GridBox = (from p in DamageGrid.SortedBoxes where p.Column.Contains(Column) && p.Index == Index select p).FirstOrDefault();
                if (GridBox != null)
                {
                    while (box.BoxState != GridBox.State)
                        box.Toggle();

                    box.BoxSystem = GridBox.System;
                }

            }
        }
    }
}
