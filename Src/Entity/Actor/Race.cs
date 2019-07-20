using UnityEngine;

using System;

public class Race
{
    public static int GetBaseLuminosityThreshold(RaceType type)
    {
        switch (type)
        {
            case RaceType.Human:
                return 33;
            case RaceType.Undead:
                return 50;
            default:
                return -1;
        }
    }
    public static int GetBaseSightRange(RaceType type)
    {
        switch (type)
        {
            case RaceType.Human:
                return 25;
            case RaceType.Undead:
                return 10;
            default:
                return -1;
        }
    }
}
