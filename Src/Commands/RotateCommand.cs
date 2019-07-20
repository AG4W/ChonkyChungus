using UnityEngine;

public class RotateCommand : ActorCommand
{
    Vector3 _target;

    public RotateCommand(Actor actor, Vector3 target) : base(actor)
    {
        _target = target;
    }

    public override void Execute()
    {
        base.actor.transform.LookAt(_target, Vector3.up);
    }
}