using System.Collections.Generic;

public class MoveCommand : ActorCommand
{
    List<Tile> _path;
    bool _isSprinting;

    public MoveCommand(Actor actor, List<Tile> path) : base(actor)
    {
        _path = path;
        _isSprinting = _path.Count > base.actor.data.GetStat(StatType.WalkRange).GetValue();
    }

    public override void Execute()
    {
        base.actor.MoveTo(_path, _isSprinting);
    }
}