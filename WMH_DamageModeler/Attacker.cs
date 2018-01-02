using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WMH_DamageModeler
{
    public class Attacker
    {
        public int MAT { get; set; }

        public List<Attack> Attacks;

        public Attacker()
        {
            Attacks = new List<Attack>();
        }

        public Attack AddAttack()
        {
            Attack attack = new Attack();
            if (Attacks.Count == 0)
                attack.Sequence = 1;
            else
                attack.Sequence = (from p in Attacks orderby p.Sequence descending select p.Sequence).First() + 1;
            Attacks.Add(attack);
            return attack;
        }

        public void AddAttack(Attack attack)
        {
            attack.Sequence = (from p in Attacks orderby p.Sequence descending select p.Sequence).First() + 1;
            Attacks.Add(attack);
        }

        public void RemoveAttack(Attack attack)
        {
            Attacks.Remove(attack);
        }

        public void Save(string FileName)
        {
            var fs = new FileStream(FileName, FileMode.Create);
            XmlSerializer x = new XmlSerializer(this.GetType());
            x.Serialize(fs, this);
            fs.Dispose();
        }

        public static Attacker Load(string FileName)
        {
            Attacker a_return = new Attacker();

            var fs = new FileStream(FileName, FileMode.Open);
            XmlSerializer x = new XmlSerializer(a_return.GetType());
            try
            {
                a_return = (Attacker)x.Deserialize(fs);
            }
            catch 
            {
                a_return = null;
            }

            fs.Dispose();
            return a_return;
        }

    }

    public class Attack
    {
        public int Sequence { get; set; }
        public int WeaponSystem { get; set; }

        public bool IsMeleeAttack { get; set; }
        public bool IsCharge { get; set; }
        public bool AttackRollBoosted { get; set; }
        public int AttackRollAdditionalDice { get; set; }
        public int AttackRollBonus { get; set; }

        public int PS { get; set; }
        public bool DamageRollBoosted { get; set; }
        public int DamageRollAdditionalDice { get; set; }
        public int DamageRollBonus { get; set; }

        public bool IsChainWeapon { get; set; }
        public bool IsAdd1DropLowestAttack { get; set; }
        public bool IsAdd1DropLowestDamage { get; set; }

        public bool ArmorPiercing { get; set; }
        public bool Decapatation { get; set; }
        public bool Amputation { get; set; }
        public bool Brutal { get; set; }
        public bool ArmorPiercingCrit { get; set; }
        public bool DecapatationCrit { get; set; }
        public bool AmputationCrit { get; set; }
        public bool BrutalCrit { get; set; }

        public bool AutoHits { get; set; }
        public bool SustainedAttack { get; set; }
        public bool SustainedAttackCrit { get; set; }

        public bool Stationary { get; set; }
        public bool StationaryCrit { get; set; }
        public bool KnockDown { get; set; }
        public bool KnockDownCrit { get; set; }

        public bool ContinuousFire { get; set; }
        public bool ContinuousFireCrit { get; set; }
        public bool ContinuousCorrosion { get; set; }
        public bool ContinuousCorrosionCrit { get; set; }
    }
}
