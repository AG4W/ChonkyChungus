public class InteractCommand : ActorCommand
{
    Entity _entity;

    public InteractCommand(Actor actor, Entity entity) : base(actor)
    {
        _entity = entity;
    }

    public override void Execute()
    {
        base.actor.data.GetVital(VitalType.Stamina).Update(-_entity.interactCost);

        _entity.Interact(base.actor);
    }
}