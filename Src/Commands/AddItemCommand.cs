using UnityEngine;

public class AddItemCommand : ActorCommand
{
    Item _item;

    public AddItemCommand(Actor actor, Item item) : base(actor)
    {
        _item = item;
    }

    public override void Execute()
    {
        base.actor.data.AddItem(_item);
        GlobalEvents.Raise(GlobalEvent.PopupRequested,
            base.actor.transform.position + Vector3.up * 2,
            "Found: " + _item.NameToString() + "!");

        if(base.actor == Player.actor)
            UIAudioManager.Play(UISoundType.ItemAdded);
    }
}