using UnityEngine;

public class CreateNPActorCommand : Command
{
    ActorTemplate _template;
    Tile _position;

    public CreateNPActorCommand(ActorTemplate template)
    {
        _template = template;
    }
    public CreateNPActorCommand(ActorTemplate template, Tile position)
    {
        _template = template;
        _position = position;
    }

    public override void Execute()
    {
        NPActor a = new GameObject("", typeof(NPActor)).GetComponent<NPActor>();

        a.Initialize(_template.Instantiate(), 1);
        a.gameObject.name = a.data.name;
        a.SetPosition(_position ?? Grid.GetRandom(TileStatus.Vacant));

        GlobalEvents.Raise(GlobalEvent.ActorAdded, a);
    }
}