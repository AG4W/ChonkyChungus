using UnityEngine;

using MoonSharp.Interpreter;

[MoonSharpUserData]
public class Weapon : Holdable
{
    public int minDamage { get; private set; }
    public int maxDamage { get; private set; }

    public int range { get; private set; }
    public int attackCost { get; private set; }

    public WeaponType weaponType { get; private set; }

    public Weapon(string name, string flavor, ItemRarity rarity, AnimationSet onehandedAnimationSet, GameObject prefab, bool requiresBothHands, bool useLeftHandIK, WeaponType weaponType) : base(name, flavor, rarity, EquipSlot.RightHandItem, prefab, onehandedAnimationSet, requiresBothHands, useLeftHandIK)
    {
        this.weaponType = weaponType;

        SetDefaultDamage();
        SetDefaultRange();
        SetDefaultCost();
    }

    void SetDefaultDamage()
    {
        switch (this.weaponType)
        {
            case WeaponType.OnehandedSword:
                this.minDamage = 1;
                this.maxDamage = 3;
                break;
            case WeaponType.TwohandedSword:
                this.minDamage = 1;
                this.maxDamage = 4;
                break;
            case WeaponType.OnehandedBlunt:
                this.minDamage = 2;
                this.maxDamage = 3;
                break;
            case WeaponType.TwohandedBlunt:
                this.minDamage = 2;
                this.maxDamage = 5;
                break;
            case WeaponType.Polearm:
                this.minDamage = 1;
                this.maxDamage = 3;
                break;
            case WeaponType.Ranged:
                this.minDamage = 1;
                this.maxDamage = 3;
                break;
            default:
                this.minDamage = 1;
                this.maxDamage = 2;
                break;
        }
    }
    void SetDefaultRange()
    {
        switch (this.weaponType)
        {
            case WeaponType.OnehandedSword:
                this.range = 1;
                break;
            case WeaponType.TwohandedSword:
                this.range = 2;
                break;
            case WeaponType.OnehandedBlunt:
                this.range = 1;
                break;
            case WeaponType.TwohandedBlunt:
                this.range = 2;
                break;
            case WeaponType.Polearm:
                this.range = 3;
                break;
            case WeaponType.Ranged:
                this.range = 30;
                break;
            default:
                this.range = 1;
                break;
        }
    }
    void SetDefaultCost()
    {
        switch (this.weaponType)
        {
            case WeaponType.OnehandedSword:
                this.attackCost = 2;
                break;
            case WeaponType.TwohandedSword:
                this.attackCost = 3;
                break;
            case WeaponType.OnehandedBlunt:
                this.attackCost = 3;
                break;
            case WeaponType.TwohandedBlunt:
                this.attackCost = 4;
                break;
            case WeaponType.Polearm:
                this.attackCost = 2;
                break;
            case WeaponType.Ranged:
                this.attackCost = 4;
                break;
            default:
                this.attackCost = 1;
                break;
        }
    }

    public int GetDamageRoll()
    {
        return Synched.Next(this.minDamage, this.maxDamage + 1);
    }

    public override string ToTooltip()
    {
        string s = base.BaseInfoToString();

        s += "\n\nDamage: " + minDamage + " - " + maxDamage;

        if (requiresBothHands)
            s += "\n\n<color=orange>Requires both hands.\n</color>";

        s += base.ActionsToString();

        return s;
    }
}
public enum WeaponType
{
    OnehandedSword,
    TwohandedSword,
    OnehandedBlunt,
    TwohandedBlunt,
    Polearm,
    Ranged,
}
