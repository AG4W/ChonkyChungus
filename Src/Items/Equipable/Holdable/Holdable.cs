using UnityEngine;

using MoonSharp.Interpreter;

[MoonSharpUserData]
public class Holdable : Equipable
{
    public AnimationSet animationSet { get; private set; }

    public bool requiresBothHands { get; private set; }
    public bool useLeftHandIK { get; private set; }

    public Holdable(string name, string flavor, ItemRarity rarity, EquipSlot slot, GameObject prefab, AnimationSet animationSet, bool requiresBothHands, bool useLeftHandIK) : base(name, flavor, rarity, slot, prefab)
    {
        this.animationSet = animationSet;
        this.requiresBothHands = requiresBothHands;
        this.useLeftHandIK = useLeftHandIK;
    }
}
