using UnityEngine;

public static class DiceUtils
{
    static Color _minimum = Color.red;
    static Color _maximum = Color.green;

    public static string ToStringFormatted(int dice)
    {
        return "<color=" + ColorUtility.ToHtmlStringRGB(Color.Lerp(_minimum, _maximum, dice / 20f)) + ">D" + dice.ToString() + "</color>";
    }
}
