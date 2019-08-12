using System;

using MoonSharp.Interpreter;

[MoonSharpUserData]
public class Stat
{
    Func<float> _getValue;
    Func<string> _getTooltip;

    public StatType type { get; private set; }
    public StatCategory category { get; private set; }

    public Stat(StatType type, StatCategory category, Func<float> getValue, Func<string> getTooltip)
    {
        this.type = type;
        this.category = category;

        _getValue = getValue;
        _getTooltip = getTooltip;
    }

    public float GetValue()
    {
        return _getValue();
    }
    public string ToTooltip()
    {
        return _getTooltip();
    }

    public string GetHeader()
    {
        switch (type)
        {
            case StatType.SightRange:
                return "Sight Range";
            case StatType.SightThreshold:
                return "Sight Threshold";
            case StatType.WalkRange:
                return "Walk Range";
            case StatType.SprintRange:
                return "Sprint Range";
            default:
                return "";
        }
    }
    public string GetValueFormatted()
    {
        switch (type)
        {
            case StatType.SightRange:
                return GetValue() + " units";
            case StatType.SightThreshold:
                return GetValue() + "%";
            case StatType.WalkRange:
                return GetValue() + " units";
            case StatType.SprintRange:
                return GetValue() + " units";
            default:
                return "n/a";
        }
    }
}
public enum StatType
{
    SightRange,
    SightThreshold,
    WalkRange,
    SprintRange
}
public enum StatCategory
{
    Offensive,
    Defensive,
    Misc
}