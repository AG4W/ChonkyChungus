using System;

public static class TargetingManager
{
    static int _targetIndex = 0;

    static Actor _targetee;
    static Action _executeCallback;
    static ActionContext _context;

    static bool _hasInitialized = false;
    static bool _isTargeting = false;

    public static void Initalize()
    {
        if (_hasInitialized)
            return;

        GlobalEvents.Subscribe(GlobalEvent.EnterTargetingMode, Enter);
        GlobalEvents.Subscribe(GlobalEvent.SetTargetee, (object[] args) => _targetee = args[0] as Actor);
        GlobalEvents.Subscribe(GlobalEvent.SetTargetIndex, SetTargetIndex);
        GlobalEvents.Subscribe(GlobalEvent.StepTargetIndex, StepTargetIndex);
        GlobalEvents.Subscribe(GlobalEvent.ExitTargetingMode, Exit);

        _hasInitialized = true;
    }

    static void Enter(object[] args)
    {
        _isTargeting = true;

        _context = args[0] as ActionContext;
        _executeCallback = () => _context.action.Execute(_context);

        SetTarget(0);
    }
    static void StepTargetIndex(params object[] args)
    {
        if (!_isTargeting)
            return;

        SetTarget(_targetIndex.WrapStep(0, _targetee.visibleActors.Count - 1, (bool)args[0]));
    }
    static void SetTargetIndex(params object[] args)
    {
        if (!_isTargeting)
            return;

        SetTarget((int)args[0]);
    }
    static void SetTarget(int index)
    {
        _targetIndex = index;
        _targetee.SetTargets(_targetee.visibleActors[_targetIndex]);
        _targetee.transform.LookAt(_targetee.visibleActors[_targetIndex].tile.position, UnityEngine.Vector3.up);

        GlobalEvents.Raise(GlobalEvent.ShowCrosshair, _context, _targetee.visibleActors[_targetIndex]);
        GlobalEvents.Raise(GlobalEvent.CutToCameraTargeteeTargetShot, _targetee, _targetee.visibleActors[_targetIndex]);
    }

    //true to execute on exit
    static void Exit(object[] args)
    {
        if ((bool)args[0])
            _executeCallback?.Invoke();

        _targetee.SetTargets();
        _isTargeting = false;

        GlobalEvents.Raise(GlobalEvent.HideCrosshair);
        GlobalEvents.Raise(GlobalEvent.ExitDynamicMode);
    }
}
public enum TargetingMode
{
    Single,
    Multiple,
    Area
}