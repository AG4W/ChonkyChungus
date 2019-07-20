using UnityEngine;

public abstract class Spell
{
    public string name { get; private set; }
    public string description { get; private set; }

    public int castCost { get; private set; }

    public GameObject vfx { get; private set; }

    public Spell(string name, string description, int castCost, GameObject vfx)
    {
        this.name = name;
        this.description = description;

        this.castCost = castCost;

        this.vfx = vfx;
    }

    public abstract void Cast(Actor caster, Entity target);
    public string ToTooltip()
    {
        return name + "\n\n" + description + "\n\n" + castCost;
    }
}