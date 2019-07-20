using UnityEngine;

using System.Linq;

public class ContainerEntity : Entity
{
    Item[] _items;

    bool _isEmpty;

    public void Initialize(ItemType[] whitelist, ItemRarity maxRarity)
    {
        _items = new Item[Synched.Next(0, 2)];

        //for (int i = 0; i < _items.Length; i++)
        //    _items[i] = Synched.Next(0, 2 + 1) == 2 ? null : ItemGenerator.Get(maxRarity, whitelist);

        _isEmpty = _items.Length == 0 || _items.All(i => i == null);
    }

    public override void Interact(Actor interactee)
    {
        base.Interact(interactee);

        if (_isEmpty)
            GlobalEvents.Raise(GlobalEvent.PopupRequested, this.transform.position + Vector3.up, "It's empty!");
        else
        {
            for (int i = 0; i < _items.Length; i++)
                if (_items[i] != null)
                    new AddItemCommand(interactee, _items[i]);

            _isEmpty = true;
        }
    }
}
