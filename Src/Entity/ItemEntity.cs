using UnityEngine;

public class ItemEntity : Entity
{
    Item _item;

    bool _destroyAfterUse;

    public void Initialize(bool destroyAfterUse, params ItemType[] choices)
    {
        _destroyAfterUse = destroyAfterUse;

        if(_item.prefab != null)
            Instantiate(_item.prefab, this.transform.position, Quaternion.Euler(0, Synched.Next(0, 360), 0), this.transform);
    }

    public override void Interact(Actor interactee)
    {
        base.Interact(interactee);

        new AddItemCommand(interactee, _item);

        if(_destroyAfterUse)
            new DestroyCommand(this.gameObject).Execute();
    }
}
