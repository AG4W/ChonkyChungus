public class Race
{
    public static float GetBaseLuminosityThreshold(RaceType type)
    {
        switch (type)
        {
            case RaceType.Human:
                return .33f;
            case RaceType.Undead:
                return .50f;
            default:
                return -1f;
        }
    }
    public static int GetBaseSightRange(RaceType type)
    {
        switch (type)
        {
            case RaceType.Human:
                return 15;
            case RaceType.Undead:
                return 10;
            default:
                return -1;
        }
    }
}
