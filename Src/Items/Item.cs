using UnityEngine;

using System.Collections.Generic;

public class Item
{
    public string name { get; private set; }
    public string flavor { get; private set; }

    public DamageType damageType { get; private set; }

    public List<ItemEffect> pickupEffects { get; private set; } = new List<ItemEffect>();
    public List<ItemEffect> equipEffects { get; private set; } = new List<ItemEffect>();
    public List<ItemEffect> newTurnEffects { get; private set; } = new List<ItemEffect>();
    public List<ItemEffect> unequipEffects { get; private set; } = new List<ItemEffect>();
    public List<ItemEffect> droppedEffects { get; private set; } = new List<ItemEffect>();

    public GameObject prefab { get; private set; }
    public ItemRarity rarity { get; private set; }

    public Entity holder { get; private set; }

    public bool isEquipped { get; private set; }

    //IT'S FUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUCJED
    //IM FUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUCJED
    //FUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUCJED
    public Item(string name, string flavor, DamageType damageType, GameObject prefab, ItemRarity rarity)
    {
        this.name = name;
        this.flavor = flavor;

        this.damageType = damageType;

        this.prefab = prefab;
        this.rarity = rarity;
    }

    public virtual void OnPickUp(Entity holder)
    {
        this.holder = holder;

        for (int i = 0; i < pickupEffects.Count; i++)
            pickupEffects[i].Tick(this.holder);
    }
    public virtual void OnEquip()
    {
        this.isEquipped = true;

        for (int i = 0; i < equipEffects.Count; i++)
            equipEffects[i].Tick(this.holder);
    }
    public virtual void OnNewTurn()
    {
        for (int i = 0; i < newTurnEffects.Count; i++)
            newTurnEffects[i].Tick(this.holder);
    }
    public virtual void OnUnequip()
    {
        this.isEquipped = false;
    
        for (int i = 0; i < unequipEffects.Count; i++)
            unequipEffects[i].Tick(this.holder);
    }
    public virtual void OnDropped()
    {
        if(this.isEquipped)
            this.OnUnequip();

        for (int i = 0; i < droppedEffects.Count; i++)
            droppedEffects[i].Tick(this.holder);

        this.holder = null;
    }

    public virtual GameObject Instantiate()
    {
        return Object.Instantiate(prefab);
    }

    public string NameToString()
    {
        return "<color=#" + ColorUtility.ToHtmlStringRGBA(GetColor()) + ">" + name + "</color>";
    }

    public override string ToString()
    {
        return name + "\n" + flavor + "\n" + damageType.ToString();
    }
    public virtual string ToTooltip()
    {
        return "<color=#" + ColorUtility.ToHtmlStringRGBA(GetColor()) + ">" + name + "\n" +
            rarity.ToString() + "</color>\n\n" +
            (damageType.requiresBothHands ? "<color=orange>Requires Both Hands</color>\n\n" : "") +
            "Damage: D" + damageType.damage;
    }

    public Color GetColor()
    {
        switch (rarity)
        {
            case ItemRarity.Common:
                return Color.gray;
            case ItemRarity.Fine:
                return Color.green;
            case ItemRarity.Mastercrafted:
                return Color.blue;
            case ItemRarity.Ancient:
                return Color.red;
            default:
                return Color.magenta;
        }
    }
}