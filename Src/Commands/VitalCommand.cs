public class VitalCommand : ActorCommand
{
    VitalType _type;
    int _amount;

    public VitalCommand(Actor actor, VitalType type, int amount) : base(actor)
    {
        _type = type;
        _amount = amount;
    }

    public override void Execute()
    {
        base.actor.data.GetVital(_type).Update(_amount);
    }
}
