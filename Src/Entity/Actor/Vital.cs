using UnityEngine;

using System;
using System.Collections.Generic;

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

        OnVitalChanged?.Invoke(type);
    }
    public void SetCurrent(int current)
    {
        this.current = current;

        if (this.current < 0)
            this.current = 0;
        else if (this.current > _getMax())
            this.current = _getMax();

        OnVitalChanged?.Invoke(type);
    }

    public int GetMax()
    {
        return _getMax();
    }

    public delegate void VitalChangedEvent(VitalType vt);
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
}
public enum VitalType
{
    Health,
    Corruption,
    Stamina
}
