public class Room : Region
{
    public int originX { get; private set; }
    public int originZ { get; private set; }

    public int centerX { get; protected set; }
    public int centerZ { get; protected set; }

    protected int index { get; }

    public Room(int ox, int oz, int index, RegionProfile profile) : base(profile)
    {
        this.originX = ox;
        this.originZ = oz;

        this.index = index;
    }
}
