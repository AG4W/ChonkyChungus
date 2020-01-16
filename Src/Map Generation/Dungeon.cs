using UnityEngine;

using System.Collections.Generic;
using System.Collections;

public class Dungeon
{
    //dungeon settings will be moved to its own datafile later
    const int MIN_MAP_SIZE = 100;
    const int MAX_MAP_SIZE = 200;

    const int MIN_ROOM_COUNT = 25;
    const int MAX_ROOM_COUNT = 40;

    const int MIN_ROOM_SIZE = 10;
    const int MAX_ROOM_SIZE = 15;

    const int MAX_LOOT_ROOMS = 2;

    LootTable _lootTable;
    List<Connection> _connections;

    public int size { get; private set; }
    public List<Room> rooms { get; private set; }

    public Dungeon()
    {
        this.size = Synched.Next(MIN_MAP_SIZE, MAX_MAP_SIZE + 1);

        this.rooms = new List<Room>();
        _connections = new List<Connection>();

        _lootTable = Resources.LoadAll<LootTable>("Collections/Loot Tables/").Random();

        GlobalEvents.Raise(GlobalEvent.SetLoadingBarProgress, 0f);
        GlobalEvents.Raise(GlobalEvent.SetLoadingBarText, "Initializing grid...");
        Grid.Initialize(size);

        Generate();
        Instantiate();
    }

    void Generate()
    {
        GlobalEvents.Raise(GlobalEvent.SetLoadingBarText, "Generating rooms...");
        GenerateRooms();

        GlobalEvents.Raise(GlobalEvent.SetLoadingBarText, "Generating connections...");
        GenerateConnections();
    }
    
    void GenerateRooms()
    {
        int i = 0;
        int roomCount = Synched.Next(MIN_ROOM_COUNT, MAX_ROOM_COUNT + 1);
        int lootRoomCount = 0;

        int w;
        int h;
        int r;

        int ox;
        int oz;

        int iterations = 0;

        while (rooms.Count < roomCount)
        {
            if (iterations == 5000)
            {
                Debug.LogWarning("Dungeon.Generate() is looping!");
                break;
            }

            w = Synched.Next(MIN_ROOM_SIZE, MAX_ROOM_SIZE + 1);
            h = Synched.Next(MIN_ROOM_SIZE, MAX_ROOM_SIZE + 1);

            while (w % 2 != 0)
                w = Synched.Next(MIN_ROOM_SIZE, MAX_ROOM_SIZE + 1);
            while (h % 2 != 0)
                h = Synched.Next(MIN_ROOM_SIZE, MAX_ROOM_SIZE + 1);

            ox = Synched.Next(1, size - w - 2);
            oz = Synched.Next(1, size - h - 2);

            if (SquareRoom.Validate(ox, oz, i, w, h))
            {
                if (i == 0)
                {
                    rooms.Add(new SquareRoom(ox, oz, i, w, h, RegionProfile.Entrance));
                }
                else if (i >= (roomCount / 2) && lootRoomCount < MAX_LOOT_ROOMS)
                {
                    lootRoomCount++;
                    rooms.Add(new SquareRoom(ox, oz, i, w, h, RegionProfile.Treasure));
                }
                else
                {
                    rooms.Add(new SquareRoom(ox, oz, i, w, h, RegionProfile.Generic));
                }

                i++;
            }

            iterations++;
        }
    }
    void GenerateConnections()
    {
        //Prim's MST alg
        //not really an mst lol
        List<Tile> mst = new List<Tile>();

        for (int i = 0; i < rooms.Count; i++)
        {
            Tile rt = Grid.Get(rooms[i].centerX, rooms[i].centerZ);
            mst.Add(rt);

            Tile closest = null;
            float distance = Mathf.Infinity;
        
            for (int j = 0; j < rooms.Count; j++)
            {
                //ignore self
                if (rooms[i] == rooms[j])
                    continue;

                Tile t = Grid.Get(rooms[j].centerX, rooms[j].centerZ);
                float d = Pathfinder.Distance(rt, t);

                if(closest == null || d < distance)
                {
                    closest = t;
                    distance = d;
                }
            }

            mst.Add(closest);
        }
        for (int i = 1; i < mst.Count; i++)
            GenerateConnection(mst[i - 1], mst[i], Synched.Next(0, 5 + 1) == 5 ? 3 : 2);
    }
    void GenerateConnection(Tile from, Tile to, int thickness)
    {
        List<Tile> tiles = new List<Tile>();

        //randomize which L shape
        if (Synched.Next(0, 1 + 1) == 1)
        {
            tiles.AddRange(CreateHorizontalConnection(from, to, from, thickness));
            tiles.AddRange(CreateVerticalConnection(from, to, to, thickness));
        }
        else
        {
            tiles.AddRange(CreateVerticalConnection(from, to, to, thickness));
            tiles.AddRange(CreateHorizontalConnection(from, to, from, thickness));
        }

        _connections.Add(new Connection(tiles, new ConnectionType[] { ConnectionType.Corridor, ConnectionType.Bridge }.RandomWeighted(75, 25)));
    }
    List<Tile> CreateVerticalConnection(Tile from, Tile to, Tile first, int thickness)
    {
        List<Tile> tiles = new List<Tile>();

        for (int z = Mathf.Min(from.z, to.z); z <= Mathf.Max(from.z, to.z); z++)
        {
            for (int x = first.x; x <= first.x + thickness; x++)
            {
                Tile t = Grid.Get(x, z);
            
                if (t.index != -1)
                    continue;

                tiles.Add(Grid.Get(x, z));
            }
        }

        return tiles;
    }
    List<Tile> CreateHorizontalConnection(Tile from, Tile to, Tile first, int thickness)
    {
        List<Tile> tiles = new List<Tile>();

        for (int x = Mathf.Min(from.x, to.x); x <= Mathf.Max(from.x, to.x); x++)
        {
            for (int z = first.z; z <= first.z + thickness; z++)
            {
                Tile t = Grid.Get(x, z);

                if (t.index != -1)
                    continue;

                tiles.Add(Grid.Get(x, z));
            }
        }
    
        return tiles;
    }

    public void Instantiate()
    {
        //need a dedicated surrogate behaviour for this, this is kinda ugly
        Object.FindObjectOfType<Loader>().StartCoroutine(InstantiateAsync());
    }

    IEnumerator InstantiateAsync()
    {
        GlobalEvents.Raise(GlobalEvent.SetLoadingBarText, "Preparing rooms...");

        for (int i = 0; i < rooms.Count; i++)
        {
            GlobalEvents.Raise(GlobalEvent.SetLoadingBarProgress, Mathf.Lerp(0f, .25f, i / (float)rooms.Count));
            rooms[i].OnInstantiate();
            yield return null;
        }

        GlobalEvents.Raise(GlobalEvent.SetLoadingBarText, "Preparing connections...");

        for (int i = 0; i < _connections.Count; i++)
        {
            GlobalEvents.Raise(GlobalEvent.SetLoadingBarProgress, Mathf.Lerp(.25f, .5f, i / (float)_connections.Count));
            _connections[i].OnInstantiate();
            yield return null;
        }

        GlobalEvents.Raise(GlobalEvent.SetLoadingBarText, "Populating rooms...");

        for (int i = 0; i < rooms.Count; i++)
        {
            GlobalEvents.Raise(GlobalEvent.SetLoadingBarProgress, Mathf.Lerp(.5f, .75f, i / (float)rooms.Count));
            RoomPopulator.Instantiate(rooms[i], _lootTable);
            yield return null;
        }

        GlobalEvents.Raise(GlobalEvent.SetLoadingBarText, "Populating connections...");

        for (int i = 0; i < _connections.Count; i++)
        {
            GlobalEvents.Raise(GlobalEvent.SetLoadingBarProgress, Mathf.Lerp(.75f, 1f, i / (float)_connections.Count));
            RoomPopulator.Instantiate(_connections[i], _lootTable);
            yield return null;
        }

        GlobalEvents.Raise(GlobalEvent.SetLoadingBarText, "Unloading unused assets...");
        //cleanup memory
        Resources.UnloadUnusedAssets();
        yield return new WaitForSecondsRealtime(1f);

        GlobalEvents.Raise(GlobalEvent.SetLoadingBarText, "Generation complete!");
        yield return new WaitForSecondsRealtime(1f);

        GlobalEvents.Raise(GlobalEvent.PCGComplete);
    }
}
