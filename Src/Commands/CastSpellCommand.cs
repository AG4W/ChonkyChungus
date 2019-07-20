public class CastSpellCommand : ActorCommand
{
    Spell _spell;
    Entity _target;

    public CastSpellCommand(Actor caster, Spell spell, Entity target) : base(caster)
    {
        _spell = spell;
        _target = target;
    }

    public override void Execute()
    {
        base.actor.data.GetVital(VitalType.Corruption).Update(_spell.castCost);

        _spell.Cast(base.actor, _target);
    }
}