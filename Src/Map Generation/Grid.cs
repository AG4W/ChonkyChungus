using System.Collections.Generic;
using System.Linq;

public static class Grid
{
    static Tile[,] _tiles;

    public static int size { get; private set; }

    public static void Initialize(int size)
    {
        Grid.size = size;

        _tiles = new Tile[size, size];

        for (int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                _tiles[x, z] = new Tile(x, z, -1);
                _tiles[x, z].SetStatus(TileStatus.Blocked);
            }
        }
    }

    public static Tile Get(int x, int z)
    {
        return _tiles[x, z];
    }
    public static Tile GetRandom(params TileStatus[] filter)
    {
        int iterations = 0;
        Tile t = _tiles[Synched.Next(0, _tiles.GetLength(0)), Synched.Next(0, _tiles.GetLength(1))];
    
        while (!filter.Contains(t.status))
        {
            if (iterations > 5000)
            {
                UnityEngine.Debug.LogWarning("Grid.GetRandom() is probably looping!");
                return null;
            }

            t = _tiles[Synched.Next(0, _tiles.GetLength(0)), Synched.Next(0, _tiles.GetLength(1))];
            iterations++;
        }

        return t;
    }
    public static List<Tile> GetNeighbours(Tile o)
    {
        List<Tile> n = new List<Tile>();

        int ox = (int)o.position.x;
        int oz = (int)o.position.z;

        for (int x = ox - 1; x <= ox + 1; x++)
        {
            for (int z = oz - 1; z <= oz + 1; z++)
            {
                if (x < 0 || x > _tiles.GetLength(0) - 1 || z < 0 || z > _tiles.GetLength(1) - 1 || _tiles[x, z] == o)
                    continue;

                if (_tiles[x, z].status == TileStatus.Blocked)
                    continue;

                n.Add(_tiles[x, z]);
            }
        }

        return n;
    }
}