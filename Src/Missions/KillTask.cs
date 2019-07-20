using System.Linq;
using System.Collections.Generic;

public class KillTask : Task
{
    public List<Actor> targets { get; private set; }

    public KillTask(int enemyCount)
    {
        this.targets = new List<Actor>();

        for (int i = 0; i < enemyCount; i++)
            this.targets.Add(GameManager.GetRandom(1));

        GlobalEvents.Subscribe(GlobalEvent.ActorRemoved, (object[] args) =>
        {
            if (targets.Contains((Actor)args[0]))
            {
                this.targets.Remove((Actor)args[0]);
                GlobalEvents.Raise(GlobalEvent.TaskStatusChanged, this);
            }
        });
    }
    public KillTask(List<Actor> targets)
    {
        this.targets = targets.ToList();

        GlobalEvents.Subscribe(GlobalEvent.ActorRemoved, (object[] args) =>
        {
            if(targets.Contains((Actor)args[0]))
            {
                this.targets.Remove((Actor)args[0]);
                GlobalEvents.Raise(GlobalEvent.TaskStatusChanged, this);
            }
        });
    }

    public override bool IsComplete()
    {
        return this.targets.Count == 0;
    }
    public override string ToString()
    {
        return
            "Kill Task\n" +
            "Remaining: " + this.targets.Count;
    }
}