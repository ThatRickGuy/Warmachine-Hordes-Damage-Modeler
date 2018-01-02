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
    /// Interaction logic for ucSpiral.xaml
    /// </summary>
    public partial class ucSpiral : UserControl, iUCDamageGrid
    {
        public ucSpiral()
        {
            InitializeComponent();
        }

        public DamageGrid GetGrid()
        {
            var g_return = new DamageGrid();

            foreach (var box in grdBoxes.Children.OfType<ucBubble>())
            {
                var DamageBox = new Box();
                DamageBox.State = box.BoxState;
                DamageBox.System = box.BoxSystem;
                //DamageBox.Column = new List<int>() { Grid.GetColumn(box) - 1 };
                DamageBox.Index = 12 - (14 - Grid.GetRow(box)) + 1;
                var col = Grid.GetColumn(box);
                switch (col)
                {
                    case 2: //1
                        DamageBox.Column = new List<int>() { 1 };
                        break;
                    case 3: //1-2
                        DamageBox.Column = new List<int>() { 1, 2 };
                        break;
                    case 4: //2
                        DamageBox.Column = new List<int>() { 2 };
                        break;
                    case 6: //3
                        DamageBox.Column = new List<int>() { 3 };
                        break;
                    case 7: //3-4
                        DamageBox.Column = new List<int>() { 3, 4 };
                        break;
                    case 8: //4
                        DamageBox.Column = new List<int>() { 4 };
                        break;
                    case 10: //5
                        DamageBox.Column = new List<int>() { 5 };
                        break;
                    case 11: //5-6
                        DamageBox.Column = new List<int>() { 5, 6 };
                        break;
                    case 12: //6
                        DamageBox.Column = new List<int>() { 6 };
                        break;
                }
                g_return.Boxes.Add(DamageBox);
            }

            return g_return;
        }

        public void LoadGrid(DamageGrid DamageGrid)
        {
            foreach (var bubble in grdBoxes.Children.OfType<ucBubble>())
            {
                var Column = Grid.GetColumn(bubble);
                var Index = Grid.GetRow(bubble);

                Column /= 2;
                Index -=1;

                var GridBox = (from p in DamageGrid.SortedBoxes where p.Column.Contains(Column) && p.Index == Index select p).FirstOrDefault();
                if (GridBox != null)
                {
                    while (bubble.BoxState != GridBox.State)
                        bubble.Toggle();

                    bubble.BoxSystem = GridBox.System;
                }

            }
        }
    }
}
