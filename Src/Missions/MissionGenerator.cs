using UnityEngine;

public static class MissionGenerator
{
    const int KILL_MIN_COUNT = 2;

    static Mission GenerateKillMission()
    {
        return 
            new Mission(
                "Kill Quest",
                new KillTask(
                    Synched.Next(Mathf.Min(KILL_MIN_COUNT, GameManager.actorCount + 1), 
                    Mathf.Max(KILL_MIN_COUNT, GameManager.actorCount + 1))));
    }

    public static Mission GetRandom()
    {
        return GenerateKillMission();
    }
}
