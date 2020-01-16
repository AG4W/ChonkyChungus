using UnityEngine;

using MoonSharp.Interpreter;

[MoonSharpUserData]
public class Equipable : Item
{
    public EquipSlot slot { get; private set; }
    public GameObject prefab { get; private set; }

    public Equipable(string name, string flavor, ItemRarity rarity, EquipSlot slot, GameObject prefab) : base(name, flavor, rarity)
    {
        this.slot = slot;
        this.prefab = prefab;
    }
}
