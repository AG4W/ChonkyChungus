﻿using System;
using System.Collections.Generic;

using MoonSharp.Interpreter;

[MoonSharpUserData]
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
    //more init
    PCGComplete,

    //ui
    ToggleInventory,
    ToggleCharacter,
    ToggleSkills,

    ToggleMovement,

    //actors
    ActorAdded,
    ActorMoveStart,
    ActorMoveEnd,
    ActorMapChanged,
    ActorVisibilityChanged,
    ActorEquipmentChanged,
    ActorInventoryChanged,
    ActorSpellsChanged,
    ActorExperienceChanged,
    ActorLeveledUp,
    ActorAttributeChanged,
    ActorVitalChanged,
    ActorTargetsChanged,
    ActorRemoved,

    LightSourceInfluenceChanged,
    //init
    GameManagerInitialized,
    //ui
    MissionAdded,
    TaskStatusChanged,

    //prob. obsolote?
    EntityDiscovered,

    //camera manager
    PopupRequested,
    SetCameraTrackingTarget,
    JumpCameraTo,
    CutToCameraTargeteeTargetShot,
    ExitDynamicMode,

    //targeting manager
    EnterTargetingMode,
    SetTargetee,
    StepTargetIndex,
    SetTargetIndex,
    ExitTargetingMode,

    //crosshair
    ShowCrosshair,
    HideCrosshair,

    //grid
    SetGridMap,
    ToggleGridVisibility,

    //game manager
    NewTurn,
    EndTurn,

    //hotkeys
    HotkeyPressed,
}
