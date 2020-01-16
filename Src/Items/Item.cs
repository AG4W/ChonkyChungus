using UnityEngine;

using System.Collections.Generic;

using ag4w.Actions;

using MoonSharp.Interpreter;

[MoonSharpUserData]
public class Item
{
    public string name { get; private set; }
    public string flavor { get; private set; }

    public List<Action>[] actions { get; private set; } = new List<Action>[]
    {
        new List<Action>(),
        new List<Action>(),
        new List<Action>(),
        new List<Action>(),
        new List<Action>(),
        new List<Action>()
    };

    public ItemRarity rarity { get; private set; }

    public Entity holder { get; private set; }

    public bool isEquipped { get; private set; }

    //IT'S FUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUCJED
    //IM FUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUCJED
    //FUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUCJED
    public Item(string name, string flavor, ItemRarity rarity)
    {
        this.name = name;
        this.flavor = flavor;
        this.rarity = rarity;
    }

    public virtual void OnPickUp(Entity holder)
    {
        this.holder = holder;

        foreach (Action ia in this.GetActions(ActionCategory.Pickup))
            if (ia.Validate(this.holder, this))
                ia.Activate(this.holder, this);
    }
    public virtual void OnEquip()
    {
        this.isEquipped = true;

        foreach (Action ia in this.GetActions(ActionCategory.Equip))
            if (ia.Validate(this.holder, this))
                ia.Activate(this.holder, this);
    }
    public virtual void OnNewTurn()
    {
        foreach (Action ia in this.GetActions(ActionCategory.NewTurn))
            if (ia.Validate(this.holder, this))
                ia.Activate(this.holder, this);
    }
    public virtual void OnUnequip()
    {
        this.isEquipped = false;

        foreach (Action ia in this.GetActions(ActionCategory.Unequip))
            if (ia.Validate(this.holder, this))
                ia.Activate(this.holder, this);
    }
    public virtual void OnDropped()
    {
        if(this.isEquipped)
            this.OnUnequip();

        foreach (Action ia in this.GetActions(ActionCategory.Drop))
            if (ia.Validate(this.holder, this))
                ia.Activate(this.holder, this);

        this.holder = null;
    }

    public List<Action> GetActions(ActionCategory ec)
    {
        return this.actions[(int)ec];
    }

    public virtual string ToTooltip()
    {
        return this.BaseInfoToString() + this.ActionsToString();
    }

    public string NameToString()
    {
        return "<color=#" + ColorUtility.ToHtmlStringRGBA(GetColor()) + ">" + name + "</color>";
    }
    public string BaseInfoToString()
    {
        return "<color=#" + ColorUtility.ToHtmlStringRGBA(GetColor()) + ">" + name + "\n" + rarity.ToString() + "</color>";
    }
    public string ActionsToString()
    {
        string s = "";

        for (int i = 0; i < this.actions.Length; i++)
        {
            if (this.actions[i].Count > 0)
            {
                s += "\n\n" + (ActionCategory)i + ":\n";

                for (int j = 0; j < this.actions[i].Count; j++)
                    s += this.actions[i][j].ToString() + (j < this.actions[i].Count - 1 ? "\n\n" : "");
            }
        }

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
public enum ActionCategory
{
    Pickup,
    Equip,
    NewTurn,
    Unequip,
    Drop,
    Activateable,
    Spell
}