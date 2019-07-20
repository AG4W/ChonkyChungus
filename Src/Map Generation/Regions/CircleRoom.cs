using UnityEngine;

public class CircleRoom : Room
{
    public int radius { get; private set; }

    public CircleRoom(int ox, int oz, int index, int r, RoomType type) : base(ox, oz, index, type)
    {
        this.radius = r;

        Tile c = Grid.Get(ox, oz);

        for (int x = ox - r; x <= ox + r + 1; x++)
        {
            for (int z = oz - r; z <= oz + r + 1; z++)
            {
                if (x < 0 || x > Grid.size - 1 || z < 0 || z > Grid.size - 1 || Vector3.Distance(c.position, Grid.Get(x, z).position) > r)
                    continue;

                base.tiles.Add(Grid.Get(x, z));
            }
        }
    }

    public static bool Validate(int ox, int oz, int index, int r)
    {
        Tile c = Grid.Get(ox, oz);

        for (int x = ox - r; x <= ox + r + 1; x++)
        {
            for (int z = oz - r; z <= oz + r + 1; z++)
            {
                if (x < 0 || x > Grid.size - 1 || z < 0 || z > Grid.size - 1 || Vector3.Distance(c.position, Grid.Get(x, z).position) > r)
                        continue;

                if (Grid.Get(x, z).index != -1)
                    return false;
            }
        }

        return true;
    }
}
