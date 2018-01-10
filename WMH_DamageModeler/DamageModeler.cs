using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMH_DamageModeler
{
    class DamageModeler
    {
        private Dice Dice;

        public DamageModeler()
        {
            Dice = new Dice();
        }

        public List<AttackResolution> ModelDamage(Attacker Attacker, Defender Defender)
        {
            var l_return = new List<AttackResolution>();

            var IsAutoHitting = false;
            var IsAutoHittingOnWeapon = new List<int>();

            var FieldGeneratorShieldBoxes = Defender.FieldGeneratorShieldBoxes;

            foreach (var Attack in Attacker.Attacks)
            {
                var AR = new AttackResolution();
                l_return.Add(AR);

                //try to hit
                if (Attack.AutoHits || IsAutoHitting || IsAutoHittingOnWeapon.Contains(Attack.WeaponSystem) || (Defender.IsAutoHitInMelee && Attack.IsMeleeAttack))
                    AR.AttackResult = AttackRollResults.Hit;
                else
                {
                    AR.MAT = Attacker.MAT + Attack.AttackRollBonus;

                    AR.AttackDice = 2;
                    if (Attack.AttackRollBoosted) AR.AttackDice += 1;
                    AR.AttackDice += Attack.AttackRollAdditionalDice;

                    AR.DEF = Defender.Defense;
                    if (Attack.IsCharge && Defender.ChargeDefense > Defender.Defense) AR.DEF = Defender.ChargeDefense;
                    if (Defender.OneLessAttackDie) AR.AttackDice -= 1;

                    var q = (from p in Defender.Grid.Boxes where p.System == "Movement" select p);
                    if (q.Count() > 0)
                    {
                        q = (from p in q where p.State == BoxState.Open select p);
                        if (q.Count() == 0) AR.DEF = 5;
                    }

                    var DieRoll = Dice.Roll(AR.AttackDice, Attack.IsAdd1DropLowestAttack, Defender.IsAdd1DropHighestDefense);
                    if (DieRoll.TotalRoll > AR.AttackDice)
                    {
                        if (DieRoll.TotalRoll + AR.MAT > AR.DEF && DieRoll.IsCrit)
                        {
                            AR.AttackResult = AttackRollResults.Crit;
                            if (Attack.SustainedAttack || Attack.SustainedAttackCrit)
                                IsAutoHittingOnWeapon.Add(Attack.WeaponSystem);
                            if (((Attack.Stationary || Attack.StationaryCrit) && !Defender.IsImmuneToStationary) || ((Attack.KnockDown || Attack.KnockDownCrit) && !Defender.IsImmuneToKnockDown))
                            {
                                AR.DefenderIsKnockedDown = true;
                                IsAutoHitting = true;
                            }
                        }
                        else if (DieRoll.TotalRoll + AR.MAT > AR.DEF)
                        {
                            AR.AttackResult = AttackRollResults.Hit;
                            if (Attack.SustainedAttack)
                                IsAutoHittingOnWeapon.Add(Attack.WeaponSystem);
                            if (Attack.Stationary && !Defender.IsImmuneToStationary)
                                IsAutoHitting = true;
                            if (Attack.KnockDown && !Defender.IsImmuneToKnockDown)
                                IsAutoHitting = true;
                        }
                        else
                        {
                            if (DieRoll.TotalRoll > 6 && DieRoll.TotalRoll == AR.AttackDice * 6)
                            {
                                AR.AttackResult = AttackRollResults.Crit; // all 6s on more than 1 die always hits
                                if (Attack.SustainedAttack || Attack.SustainedAttackCrit)
                                    IsAutoHittingOnWeapon.Add(Attack.WeaponSystem);
                                if (((Attack.Stationary || Attack.StationaryCrit) && !Defender.IsImmuneToStationary) || ((Attack.KnockDown || Attack.KnockDownCrit) && !Defender.IsImmuneToKnockDown))
                                {
                                    AR.DefenderIsKnockedDown = true;
                                    IsAutoHitting = true;
                                }
                            }
                            else
                                AR.AttackResult = AttackRollResults.Miss;
                        }
                    }
                    else
                        AR.AttackResult = AttackRollResults.Miss; // all 1s always misses

                }

                // if hit, try to damage
                if (AR.AttackResult == AttackRollResults.Hit || AR.AttackResult == AttackRollResults.Crit)
                {
                    AR.PS = Attack.PS;
                    AR.PS += Attack.DamageRollBonus;
                    AR.DamageDice = 2;
                    AR.DamageDice += Attack.DamageRollAdditionalDice;
                    if (Attack.DamageRollBoosted) AR.DamageDice += 1;
                    if (Defender.OneLessDamageDie) AR.DamageDice -= 1;
                    //Brutal
                    if ((AR.AttackResult == AttackRollResults.Crit && Attack.BrutalCrit) || Attack.Brutal)
                        AR.DamageDice += 1;

                    AR.ARM = Defender.Armor;
                    //Armor Piercing
                    if ((AR.AttackResult == AttackRollResults.Crit && Attack.ArmorPiercingCrit) || Attack.ArmorPiercing)
                        AR.ARM = (int)Math.Ceiling(((decimal)AR.ARM / 2));

                    //Armor bonuses
                    if (Attack.IsCharge && Defender.ArmorCharge > Defender.Armor)
                        AR.ARM += Defender.ArmorCharge - Defender.Armor;
                    AR.ARM += Defender.ArmorBonus;

                    //System dependent armor bonus (shield)
                    if (!Attack.IsChainWeapon)
                    {
                        if (Defender.ArmorSystem != null) Defender.ArmorSystem = Defender.ArmorSystem.Replace("System.Windows.Controls.ComboBoxItem: ", string.Empty);
                        if (Defender.ArmorSystem != string.Empty && (from p in Defender.Grid.Boxes where p.State == BoxState.Open && p.System == Defender.ArmorSystem select p).Count() > 0)
                            AR.ARM += Defender.ArmorSystemBonus;
                    }

                    AR.StartingColumn = Dice.Roll(1, false, false).TotalRoll;
                    AR.DamageDone = Dice.Roll(AR.DamageDice, Attack.IsAdd1DropLowestDamage, Defender.IsAdd1DropHighestArmor).TotalRoll + AR.PS - AR.ARM;

                    // Decap
                    if ((AR.AttackResult == AttackRollResults.Crit && Attack.DecapatationCrit) || Attack.Decapatation)
                        AR.DamageDice *= 2;

                    if (AR.DamageDone < 0) AR.DamageDone = 0;

                    // Focus
                    if (AR.DamageDone >= 5 && Defender.Focus > 0)
                    {
                        AR.DamageDone -= 5;
                        Defender.Focus -= 1;
                    }


                    // Fury
                    if (AR.DamageDone > Defender.Grid.GetRemainingBoxCount() && Defender.Fury > 0)
                    {
                        AR.DamageDone = 0;
                        Defender.Fury -= 1;
                    }


                    var DamageRemaining = AR.DamageDone;
                    var DamageAbsorbedByFieldGeneratorShield = FieldGeneratorShieldBoxes - DamageRemaining;
                    if (DamageAbsorbedByFieldGeneratorShield < 0) DamageAbsorbedByFieldGeneratorShield = FieldGeneratorShieldBoxes;
                    FieldGeneratorShieldBoxes -= DamageAbsorbedByFieldGeneratorShield;
                    DamageRemaining -= DamageAbsorbedByFieldGeneratorShield;

                    AR.BoxesChanged = Defender.Grid.ApplyDamage(DamageRemaining, AR.StartingColumn);


                    // Amputation (after damage)
                    if ((AR.AttackResult == AttackRollResults.Crit && Attack.AmputationCrit) || Attack.Amputation)
                    {
                        if (AR.BoxesChanged.Count() > 0) AR.BoxesChanged.AddRange(Defender.Grid.AmputateColumn(AR.BoxesChanged.Last().Column));
                    }

                    // Fire
                    if ((AR.AttackResult == AttackRollResults.Crit && Attack.ContinuousFireCrit) || Attack.ContinuousFire)
                    {
                        AR.DefenderIsOnFire = true;
                    }

                    //Corrosion
                    if ((AR.AttackResult == AttackRollResults.Crit && Attack.ContinuousCorrosionCrit) || Attack.ContinuousCorrosion)
                    {
                        AR.DefenderIsCorroded = true;
                    }


                }

            }

            // Fire
            if ((from p in l_return where p.DefenderIsOnFire select p).Count() > 0)
                if (Dice.Roll(1, false, false).TotalRoll > 2)
                {
                    //POW 12 damage roll
                    //This is overly simplified, it goes by the same ARM that the attack was made at, so it will be inaccurate if the model 
                    //has buffs to ARM against enemy models/attacks only. 
                    var AR = new AttackResolution();
                    AR.DamageDone = 12 + Dice.Roll(2, false, false).TotalRoll - AR.ARM;
                    AR.BoxesChanged = Defender.Grid.ApplyDamage(AR.DamageDone, Dice.Roll(1, false, false).TotalRoll);
                    l_return.Add(AR);
                }

            // Corrosion 
            if ((from p in l_return where p.DefenderIsCorroded select p).Count() > 0)
                if (Dice.Roll(1, false, false).TotalRoll > 2)
                {
                    //1 point of damage
                    var AR = new AttackResolution();
                    AR.DamageDone = 1;
                    AR.BoxesChanged = Defender.Grid.ApplyDamage(AR.DamageDone, Dice.Roll(1, false, false).TotalRoll);
                    l_return.Add(AR);
                }


            return l_return;
        }



    }


    public enum AttackRollResults
    {
        Miss,
        Hit,
        Crit
    }

    public class AttackResolution
    {
        public int MAT { get; set; }
        public int PS { get; set; }
        public int AttackDice { get; set; }
        public int DamageDice { get; set; }
        public int DEF { get; set; }
        public int ARM { get; set; }
        public AttackRollResults AttackResult;
        public int DamageDone { get; set; }
        public int StartingColumn { get; set; }
        public List<Box> BoxesChanged { get; set; }
        public bool DefenderIsKnockedDown { get; set; }
        public bool DefenderIsOnFire { get; set; }
        public bool DefenderIsCorroded { get; set; }
    }
}
