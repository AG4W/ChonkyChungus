using UnityEngine;

public class CreateActorCommand : Command
{
    Tile _position;

    public CreateActorCommand(Tile position)
    {
        _position = position;
    }
    public override void Execute()
    {
        //look upon me and despair
        Actor a = new GameObject("player actor", typeof(Actor)).GetComponent<Actor>();
        a.Initialize(Resources.Load<ActorTemplate>("ActorTemplates/player").Instantiate(), 0);
        a.SetPosition(_position == null ? Grid.GetRandom(true) : _position);

        GlobalEvents.Raise(GlobalEvent.ActorAdded, a);
    }
}