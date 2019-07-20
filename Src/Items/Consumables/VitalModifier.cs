using UnityEngine;

[System.Serializable]
public class VitalModifier
{
    [SerializeField]int _value;
    [SerializeField]VitalType _type;

    public int value { get { return _value; } }
    public VitalType type { get { return _type; } }

    public VitalModifier(int value, VitalType type)
    {
        _value = value;
        _type = type;
    }

    public override string ToString()
    {
        return _value.ToString("+#;-#;0") + " " + _type;
    }
}