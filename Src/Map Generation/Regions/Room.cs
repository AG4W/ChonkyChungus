public class Room : Region
{
    public int originX { get; private set; }
    public int originZ { get; private set; }

    public int centerX { get; protected set; }
    public int centerZ { get; protected set; }

    public RoomType type { get; private set; }

    protected int index { get; }

    public Room(int ox, int oz, int index, RoomType type)
    {
        this.originX = ox;
        this.originZ = oz;

        this.type = type;

        this.index = index;
    }
}
public enum RoomType
{
    Entrance,
    Commons,
    //Kitchen,
    Library,
    Chapel,
}