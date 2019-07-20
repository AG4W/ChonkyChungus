using UnityEngine;

public class LightSpell : Spell
{
    public LightSpell(string name, string description, int castCost, GameObject vfx) : base(name, description, castCost, vfx)
    {
    }

    public override void Cast(Actor caster, Entity target)
    {
        GameObject g = Object.Instantiate(
            base.vfx, 
            caster.transform.position + new Vector3(.5f, 1.75f, .5f), 
            Quaternion.identity, 
            caster.transform);
    }
}