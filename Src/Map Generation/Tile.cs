using UnityEngine;

public class Tile
{
    GameObject _prefab;

    public int x { get; private set; }
    public int z { get; private set; }
    public int index { get; private set; }

    public float luminosity { get; private set; }

    public Entity entity { get; private set; }

    public TileStatus status { get; private set; }
    public Vector3 position { get; private set; }
    public bool isTraversable
    {
        get
        {
            return this.status == TileStatus.Vacant && this.entity == null;
        }
    }

    public Tile(int x, int z, int i)
    {
        this.x = x;
        this.z = z;

        this.index = i;
        this.status = TileStatus.Blocked;
        this.position = new Vector3(x, 0, z);
    }

    public void SetIndex(int i)
    {
        this.index = i;
    }
    public void SetStatus(TileStatus status)
    {
        this.status = status;
    }
    public void SetEntity(Entity e)
    {
        this.entity = e;
    }

    public void AddLuminosity(float value)
    {
        this.luminosity = Mathf.Clamp01(this.luminosity + value);
    }
    public void SubtractLuminosity(float value)
    {
        this.luminosity = Mathf.Clamp01(this.luminosity - value);
    }

    public GameObject Instantiate(GameObject prefab)
    {
        return _prefab = Object.Instantiate(prefab, this.position, Quaternion.identity, null);
    }

    public override string ToString()
    {
        return
            "Tile <color=blue>" + x + "</color>, <color=red>" + 0 + "</color>, <color=green>" + z + "</color>\n" +
            "Luminosity: " + (luminosity * 100f).ToString("#0.0") + "%\n" +
            "Player Can See: " + ((luminosity * 100f) > Player.data.GetStat(StatType.SightThreshold).GetValue()) + "\n" +
            "Distance from Player: " + Pathfinder.Distance(this, Player.actor.tile) + "\n" +
            "Entity: " + (entity == null ? "Nothing" : entity.name) + "\n" +
            "Is Traversable? " + isTraversable;
    }
}
public enum TileStatus
{
    Blocked,
    Vacant
}