using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace striveClasses
{
    public abstract class Item
    {
        public string Name
        {
            get; set;
        }

        public int StackMax
        {
            get; set;
        }

        public int StackCurrent
        {
            get; set;
        }

        public Item()
        {
            this.Name = "Item";
            this.StackMax = this.StackCurrent = 1;
        }

        public Item(string name, int stack)
        {
            this.Name = name;
            this.StackMax = this.StackCurrent = stack;
        }

        public Item(string name, int stack, int stackCurrent)
        {
            this.Name = name;
            this.StackMax = stack;
            this.StackCurrent = stackCurrent;
        }

        public override string ToString()
        {
            return this.Name;
        }
    }

    public class Ammo : Item
    {
        public enum AmmoCaliber
        {
            Handgun = 0,
            Intermediate = 1,
            Heavy = 2,
            EnergyPack = 3,
            Shotgun = 4,
            Explosive = 5
        }

        public string Type
        {
            get; set;
        }

        public Ammo(string name, int stack, int stackCurrent, int ammoCaliber) : base(name, stack, stackCurrent)
        {
            this.Type = ((AmmoCaliber)ammoCaliber).ToString();
        }
    }

    public abstract class Weapon : Item
    {
        public int SelectedMode
        {
            get; set;
        }

        public List<string> ModeNames
        {
            get; set;
        }

        public List<int> ModeAttacks
        {
            get; set;
        }

        public List<string> ModeAmmoType
        {
            get; set;
        }

        public int BaseDamage
        {
            get; set;
        }

        public int BaseRange
        {
            get; set;
        }

        /*
        // bleeding, piercing ...
        public string DamageType
        {
            get; set;
        }
        */

        public int BaseAccuracy
        {
            get; set;
        }

        public Weapon(int baseDamage, int baseRange, int baseAccuracy)
        {
            this.BaseDamage = baseDamage;
            this.BaseRange = baseRange;
            this.BaseAccuracy = baseAccuracy;
            //intitialize lists
            this.ModeNames = new List<string>();
            this.ModeAttacks = new List<int>();
            this.ModeAmmoType = new List<string>();
        }

        public abstract void Reload(GameRun run);
    }

    public class MeleeWeapon : Weapon
    {
        /*
        perry?
        just the stat, implement logic at attack
        */

        public MeleeWeapon() : base(25, 2, 75)
        {
            this.ModeNames.Add("Melee");
            this.ModeAttacks.Add(1);
            this.SelectedMode = 0;
            this.Name = "Knife";
        }

        public MeleeWeapon(int baseDamage, int baseAccuracy, string name) : base(baseDamage, 2, baseAccuracy)
        {
            this.ModeNames.Add("Melee");
            this.ModeAttacks.Add(1);
            this.SelectedMode = 0;
            this.Name = name;
        }

        public override void Reload(GameRun run)
        {
            //fix (or dont, melee should not be reloading, generally speaking)
            throw new NotImplementedException();
        }
    }

    public class RangedWeapon : Weapon
    {
        public int ClipMax
        {
            get; set;
        }

        public int ClipCurrent
        {
            get; set;
        }

        public int ClipAltMax
        {
            get; set;
        }

        public int ClipAltCurrent
        {
            get; set;
        }

        public int AltDmg
        {
            get; set;
        }

        public RangedWeapon() : base(25, 5, 90)
        {
            this.ModeNames.Add("Semi");
            this.ModeAttacks.Add(1);
            this.SelectedMode = 0;
            this.ClipMax = 10;
            this.ClipCurrent = this.ClipMax;
            this.ClipAltMax = 1;
            this.ClipAltCurrent = this.ClipAltMax;
            this.AltDmg = this.BaseDamage;


            this.Name = "Gun";
        }

        public RangedWeapon(int baseDamage, int baseRange, int baseAccuracy, int clip) : base(baseDamage, baseRange, baseAccuracy)
        {
            this.ModeNames.Add("Semi");
            this.ModeAttacks.Add(1);
            this.SelectedMode = 0;
            this.ClipMax = clip;
            this.ClipCurrent = ClipMax;
            this.ClipAltMax = 1;
            this.ClipAltCurrent = this.ClipAltMax;
            this.AltDmg = this.BaseDamage;

            this.Name = "Gun";
        }

        //delete this constructor when ammo use is implemented. it should not be used.
        public RangedWeapon(int baseDamage, int baseRange, int baseAccuracy, List<string> modeNames, List<int> modeAttacks, int selectedMode, int clip, string name) : base(baseDamage, baseRange, baseAccuracy)
        {
            this.ModeNames = modeNames;
            this.ModeAttacks = modeAttacks;
            this.SelectedMode = selectedMode;
            this.ClipMax = clip;
            this.ClipCurrent = this.ClipMax;
            this.ClipAltMax = 5;
            this.ClipAltCurrent = this.ClipAltMax;
            this.AltDmg = this.BaseDamage;

            this.Name = name;
        }

        public RangedWeapon(int baseDamage, int baseRange, int baseAccuracy, List<string> modeNames, List<int> modeAttacks, List<string> modeAmmoTypes, int selectedMode, int clip, int clipAlt, string name) : base(baseDamage, baseRange, baseAccuracy)
        {
            this.ModeNames = modeNames;
            this.ModeAttacks = modeAttacks;
            this.ModeAmmoType = modeAmmoTypes;
            this.SelectedMode = selectedMode;
            this.ClipMax = clip;
            this.ClipCurrent = this.ClipMax;
            this.ClipAltMax = clipAlt;
            this.ClipAltCurrent = this.ClipAltMax;
            this.AltDmg = this.BaseDamage;

            this.Name = name;
        }

        public RangedWeapon(int baseDamage, int baseRange, int baseAccuracy, List<string> modeNames, List<int> modeAttacks, List<string> modeAmmoTypes, int selectedMode, int clip, int clipAlt, int altDmg, string name) : base(baseDamage, baseRange, baseAccuracy)
        {
            this.ModeNames = modeNames;
            this.ModeAttacks = modeAttacks;
            this.ModeAmmoType = modeAmmoTypes;
            this.SelectedMode = selectedMode;
            this.ClipMax = clip;
            this.ClipCurrent = this.ClipMax;
            this.ClipAltMax = clipAlt;
            this.ClipAltCurrent = this.ClipAltMax;
            this.AltDmg = altDmg;

            this.Name = name;
        }

        public RangedWeapon(RangedWeapon weapon) : this(weapon.BaseDamage, weapon.BaseRange, weapon.BaseAccuracy, weapon.ModeNames, weapon.ModeAttacks, weapon.SelectedMode, weapon.ClipMax, weapon.Name)
        {
            this.ClipCurrent = weapon.ClipCurrent;
        }

        public override void Reload(GameRun run)
        {
            //fix to check inventory for ammo
            this.ClipCurrent = this.ClipMax;

            Sentient.UpdateAmmo(run, this);
        }
    }
}