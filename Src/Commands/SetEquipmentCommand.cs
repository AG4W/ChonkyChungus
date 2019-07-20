public class SetEquipmentCommand : ActorCommand
{
    EquipSlot _slot;
    Item _item;

    public SetEquipmentCommand(Actor actor, EquipSlot slot, Item item) : base(actor)
    {
        _slot = slot;
        _item = item;
    }
    public override void Execute()
    {
        base.actor.data.SetEquipment(_slot, _item);
    }
}
