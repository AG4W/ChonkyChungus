using UnityEngine;

public class RemoveItemCommand : ActorCommand
{
    Item _item;

    public RemoveItemCommand(Actor actor, Item item) : base(actor)
    {
        _item = item;
    }

    public override void Execute()
    {
        actor.data.RemoveItem(_item);
        GlobalEvents.Raise(GlobalEvent.PopupRequested, 
            base.actor.transform.position + Vector3.up * 2, 
            "Lost: " + _item.NameToString() + "!");

        if (base.actor == Player.actor)
            UIAudioManager.Play(UISoundType.ItemRemoved);
    }
}