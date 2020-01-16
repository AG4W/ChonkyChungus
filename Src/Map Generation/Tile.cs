using UnityEngine;

using System.Collections.Generic;

using MoonSharp.Interpreter;

[MoonSharpUserData]
public class Tile
{
    public int x { get; private set; }
    public int z { get; private set; }
    public int index { get; private set; }

    public float luminosity { get; private set; }

    public Entity entity { get; private set; }
    public List<TileEffect> effects { get; private set; } = new List<TileEffect>();

    public TileStatus status { get; private set; }

    public Vector3 position { get; private set; }
    public bool blocksLineOfSight { get; private set; }

    public Tile(int x, int z, int i)
    {
        this.x = x;
        this.z = z;

        this.index = i;
        this.status = TileStatus.Occupied;
        this.position = new Vector3(x, 0, z);

        GlobalEvents.Subscribe(GlobalEvent.NewTurn, (object[] args) => TickEffects());
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
    public void SetBlocksLineOfSight(bool blocksLineOfSight)
    {
        this.blocksLineOfSight = blocksLineOfSight;
    }

    public void AddLuminosity(float value)
    {
        this.luminosity = Mathf.Clamp01(this.luminosity + value);
    }
    public void SubtractLuminosity(float value)
    {
        this.luminosity = Mathf.Clamp01(this.luminosity - value);
    }

    public void AddEffect()
    {
        AddEffect(Resources.Load<TileEffectTemplate>("Templates/Tile Effects/Fire").Instantiate());
    }
    public void AddEffect(TileEffect effect)
    {
        this.effects.Add(effect);

        effect.SetTile(this);
        effect.OnComplete += OnEffectComplete;
        effect.Instantiate(this.position, Quaternion.identity, null);
    }

    void TickEffects()
    {
        for (int i = 0; i < this.effects.Count; i++)
            this.effects[i].Tick();
    }

    void OnEffectComplete(TileEffect te)
    {
        this.effects.Remove(te);

        te.SetTile(null);
        te.OnComplete -= OnEffectComplete;
    }

    public List<Tile> GetNeighbours()
    {
        List<Tile> neighbours = new List<Tile>();

        for (int xAround = x - 1; xAround <= x + 1; xAround++)
        {
            for (int zAround = z - 1; zAround <= z + 1; zAround++)
            {
                if (xAround < 0 || xAround > Grid.size || zAround < 0 || zAround > Grid.size || (xAround == x && zAround == z))
                    continue;
                else
                    neighbours.Add(Grid.Get(xAround, zAround));
            }
        }

        return neighbours;
    }

    public override string ToString()
    {
        return "Tile <color=blue>" + x + "</color>, <color=red>" + 0 + "</color>, <color=green>" + z + "</color>\n" +
            "Luminosity: " + (luminosity * 100f).ToString("#0.0") + "%\n" +
            "Player Can See: " + (Pathfinder.Distance(this, Player.selectedActor.tile) <= 5 || this.luminosity >= Player.selectedActor.data.GetStat(StatType.SightThreshold).GetValue() ? "<color=green>true</color>" : "<color=red>false</color>");

        /*return
            "Tile <color=blue>" + x + "</color>, <color=red>" + 0 + "</color>, <color=green>" + z + "</color>\n" +
            "Luminosity: " + (luminosity * 100f).ToString("#0.0") + "%\n" +
            "Player Can See: " + ((luminosity * 100f) > Player.data.GetStat(StatType.SightThreshold).GetValue()) + "\n" +
            "Distance from Player: " + Pathfinder.Distance(this, Player.actor.tile) + "\n" +
            "Entity: " + (entity == null ? "Nothing" : entity.name) + "\n" +
            "Is Traversable? " + isTraversable + "\n" +
            "Status: " + status.ToString();*/
    }
}
public enum TileStatus
{
    Blocked,
    Occupied,
    Vacant
}