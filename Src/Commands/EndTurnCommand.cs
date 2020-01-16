public class EndTurnCommand : Command
{
    public override void Execute()
    {
        GlobalEvents.Raise(GlobalEvent.EndTurn);
    }
}