using UnityEngine;

public static class Player
{
    public static int level { get; private set; } = 1;
    public static int spendableLevels { get; private set; } = 2;
    public static float experience { get; private set; } = 0f;

    public static ActorData data { get; private set; }
    public static Actor actor { get; private set; }

    public static void SetActor(Actor player)
    {
        actor = player;
        data = player.data;
    }
    public static void IncreaseExperience(float amount)
    {
        if (level >= 25)
            return;

        experience += amount;

        if(experience >= level * 1000)
        {
            experience = 0f;
            level++;
            spendableLevels++;

            GlobalEvents.Raise(GlobalEvent.ActorLeveledUp, actor);
            GlobalEvents.Raise(GlobalEvent.PopupRequested,
                    actor.transform.position + Vector3.up * 2,
                    "Level " + level + " reached!");
        }

        GlobalEvents.Raise(GlobalEvent.ActorExperienceChanged, actor);
    }

    public static void IncrementAttribute(AttributeType type)
    {
        spendableLevels--;
        data.GetAttribute(type).Increment();
    }
}