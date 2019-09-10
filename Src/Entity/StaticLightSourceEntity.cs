using UnityEngine;

using System.Collections.Generic;

public class StaticLightSourceEntity : ToggleableEntity
{
    [SerializeField]int _range = 15;

    bool _isLit = false;

    Dictionary<Tile, float> _map = new Dictionary<Tile, float>();

    void Start()
    {
        _map = Pathfinder.Dijkstra(_map, this.tile, _range, false);
    }

    public override void Interact(Actor interactee)
    {
        base.Interact(interactee);
        Toggle();
    }

    void Toggle()
    {
        _isLit = !_isLit;

        foreach (Tile tile in _map.Keys)
        {
            if (_isLit)
                tile.AddLuminosity(EvaluateLuminosity(tile));
            else
                tile.SubtractLuminosity(EvaluateLuminosity(tile));
        }
    }
    float EvaluateLuminosity(Tile target)
    {
        return 1 - (Pathfinder.Distance(base.tile, target) / (float)_range);
    }
}
