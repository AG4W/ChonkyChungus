using UnityEngine;

using MoonSharp.Interpreter;

[MoonSharpUserData]
public class Entity : MonoBehaviour
{
    [Range(0, 99)][SerializeField]int _interactCost = 0;
    [Range(1, 99)][SerializeField]int _interactRange = 1;

    [SerializeField]string _interactionHeader = "[Interact]";
    [SerializeField]Vector3 _headerOffset = Vector3.up;
    [TextArea(3, 10)][SerializeField]string[] _examinationSnippets;

    public int interactCost { get { return _interactCost; } }
    public int interactRange { get { return _interactRange; } }

    public string interactionHeader { get { return _interactionHeader; } }
    public Vector3 headerOffset { get { return _headerOffset; } }

    public Tile tile { get; private set; }

    public bool isBusy { get; private set; }
    public bool isActor { get { return this is Actor; } }
    
    public virtual void Interact(Actor interactee)
    {
        //interactee.AddCommand(new RotateCommand(interactee, this.tile.position));
    }
    public virtual void Examine(Actor examineer)
    {
        //examineer.AddCommand(new RotateCommand(examineer, this.tile.position));

        GlobalEvents.Raise(GlobalEvent.PopupRequested, this.transform.position + Vector3.up, _examinationSnippets.Random());
    }

    public void SetPosition(Tile t)
    {
        //set old
        this.tile?.SetStatus(TileStatus.Vacant);
        this.tile?.SetEntity(null);
        //set new
        this.tile = t;
        this.tile.SetStatus(TileStatus.Occupied);
        this.tile.SetEntity(this);

        this.transform.position = t.position;
    }
    public void SetIsBusy(bool isBusy)
    {
        this.isBusy = isBusy;
    }
}
