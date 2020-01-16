using UnityEngine;

using System.Collections.Generic;
using System.Linq;

public class Region
{
    public List<Tile> tiles { get; protected set; }
    public List<Tile> edges { get; protected set; }

    public RegionProfile profile { get; private set; }
    public PCGTemplate template { get; protected set; }

    public Region(RegionProfile profile)
    {
        this.tiles = new List<Tile>();
        this.edges = new List<Tile>();

        this.profile = profile;
    }

    public virtual void OnInstantiate()
    {
        //load a profile
        if(this.template == null)
            this.template = Resources.LoadAll<PCGTemplate>("PCGTemplates/Rooms/").Where(t => t.profile == this.profile).ToArray().Random();
    }

    public Tile GetRandom(params TileStatus[] filter)
    {
        Tile t = tiles.Random();

        while (!filter.Contains(t.status))
            t = tiles.Random();

        return t;
    }
    public Tile[] GetTiles(params TileStatus[] filter)
    {
        return tiles.Where(t => filter.Contains(t.status)).ToArray();
    }
}
//this is sorta awkward, but whatevs
public enum RegionProfile
{
    //needs to always stay at 0
    Entrance,

    Generic,
    Treasure,
}
