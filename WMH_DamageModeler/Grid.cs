using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMH_DamageModeler
{
    public class DamageGrid
    {
        private List<Box> _Grid;
        public DamageGrid()
        {
            _Grid = new List<Box>();
        }

        public GridType GridType { get; set; }

        public List<Box> Boxes { get {
                _SortedBoxes = null;
                return _Grid;
            } }

        private List<Box> _SortedBoxes;
        public List<Box> SortedBoxes
        { get
            {
                if (_SortedBoxes == null)
                    _SortedBoxes = (from p in _Grid orderby p.Column.First(), p.Index select p).ToList();
                return _SortedBoxes;
            } }

        private List<int> GetColumnList(int Col1, int? Col2)
        {
            var l_return = new List<int>();
            l_return.Add(Col1);
            if (Col2.HasValue) l_return.Add(Col2.Value );
            return l_return;
        }

        public List<Box> ApplyDamage(int Damage, int Column)
        {
            var l_return = new List<Box>();

            //find first box in target column
            int FirstBoxIndex = 0;
            for (int i = 0; i < SortedBoxes.Count(); i++)
            {
                if (SortedBoxes[i].Column.Contains(Column ))
                {
                    FirstBoxIndex = i;
                    break;
                }
            }
                        
            for (int i = 1; i<= Damage; i++)
            {
                var DamageApplied = false;
                //start at the top of the target column and go up!
                for (int j = FirstBoxIndex; j< SortedBoxes.Count; j++)
                {
                    if (SortedBoxes[j].State==BoxState.Open)
                    {
                        SortedBoxes[j].State = BoxState.Boxed;
                        l_return.Add(SortedBoxes[j]);
                        DamageApplied = true;
                        break;
                    }
                }

                if (!DamageApplied )
                {//No damage was applied, start looking for unmarked boxes at the begining
                    for (int j = 0; j < FirstBoxIndex; j++)
                    {
                        if (SortedBoxes[j].State == BoxState.Open)
                        {
                            SortedBoxes[j].State = BoxState.Boxed;
                            l_return.Add(SortedBoxes[j]);
                            DamageApplied = true;
                            break;
                        }
                    }
                }

                //No more damage was applied, target is Boxed!
                if (!DamageApplied)
                    break;
            }

            return l_return;
        }

        public List<Box> AmputateColumn(List<int> Column)
        {
            var l_return = new List<Box>();
            foreach (var box in (from p in SortedBoxes where (p.Column.SequenceEqual(Column) || (Column.Count==1 && p.Column.Contains(Column.First()) )) && p.State==BoxState.Open select p))
            {
                box.State = BoxState.Boxed;
                l_return.Add(box);
            }
            return l_return;
        }

        public int GetRemainingBoxCount()
        {
            var i_return = 0;
            i_return= (from p in SortedBoxes where p.State == BoxState.Open select p).Count();
            return i_return;
        }
    }
}

public class Box
{
    public List<int> Column { get; set; }
    public int Index { get; set; }
    public BoxState State;
    public string System;
}

public enum BoxState
{
    Unused,
    Open,
    Boxed
};

public enum GridType
{
    Grid,
    Spiral
};