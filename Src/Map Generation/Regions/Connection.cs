using UnityEngine;

using System.Collections.Generic;

public class Connection : Region
{
    public ConnectionType type { get; private set; }

    public Connection(List<Tile> tiles, ConnectionType type) : base(RegionProfile.Generic)
    {
        base.tiles = tiles;
        base.edges = new List<Tile>();

        for (int i = 0; i < tiles.Count; i++)
            base.edges.Add(tiles[i]);

        for (int i = 0; i < base.tiles.Count; i++)
        {
            base.tiles[i].SetIndex(-2);
            base.tiles[i].SetStatus(TileStatus.Vacant);
        }

        this.type = type;
    }

    public override void OnInstantiate()
    {
        base.template = Resources.LoadAll<PCGTemplate>("PCGTemplates/Connections/" + this.type.ToString()).Random();
        base.OnInstantiate();
    }
}
public enum ConnectionType
{
    Corridor,
    Bridge
}
