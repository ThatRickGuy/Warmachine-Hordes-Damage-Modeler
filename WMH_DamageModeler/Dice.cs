using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMH_DamageModeler
{
    class Dice
    {
        private Random rnd;

        public Dice()
        {
            int? Seed = null;
#if debug
            Seed = 1;
#endif
            if (Seed.HasValue)
                rnd = new Random(Seed.Value);
            else
                rnd = new Random();
        }

        public RollResolution Roll(int NumberOfDice, bool Add1DropLowest, bool Add1DropHighest)
        {
            var r_Return = new RollResolution();
            List<int> Rolls = new List<int>();

            if (Add1DropHighest) NumberOfDice += 1;
            if (Add1DropLowest) NumberOfDice += 1;

            for (int i = 1; i<=NumberOfDice;i++)
                Rolls.Add(rnd.Next(1, 7));

            if (Add1DropHighest) Rolls.Remove((from p in Rolls orderby p select p).Last());
            if (Add1DropLowest) Rolls.Remove((from p in Rolls orderby p select p).First());

            foreach (var roll in Rolls)
            {
                if ((from p in Rolls where p == roll select p).Count() > 1)
                {
                    r_Return.IsCrit = true;
                }
                r_Return.TotalRoll += roll;
            }
            
            return r_Return;
        }
    }

    public class RollResolution
    {
        public int TotalRoll { get; set; }
        public bool IsCrit { get; set; }
    }
}
