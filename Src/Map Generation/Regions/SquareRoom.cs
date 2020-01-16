public class SquareRoom : Room
{
    public int width { get; private set; }
    public int height { get; private set; }

    public SquareRoom(int ox, int oz, int index, int w, int h, RegionProfile profile) : base(ox, oz, index, profile)
    {
        this.width = w;
        this.height = h;

        base.centerX = base.originX + (w / 2);
        base.centerZ = base.originZ + (h / 2);

        for (int x = base.originX; x <= base.originX + w; x++)
        {
            for (int z = base.originZ; z <= base.originZ + h; z++)
            {
                Tile t = Grid.Get(x, z);

                t.SetIndex(base.index);
                t.SetStatus(TileStatus.Vacant);
                base.tiles.Add(t);

                if (x == base.originX || x == base.originX + w || z == base.originZ || z == base.originZ + h)
                    base.edges.Add(t);
            }
        }
    }

    public static bool Validate(int ox, int oz, int index, int w, int h)
    {
        for (int x = ox; x <= ox + w; x++)
            for (int z = oz; z <= oz + h; z++)
                if (x < 0 || x > Grid.size - 1 || z < 0 || z > Grid.size - 1 || Grid.Get(x, z).index != -1)
                    return false;

        return true;
    }
}
