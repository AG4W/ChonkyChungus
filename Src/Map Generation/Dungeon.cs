using UnityEngine;

using System.Collections.Generic;

public class Dungeon
{
    const int MIN_MAP_SIZE = 150;
    const int MAX_MAP_SIZE = 250;

    const int MIN_ROOM_COUNT = 25;
    const int MAX_ROOM_COUNT = 40;

    const int MIN_ROOM_SIZE = 10;
    const int MAX_ROOM_SIZE = 25;

    int _size;
    List<Corridor> _corridors;

    public List<Room> rooms { get; }

    public Dungeon()
    {
        _size = Synched.Next(MIN_MAP_SIZE, MAX_MAP_SIZE + 1);

        rooms = new List<Room>();
        _corridors = new List<Corridor>();

        Grid.Initialize(_size);

        Generate();
        Instantiate();
    }

    void Generate()
    {
        GenerateRooms();
        GenerateCorridors();
    }

    void GenerateRooms()
    {
        int i = 0;

        int w;
        int h;
        int r;

        int ox;
        int oz;

        int rtr;
        int iterations = 0;

        while (rooms.Count < Synched.Next(MIN_ROOM_COUNT, MAX_ROOM_COUNT + 1))
        {
            if (iterations == 5000)
            {
                Debug.LogWarning("Dungeon.Generate() is looping!");
                break;
            }

            rtr = Synched.Next(0, 3 + 1);

            //if(rtr == 3)
            //{
            //    r = Synched.Next(MIN_ROOM_SIZE, MAX_ROOM_SIZE + 1);

            //    while (r % 2 != 0)
            //        r = Synched.Next(MIN_ROOM_SIZE, MAX_ROOM_SIZE + 1);

            //    ox = Synched.Next(1, _size - r - 2);
            //    oz = Synched.Next(1, _size - r - 2);

            //    if (CircleRoom.Validate(ox, oz, i, r))
            //    {
            //        rooms.Add(new CircleRoom(ox, oz, i, r, i == 0 ? RoomType.Entrance : (RoomType)Synched.Next(1, System.Enum.GetNames(typeof(RoomType)).Length)));
            //        i++;
            //    }
            //}
            //else
            //{
                w = Synched.Next(MIN_ROOM_SIZE, MAX_ROOM_SIZE + 1);
                h = Synched.Next(MIN_ROOM_SIZE, MAX_ROOM_SIZE + 1);

                while (w % 2 != 0)
                    w = Synched.Next(MIN_ROOM_SIZE, MAX_ROOM_SIZE + 1);
                while (h % 2 != 0)
                    h = Synched.Next(MIN_ROOM_SIZE, MAX_ROOM_SIZE + 1);

                ox = Synched.Next(1, _size - w - 2);
                oz = Synched.Next(1, _size - h - 2);

                if (SquareRoom.Validate(ox, oz, i, w, h))
                {
                    rooms.Add(new SquareRoom(ox, oz, i, w, h, i == 0 ? RoomType.Entrance : (RoomType)Synched.Next(1, System.Enum.GetNames(typeof(RoomType)).Length)));
                    i++;
                }
            //}

            iterations++;
        }
    }
    void GenerateCorridors()
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
            GenerateCorridor(mst[i - 1], mst[i], Synched.Next(0, 5 + 1) == 5 ? 3 : 2);
    }
    void GenerateCorridor(Tile from, Tile to, int thickness)
    {
        List<Tile> tiles = new List<Tile>();
    
        //randomize which L shape
        if(Synched.Next(0, 1 + 1) == 1)
        {
            tiles.AddRange(CreateHorizontalCorridor(from, to, from, thickness));
            tiles.AddRange(CreateVerticalCorridor(from, to, to, thickness));
        }
        else
        {
            tiles.AddRange(CreateVerticalCorridor(from, to, to, thickness));
            tiles.AddRange(CreateHorizontalCorridor(from, to, from, thickness));
        }
    
        _corridors.Add(new Corridor(tiles));
    }
    List<Tile> CreateVerticalCorridor(Tile from, Tile to, Tile first, int thickness)
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
    List<Tile> CreateHorizontalCorridor(Tile from, Tile to, Tile first, int thickness)
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
        for (int i = 0; i < rooms.Count; i++)
            rooms[i].Instantiate();
        for (int i = 0; i < _corridors.Count; i++)
            _corridors[i].Instantiate();

        for (int i = 0; i < rooms.Count; i++)
            RoomPopulator.Populate(rooms[i]);
    }
}
