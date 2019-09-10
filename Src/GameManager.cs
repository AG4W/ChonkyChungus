using UnityEngine;

using System.Collections.Generic;
using System.Linq;

public static class GameManager
{
    static int _turnIndex;

    static Dungeon _dungeon;
    static Mission _mission;

    static List<Actor> _actors;

    public static int actorCount { get { return _actors.Count; } }

    public static void Initialize()
    {
        _dungeon = new Dungeon();
        _actors = new List<Actor>();
    
        GlobalEvents.Subscribe(GlobalEvent.ActorAdded, (object[] args) => 
        {
            AddActor((Actor)args[0]);
        });
        GlobalEvents.Subscribe(GlobalEvent.ActorRemoved, (object[] args) => 
        {
            RemoveActor((Actor)args[0]);
        });
        GlobalEvents.Subscribe(GlobalEvent.EndTurn, (object[] args) =>
        {
            MoveToNextTurn();
        });

        CreatePlayer();
        CreateEnemies();
        CreateMission();

        _turnIndex = 0;
        _actors[_turnIndex].OnNewTurn();

        GlobalEvents.Raise(GlobalEvent.GameManagerInitialized);
    }

    static void CreatePlayer()
    {
        Actor a = new GameObject("player actor", typeof(Actor)).GetComponent<Actor>();
        a.Initialize(Resources.Load<ActorTemplate>("ActorTemplates/player").Instantiate(), 0);
        a.SetPosition(Grid.Get(_dungeon.rooms[0].centerX + 1, _dungeon.rooms[0].centerZ));

        Player.SetActor(a);
        GlobalEvents.Raise(GlobalEvent.ActorAdded, a);
    }
    static void CreateEnemies()
    {
        ActorTemplate[] templates = Resources.LoadAll<ActorTemplate>("ActorTemplates/Enemies/");

        for (int i = 0; i < Synched.Next(4, 10); i++)
            new CreateNPActorCommand(templates.Random()).Execute();

        new CreateNPActorCommand(templates.Random(), Grid.Get((int)Player.actor.tile.position.x + 3, (int)Player.actor.tile.position.z + 3)).Execute();
        new CreateNPActorCommand(templates.Random(), Grid.Get((int)Player.actor.tile.position.x + 3, (int)Player.actor.tile.position.z + -3)).Execute();
        new CreateNPActorCommand(templates.Random(), Grid.Get((int)Player.actor.tile.position.x + -3, (int)Player.actor.tile.position.z + -3)).Execute();
        new CreateNPActorCommand(templates.Random(), Grid.Get((int)Player.actor.tile.position.x + -3, (int)Player.actor.tile.position.z + 3)).Execute();
    }
    static void CreateMission()
    {
        _mission = MissionGenerator.GetRandom();

        GlobalEvents.Raise(GlobalEvent.MissionAdded, _mission);
    } 

    static void AddActor(Actor a)
    {
        _actors.Add(a);
    }
    static void RemoveActor(Actor a)
    {
        _actors.Remove(a);
    }

    static void MoveToNextTurn()
    {
        if (_turnIndex == _actors.Count - 1)
            _turnIndex = 0;
        else
            _turnIndex++;
    
        _actors[_turnIndex].OnNewTurn();
    }

    public static Actor GetRandom(int teamID = -1)
    {
        if (teamID == -1)
            return _actors.Random();
        else
            //this is ugly, plz fix >---------------------\/
            return _actors.Where(a => a.teamID == teamID).ToArray().Random();
    }
}
