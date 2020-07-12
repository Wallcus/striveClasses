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

        public abstract bool Reload(GameRun run, Sentient user);
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

        public override bool Reload(GameRun run, Sentient user)
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

        public string ammoTypeAltName
        {
            get; set;
        }

        public string AmmoTypeName
        {
            get; set;
        }

        public int AmmoTypeInt
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
            this.AmmoTypeInt = 0;
            this.AmmoTypeName = "Handgun";

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
            this.AmmoTypeInt = 0;
            this.AmmoTypeName = "Handgun";

            this.Name = "Gun";
        }

        public RangedWeapon(int baseDamage, int baseRange, int baseAccuracy, List<string> modeNames, List<int> modeAttacks, string ammoTypeName, int selectedMode, int clip, string name) : base(baseDamage, baseRange, baseAccuracy)
        {
            this.ModeNames = modeNames;
            this.ModeAttacks = modeAttacks;
            this.SelectedMode = selectedMode;
            this.ClipMax = clip;
            this.ClipCurrent = this.ClipMax;
            this.ClipAltMax = 0;
            this.ClipAltCurrent = 0;
            this.AltDmg = 0;
            this.AmmoTypeName = ammoTypeName;
            this.ammoTypeAltName = "Empty Name";

            this.Name = name;
        }

        public RangedWeapon(int baseDamage, int baseRange, int baseAccuracy, List<string> modeNames, List<int> modeAttacks, string ammoTypeName, int selectedMode, int clip, int clipAlt, int altDmg, string ammoTypeAltName, string name) : base(baseDamage, baseRange, baseAccuracy)
        {
            this.ModeNames = modeNames;
            this.ModeAttacks = modeAttacks;
            this.SelectedMode = selectedMode;
            this.ClipMax = clip;
            this.ClipCurrent = this.ClipMax;
            this.ClipAltMax = clipAlt;
            this.ClipAltCurrent = this.ClipAltMax;
            this.AltDmg = altDmg;
            this.AmmoTypeName = ammoTypeName;
            this.ammoTypeAltName = ammoTypeAltName;

            this.Name = name;
        }

        public RangedWeapon(RangedWeapon weapon) : this(weapon.BaseDamage, weapon.BaseRange, weapon.BaseAccuracy, weapon.ModeNames, weapon.ModeAttacks, weapon.AmmoTypeName, weapon.SelectedMode, weapon.ClipMax, weapon.ClipAltMax, weapon.AltDmg, weapon.ammoTypeAltName, weapon.Name)
        {
            this.ClipCurrent = weapon.ClipCurrent;
            this.ClipAltCurrent = weapon.ClipAltCurrent;
        }

        public override bool Reload(GameRun run, Sentient user)
        {
            bool hasReloaded = false;

            for(int i = 0; i < user.ItemInventory.Count(); i++)
            {
                if(this.ClipCurrent < this.ClipMax && user.ItemInventory[i] is Ammo && ((Ammo)(user.ItemInventory[i])).Type == this.AmmoTypeName)
                {
                    if(this.ClipMax - this.ClipCurrent <= ((Ammo)(user.ItemInventory[i])).StackCurrent)
                    {
                        ((Ammo)(user.ItemInventory[i])).StackCurrent -= this.ClipMax - this.ClipCurrent;
                        this.ClipCurrent = this.ClipMax;

                        hasReloaded = true;
                    }
                    else if(((Ammo)(user.ItemInventory[i])).StackCurrent > 0)
                    {
                        this.ClipCurrent += ((Ammo)(user.ItemInventory[i])).StackCurrent;
                        ((Ammo)(user.ItemInventory[i])).StackCurrent = 0;
                        user.ItemInventory.Remove(((Ammo)(user.ItemInventory[i])));

                        hasReloaded = true;
                    }
                }
            }

            Sentient.UpdateAmmo(run, this);

            return hasReloaded;
        }
    }
}