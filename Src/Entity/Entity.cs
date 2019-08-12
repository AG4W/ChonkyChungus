using UnityEngine;

using MoonSharp.Interpreter;

[MoonSharpUserData]
public class Entity : MonoBehaviour
{
    [Range(0, 99)][SerializeField]int _interactCost = 0;
    [Range(1, 99)][SerializeField]int _interactRange = 1;

    public int interactCost { get { return _interactCost; } }
    public int interactRange { get { return _interactRange; } }

    public Tile tile { get; private set; }

    public bool isBusy { get; private set; }
    
    public virtual void Interact(Actor interactee)
    {

    }
    public virtual void Examine(Actor examineer)
    {
        GlobalEvents.Raise(GlobalEvent.PopupRequested,
            this.transform.position + Vector3.up,
            examineer.data.name + " examined " + this.name);
    }

    public void SetPosition(Tile t)
    {
        //set old
        this.tile?.SetEntity(null);
        //set new
        this.tile = t;
        this.tile.SetEntity(this);

        this.transform.position = t.position;
    }
    public void SetIsBusy(bool isBusy)
    {
        this.isBusy = isBusy;
    }
}
