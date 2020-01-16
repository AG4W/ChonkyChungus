using UnityEngine;

using System.Collections.Generic;
using System.Linq;

public class NPActor : Actor
{
    Dictionary<Tile, float> _attackMap = new Dictionary<Tile, float>();

    public override void OnNewTurn()
    {
        base.OnNewTurn();
    }

    public void PlotMove()
    {
        Debug.Log("NPActor plotting move...");

        //pick random player actor
        Actor pa = GameManager.GetRandom(0);
        Tile t = this.GetMap(MapType.Movement).Keys.OrderBy(tile => Pathfinder.Distance(tile, pa.tile)).FirstOrDefault();

        new MoveCommand(this, Pathfinder.GetPath(base.GetMap(MapType.Movement), t, this.tile));
    }
}