using Random = System.Random;

using MoonSharp.Interpreter;

[MoonSharpUserData]
public static class Synched
{
    static Random _random;

    public static void SetSeed(int seed)
    {
        _random = new Random(seed);
    }

    public static int Next(int min, int max)
    {
        return _random.Next(min, max);
    }
    public static float Next(float min, float max)
    {
        return (float)(min + (_random.NextDouble() * (max - min)));
    }

    public static int Dice(int size)
    {
        return _random.Next(1, size + 1);
    }
}
