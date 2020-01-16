using UnityEngine;

using System.Collections.Generic;
using System.Collections;
using System.Linq;

public static class GameManager
{
    static Mission _mission;

    static List<List<Actor>> _actors;

    public static Dungeon dungeon { get; private set; }

    public static int turnIndex { get; private set; } = -1;
    public static int actorCount { get { return _actors.Sum(l => l.Count); } }

    public static void Initialize()
    {
        GlobalEvents.Subscribe(GlobalEvent.PCGComplete, (object[] args) => GlobalEvents.Raise(GlobalEvent.CloseLoadingScreen));

        dungeon = new Dungeon();

        _actors = new List<List<Actor>>();
    
        GlobalEvents.Subscribe(GlobalEvent.ActorAdded, (object[] args) => 
        {
            AddActor((Actor)args[0]);
        });
        GlobalEvents.Subscribe(GlobalEvent.ActorStatusChanged, OnActorStatusChanged);
        GlobalEvents.Subscribe(GlobalEvent.ActorRemoved, (object[] args) => 
        {
            RemoveActor((Actor)args[0]);
        });
        GlobalEvents.Subscribe(GlobalEvent.EndTurn, (object[] args) => 
        {
            MoveToNextTurn();
        });

        CreatePlayers();
        CreateEnemies();
        //CreateMission();

        MoveToNextTurn();

        GlobalEvents.Raise(GlobalEvent.GameManagerInitialized);
    }

    static void CreatePlayers()
    {
        int rowIndex = -1;
        int colIndex = -1;

        for (int i = 0; i < Player.party.Length; i++)
        {
            colIndex++;

            if (i % 3 == 0)
            {
                rowIndex++;
                colIndex = 0;
            }

            if (Player.party[i] == null)
                continue;

            Tile t = Grid.Get(dungeon.rooms[0].centerX + colIndex, dungeon.rooms[0].centerZ + rowIndex);

            while (t.status != TileStatus.Vacant)
                t = dungeon.rooms[0].GetRandom(TileStatus.Vacant);

            Actor a = new GameObject("player actor", typeof(Actor)).GetComponent<Actor>();

            a.Initialize(Player.party[i], 0);
            a.SetPosition(t);

            if (Player.selectedActor == null)
                Player.SelectActor(a);
        
            GlobalEvents.Raise(GlobalEvent.ActorAdded, a);
        }
    }
    static void CreateEnemies()
    {
        ActorTemplate[] templates = Resources.LoadAll<ActorTemplate>("ActorTemplates/Enemies/");

        for (int i = 0; i < 4; i++)
        {
            NPActor a = new GameObject("", typeof(NPActor)).GetComponent<NPActor>();

            a.Initialize(templates.Random().Instantiate(), 1);
            a.gameObject.name = a.data.name;
            a.SetPosition(dungeon.rooms[0].GetRandom(TileStatus.Vacant));

            GlobalEvents.Raise(GlobalEvent.ActorAdded, a);

            a.data.SetEquipment(ItemGenerator.GetArmour(EquipSlot.Legs, ItemRarity.Common));
            a.data.SetEquipment(ItemGenerator.GetArmour(EquipSlot.Tunic, ItemRarity.Common));
            a.data.SetEquipment(ItemGenerator.GetArmour(EquipSlot.Chestplate, ItemRarity.Common));
            a.data.SetEquipment(ItemGenerator.GetArmour(EquipSlot.Head, ItemRarity.Common));
            a.data.SetEquipment(ItemGenerator.GetArmour(EquipSlot.Feet, ItemRarity.Common));

            a.data.SetEquipment(ItemGenerator.GetWeapon(WeaponType.OnehandedSword, ItemRarity.Common));
            a.data.Unequip(EquipSlot.RightHandItem);
        }
    }
    static void CreateMission()
    {
        _mission = MissionGenerator.GetRandom();

        GlobalEvents.Raise(GlobalEvent.MissionAdded, _mission);
    } 

    static void AddActor(Actor a)
    {
        if (a.teamID >= _actors.Count)
            _actors.Add(new List<Actor>());

        _actors[a.teamID].Add(a);
    }
    static void RemoveActor(Actor a)
    {
        _actors[a.teamID].Remove(a);
    }
    static void OnActorStatusChanged(object[] args)
    {
        Actor actor = args[0] as Actor;
    
        if (_actors[actor.teamID].Where(a => a.canAct).Count() == 0)
            GlobalEvents.Raise(GlobalEvent.EndTurn);
        else
        {
            if (actor.teamID == 0)
                Player.SelectActor(_actors[actor.teamID].Where(a => a.canAct).First());
        }
    }

    static void MoveToNextTurn()
    {
        if (turnIndex == _actors.Count - 1)
            turnIndex = 0;
        else
            turnIndex++;

        for (int i = 0; i < _actors[turnIndex].Count; i++)
            _actors[turnIndex][i].OnNewTurn();
    
        GlobalEvents.Raise(GlobalEvent.NewTurn, turnIndex);

        if (turnIndex > 0)
            Object.FindObjectOfType<Loader>().StartCoroutine(AI(0));
    }

    public static Actor GetRandom(int teamID)
    {
        return _actors[teamID].Random();
    }

    static IEnumerator AI(int index)
    {
        (_actors[turnIndex][index] as NPActor).PlotMove();

        while (_actors[turnIndex][index].commandCount > 0 || _actors[turnIndex][index].isBusy)
            yield return null;

        if (index < _actors[turnIndex].Count - 1)
            Object.FindObjectOfType<Loader>().StartCoroutine(AI(index + 1));
        else
            new EndTurnCommand().Execute();
    }
}
