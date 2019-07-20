using UnityEngine;

public class Consumable : Item
{
    public int maxCharges { get; private set; }
    public int currentCharges { get; private set; }

    public VitalModifier[] modifiers { get; private set; }

    public Consumable(string name, string flavor, DamageType damageType, GameObject prefab, ItemRarity rarity, int maxCharges, int currentCharges, VitalModifier[] modifiers) : base(name, flavor, damageType, prefab, rarity)
    {
        this.maxCharges = maxCharges;
        this.currentCharges = currentCharges;
        this.modifiers = modifiers;
    }

    public virtual void Consume(Actor consumer)
    {
        for (int i = 0; i < modifiers.Length; i++)
            consumer.data.GetVital(modifiers[i].type).Update(modifiers[i].value);

        currentCharges--;

        if(currentCharges == 0)
            if (consumer.data.inventory.Contains(this))
                new RemoveItemCommand(consumer, this);
    }

    public override string ToTooltip()
    {
        string s = base.ToTooltip();

        s += "\n\n";
        s += "Charges: " + currentCharges + "/" + maxCharges + "\n\n";

        for (int i = 0; i < modifiers.Length; i++)
            s += modifiers[i].ToString() + "\n";

        return s;
    }
}
public enum ConsumableType
{
    Potion,
    Bandage,
}
