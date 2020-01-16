using UnityEngine;

public class Armour : Equipable
{
    public Color color { get; private set; }

    public Armour(string name, string flavor, ItemRarity rarity, EquipSlot slot, GameObject prefab) : base(name, flavor, rarity, slot, prefab)
    {
        this.color = new Color(Synched.Next(0f, 1f), Synched.Next(0f, 1f), Synched.Next(0f, 1f), 1f);
    }
}
