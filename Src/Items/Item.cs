using UnityEngine;

using Object = UnityEngine.Object;
using System.Collections.Generic;

using MoonSharp.Interpreter;

[MoonSharpUserData]
public class Item
{
    public string name { get; private set; }
    public string flavor { get; private set; }

    public DamageType damageType { get; private set; }

    public List<ItemAction> pickupEffects { get; private set; } = new List<ItemAction>();
    public List<ItemAction> equipEffects { get; private set; } = new List<ItemAction>();
    public List<ItemAction> newTurnEffects { get; private set; } = new List<ItemAction>();
    public List<ItemAction> unequipEffects { get; private set; } = new List<ItemAction>();
    public List<ItemAction> droppedEffects { get; private set; } = new List<ItemAction>();

    public List<ItemAction> actions { get; private set; } = new List<ItemAction>();

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
    public Item(string name, string flavor, DamageType damageType, GameObject prefab, ItemRarity rarity, params ItemAction[] actions)
    {
        this.name = name;
        this.flavor = flavor;

        this.damageType = damageType;

        this.prefab = prefab;
        this.rarity = rarity;

        this.actions.AddRange(actions);
    }

    public virtual void OnPickUp(Entity holder)
    {
        this.holder = holder;

        for (int i = 0; i < pickupEffects.Count; i++)
            if (pickupEffects[i].InvokeFunction(ItemAction.Function.Validate, this.holder))
                pickupEffects[i].InvokeFunction(this.holder);
    }
    public virtual void OnEquip()
    {
        this.isEquipped = true;

        for (int i = 0; i < equipEffects.Count; i++)
            if (equipEffects[i].InvokeFunction(ItemAction.Function.Validate, this.holder))
                equipEffects[i].InvokeFunction(this.holder);
    }
    public virtual void OnNewTurn()
    {
        for (int i = 0; i < newTurnEffects.Count; i++)
            if (newTurnEffects[i].InvokeFunction(ItemAction.Function.Validate, this.holder))
                newTurnEffects[i].InvokeFunction(this.holder);
    }
    public virtual void OnUnequip()
    {
        this.isEquipped = false;
    
        for (int i = 0; i < unequipEffects.Count; i++)
            if (unequipEffects[i].InvokeFunction(ItemAction.Function.Validate, this.holder))
                unequipEffects[i].InvokeFunction(this.holder);
    }
    public virtual void OnDropped()
    {
        if(this.isEquipped)
            this.OnUnequip();
    
        for (int i = 0; i < droppedEffects.Count; i++)
            if (droppedEffects[i].InvokeFunction(ItemAction.Function.Validate, this.holder))
                droppedEffects[i].InvokeFunction(this.holder);

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
        string s = "<color=#" + ColorUtility.ToHtmlStringRGBA(GetColor()) + ">" + name + "\n" +
            rarity.ToString() + "</color>\n\n" +
            (damageType.requiresBothHands ? "<color=orange>Requires Both Hands</color>\n\n" : "") +
            "Damage: D" + damageType.damage + "\n\nActions:\n";

        for (int i = 0; i < this.actions.Count; i++)
            s += this.actions[i].ToString() + (i < this.actions.Count - 1 ? "\n\n" : "");


        return s;
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