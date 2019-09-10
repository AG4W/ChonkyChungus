using UnityEngine;

public static class Map
{
    public static string StringFormatted(MapType mt)
    {
        switch (mt)
        {
            case MapType.Movement:
                return "Movement";
            case MapType.LineOfSight:
                return "Line of Sight";
            default:
                return "n/a";
        }
    }
    public static Color GetColor(MapType mt)
    {
        switch (mt)
        {
            case MapType.Movement:
                return Color.yellow;
            case MapType.LineOfSight:
                return Color.white;
            default:
                return Color.magenta;
        }
    }
}
