using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMH_DamageModeler
{
    interface iUCDamageGrid
    {
        DamageGrid GetGrid();

        void LoadGrid(DamageGrid DamageGrid);
    }
}
