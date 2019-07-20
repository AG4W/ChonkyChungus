using UnityEngine;

using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class NPActor : Actor
{
    Dictionary<Tile, float> _attackMap = new Dictionary<Tile, float>();

    public override void OnNewTurn()
    {
        base.OnNewTurn();
        StartCoroutine(CalculateMoveDelayed());
    }

    void CalculateMove()
    {
        Actor p = base.visibleActors.FirstOrDefault();

        if (p == null)
            new EndTurnCommand(this);
        //do I need to move or am I in range?
        //else if (base.CanAttack(p, false))
        //{
        //    new AttackCommand(this, p);
        //    new EndTurnCommand(this);
        //}
        else
        {
            new MoveCommand(this, Pathfinder.GetPath(base.GetMap(MapType.Movement), base.GetMap(MapType.Movement).Keys.OrderBy(t => Pathfinder.Distance(this.tile, t)).First(), this.tile));
            new EndTurnCommand(this);
        }
    }
    IEnumerator CalculateMoveDelayed()
    {
        yield return new WaitForSeconds(.1f);

        while (base.isBusy)
            yield return null;

        CalculateMove();
    }
}