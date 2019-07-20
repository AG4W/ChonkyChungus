using System;
using System.Collections.Generic;

public static class GlobalEvents
{
    static List<Action<object[]>>[] _events;
    static bool _hasInitialized = false;

    static void Initialize()
    {
        _events = new List<Action<object[]>>[Enum.GetNames(typeof(GlobalEvent)).Length];

        for (int i = 0; i < _events.Length; i++)
            _events[i] = new List<Action<object[]>>();

        _hasInitialized = true;
    }

    public static void Subscribe(GlobalEvent g, Action<object[]> a)
    {
        if (!_hasInitialized)
            Initialize();

        _events[(int)g].Add(a);
    }
    public static void Raise(GlobalEvent g, params object[] args)
    {
        for (int i = 0; i < _events[(int)g].Count; i++)
            _events[(int)g][i]?.Invoke(args);
    }
}
public enum GlobalEvent
{
    PCGComplete,

    ToggleInventory,
    ToggleCharacter,
    ToggleSkills,

    ToggleMovement,

    ActorAdded,
    ActorMoveStart,
    ActorMoveEnd,
    ActorMapChanged,
    ActorVisibilityChanged,
    ActorEquipmentChanged,
    ActorInventoryChanged,
    ActorSpellbookChanged,
    ActorExperienceChanged,
    ActorLeveledUp,
    ActorAttributeChanged,
    ActorVitalChanged,
    ActorRemoved,

    LightSourceInfluenceChanged,

    GameManagerInitialized,
    MissionAdded,
    TaskStatusChanged,

    EntityDiscovered,

    PopupRequested,
    SetCameraTrackingTarget,
    JumpCameraTo,

    NewTurn,
    EndTurn,
}
