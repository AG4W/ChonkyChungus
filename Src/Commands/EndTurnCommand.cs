public class EndTurnCommand : ActorCommand
{
    public EndTurnCommand(Actor actor) : base(actor)
    {

    }

    public override void Execute()
    {
        base.actor.OnEndTurn();
    }
}

