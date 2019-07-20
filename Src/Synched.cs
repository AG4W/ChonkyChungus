using Random = System.Random;

public static class Synched
{
    static Random random;

    public static void SetSeed(int seed)
    {
        random = new Random(seed);
    }

    public static int Next(int min, int max)
    {
        return random.Next(min, max);
    }
    public static float Next(float min, float max)
    {
        double n = random.NextDouble();
        return (float)(min + (n * (max - min)));
    }
    public static ItemRarity GetRarityWeighted(ItemRarity maxRarity)
    {
        return random.GetRarityWeighted(maxRarity);
    }

    public static int Dice(int size)
    {
        return random.Next(1, size + 1);
    }
}
