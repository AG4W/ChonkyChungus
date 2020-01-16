using UnityEngine;

using System;
using System.Collections.Generic;

using MoonSharp.Interpreter;

[MoonSharpUserData]
public class Vital
{
    Func<int> _getMax;
    Func<string> _getTooltip;

    List<VitalModifier> _modifiers = new List<VitalModifier>();

    public int current { get; private set; }

    public VitalType type { get; private set; }

    public Vital(VitalType type, Func<int> getMax, Func<string> getTooltip)
    {
        this.type = type;

        _getMax = getMax;
        _getTooltip = getTooltip;

        this.current = type == VitalType.Corruption ? 0 : _getMax();
    }

    public void Update(int amount)
    {
        this.current += amount;

        if (this.current < 0)
            this.current = 0;
        else if (this.current > _getMax())
            this.current = _getMax();

        OnVitalChanged?.Invoke(type, amount);
    }
    public void SetCurrent(int current)
    {
        //cache old
        int change = current - this.current;

        this.current = current;

        if (this.current < 0)
            this.current = 0;
        else if (this.current > _getMax())
            this.current = _getMax();

        OnVitalChanged?.Invoke(type, change);
    }

    public int GetMax()
    {
        return _getMax();
    }

    public delegate void VitalChangedEvent(VitalType vt, int change);
    public event VitalChangedEvent OnVitalChanged;

    public string ToTooltip()
    {
        return _getTooltip();
    }
    public static Color ToColor(VitalType type)
    {
        switch (type)
        {
            case VitalType.Health:
                return Color.red;
            case VitalType.Corruption:
                return new Color(.5f, 0f, .5f);
            case VitalType.Stamina:
                return Color.green;
            default:
                return Color.white;
        }
    }
    public static string ToStringFormat(VitalType type)
    {
        switch (type)
        {
            case VitalType.Health:
                return "<color=red>Health</color>";
            case VitalType.Corruption:
                return "<color=purple>Corruption</color>";
            case VitalType.Stamina:
                return "<color=green>Stamina</color>";
            default:
                return "Vital N/A";
        }
    }
}
public enum VitalType
{
    Health,
    Corruption,
    Stamina
}
