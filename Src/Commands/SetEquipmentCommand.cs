public class SetEquipmentCommand : ActorCommand
{
    Equipable _equipable;

    public SetEquipmentCommand(Actor actor, Equipable equipable) : base(actor)
    {
        _equipable = equipable;
    }
    public override void Execute()
    {
        base.actor.data.SetEquipment(_equipable);
    }
}
