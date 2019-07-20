using UnityEngine;

public class CreateNPActorCommand : Command
{
    ActorTemplate _template;

    public CreateNPActorCommand(ActorTemplate template)
    {
        _template = template;
    }

    public override void Execute()
    {
        NPActor a = new GameObject("enemy", typeof(NPActor)).GetComponent<NPActor>();
        a.Initialize(_template.Instantiate(), 1);
        a.SetPosition(Grid.GetRandom(true));

        GlobalEvents.Raise(GlobalEvent.ActorAdded, a);
    }
}