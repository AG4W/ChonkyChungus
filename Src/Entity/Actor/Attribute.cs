using UnityEngine;

using System;

public class Attribute
{
    Func<string> _getTooltip;

    int _base;
    int _modSum;

    public int assigned { get; private set; }
    public int value { get { return _base + assigned + _modSum; } }
    public AttributeType type { get; private set; }

    public Attribute(AttributeType type, int baseValue, Func<string> getTooltip)
    {
        this.type = type;

        _base = baseValue;
        _modSum = 0;

        _getTooltip = getTooltip;
    }

    public void Increment()
    {
        assigned++;

        OnAttributeChanged?.Invoke(type);
    }

    public delegate void AttributeChangedEvent(AttributeType at);
    public event AttributeChangedEvent OnAttributeChanged;

    public string GetTooltip()
    {
        return _getTooltip();
    }
    public static Color ToColor(AttributeType type)
    {
        switch (type)
        {
            case AttributeType.Strength:
                return Color.red;
            case AttributeType.Vitality:
                return new Color(1f, .5f, 0f);
            case AttributeType.Quickness:
                return Color.green;
            case AttributeType.Accuracy:
                return Color.cyan;
            case AttributeType.Willpower:
                return Color.blue;
            default:
                return Color.white;
        }
    }
}
public enum AttributeType
{
    Strength,
    Vitality,
    Quickness,
    Accuracy,
    Willpower,
}