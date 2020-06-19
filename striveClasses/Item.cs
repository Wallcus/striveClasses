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

        public int Stack
        {
            get; set;
        }

        public Item()
        {
            this.Name = "Item";
            this.Stack = 1;
        }

        public Item(string name, int stack)
        {
            this.Name = name;
            this.Stack = stack;
        }

        public override string ToString()
        {
            return this.Name;
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

        public MeleeWeapon(int baseDamage, int baseAccuracy, string name) : base(baseDamage, 1, baseAccuracy)
        {
            this.ModeNames.Add("Melee");
            this.ModeAttacks.Add(1);
            this.SelectedMode = 0;
            this.Name = name;
        }

        public override void Reload(GameRun run)
        {
            //fix
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

        public RangedWeapon() : base(25, 5, 90)
        {
            this.ModeNames.Add("Semi");
            this.ModeAttacks.Add(1);
            this.SelectedMode = 0;
            this.ClipMax = 10;
            this.ClipCurrent = this.ClipMax;

            this.Name = "Gun";
        }

        public RangedWeapon(int baseDamage, int baseRange, int baseAccuracy, int clip) : base(baseDamage, baseRange, baseAccuracy)
        {
            this.ModeNames.Add("Semi");
            this.ModeAttacks.Add(1);
            this.SelectedMode = 0;
            this.ClipMax = clip;
            this.ClipCurrent = ClipMax;

            this.Name = "Gun";
        }

        public RangedWeapon(int baseDamage, int baseRange, int baseAccuracy, List<string> modeNames, List<int> modeAttacks, int selectedMode, int clip, string name) : base(baseDamage, baseRange, baseAccuracy)
        {
            this.ModeNames = modeNames;
            this.ModeAttacks = modeAttacks;
            this.SelectedMode = selectedMode;
            this.ClipMax = clip;
            this.ClipCurrent = this.ClipMax;
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