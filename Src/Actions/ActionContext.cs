using System.Collections.Generic;

using MoonSharp.Interpreter;

using ag4w.Actions;

[MoonSharpUserData]
public class ActionContext
{
    public readonly Entity caster;
    public readonly Item item;
    public readonly Action action;

    public List<Tile> tiles { get; private set; } = new List<Tile>();
    public List<Actor> actors { get; private set; } = new List<Actor>();

    public ActionContext(Entity caster, Item item, Action action)
    {
        this.caster = caster;
        this.item = item;
        this.action = action;
    }

    public void SetActors(List<Actor> actors)
    {
        this.actors = actors;
    }
    public void SetTiles(List<Tile> tiles)
    {
        this.tiles = tiles;
    }

    public override string ToString()
    {
        return caster?.ToString() + "\n\n" + item?.ToString() + "\n\n" + action?.ToString();
    }
}
