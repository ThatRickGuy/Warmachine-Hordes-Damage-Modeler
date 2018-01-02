using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WMH_DamageModeler
{
    public class Defender : INotifyPropertyChanged
    {

        public int ChargeDefense { get; set; }
        private int _Defense;
        public int Defense
        {
            get
            {
                return _Defense;
            }
            set
            {
                if (value != this._Defense)
                {
                    this._Defense = value;
                    NotifyPropertyChanged("Defense");
                }
            }
        }
        public bool IsAutoHitInMelee { get; set; }
        public bool OneLessAttackDie { get; set; }
        public bool IsAdd1DropHighestDefense { get; set; }
        public bool IsImmuneToKnockDown { get; set; }
        public bool IsImmuneToStationary { get; set; }

        public int FieldGeneratorShieldBoxes { get; set; }
        public string ArmorSystem { get; set; }
        public int ArmorSystemBonus { get; set; }
        public int ArmorBonus { get; set; }
        public int ArmorCharge { get; set; }
        public int Armor { get; set; }
        public bool OneLessDamageDie { get; set; }
        public bool IsAdd1DropHighestArmor { get; set; }

        public DamageGrid Grid;
        
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public Defender()
        {
            Grid = new DamageGrid();
        }

        public void Save(string FileName)
        {
            var fs = new FileStream(FileName, FileMode.Create);
            XmlSerializer x = new XmlSerializer(this.GetType());
            x.Serialize(fs, this);
            fs.Dispose();
        }

        public static Defender Load(string FileName)
        {
            Defender d_return = new Defender();

            var fs = new FileStream(FileName, FileMode.Open);
            XmlSerializer x = new XmlSerializer(d_return.GetType());
            try
            {
                d_return = (Defender)x.Deserialize(fs);
            }
            catch
            {
               d_return = null;
            }
            fs.Dispose();

            return d_return;
        }

    }
}
