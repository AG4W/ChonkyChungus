using UnityEngine;

using System.Collections.Generic;

public static class GridManager
{
    static int _mapRange = 50;

    static GameObject _root;
    static GameObject _prefab;

    static List<GameObject> _items = new List<GameObject>();

    static MapType _current = MapType.Movement;

    static Dictionary<Tile, float> _map = new Dictionary<Tile, float>();

    static bool _isHidden = true;
    static bool _hasInitialized = false;

    public static void Initialize()
    {
        if (_hasInitialized)
            return;

        _root = new GameObject("grid root");
        _prefab = Resources.Load<GameObject>("gridMarker");

        GlobalEvents.Subscribe(GlobalEvent.SetGridMap, SetMap);
        GlobalEvents.Subscribe(GlobalEvent.ToggleGridVisibility, (object[] args) => Toggle());

        GlobalEvents.Subscribe(GlobalEvent.ActorMapChanged, (object[] args) =>
        {
            if ((Actor)args[0] == Player.actor && (MapType)args[1] == _current)
                Repaint();
        });

        GlobalEvents.Subscribe(GlobalEvent.NewTurn, (object[] args) =>
        {
            if ((Actor)args[0] != Player.actor)
                return;

            _root.SetActive(_isHidden);
        });
        GlobalEvents.Subscribe(GlobalEvent.EndTurn, (object[] args) =>
        {
            if ((Actor)args[0] != Player.actor)
                return;
        
            _root.SetActive(false);
        });

        _hasInitialized = true;
    }

    //set grid to whatever map player cares about
    static void SetMap(object[] args)
    {
        //if clicking on the same button, just toggle
        if (_current == (MapType)args[0])
            Toggle();
        //if switching
        else
        {
            _current = (MapType)args[0];

            Repaint();

            //if we're hidden, display
            if (_isHidden)
                Toggle();
        }
    }
    static void Repaint()
    {
        UpdateMap();
        Clear();

        Color c = Map.GetColor(_current);
        MeshRenderer mr;

        switch (_current)
        {
            case MapType.Movement:
                foreach(Tile tile in _map.Keys)
                {
                    if (Pathfinder.Distance(tile, Player.actor.tile) > Player.actor.data.GetStat(StatType.SprintRange).GetValue())
                        continue;

                    GameObject t = Object.Instantiate(_prefab, tile.position, Quaternion.identity, _root.transform);
                
                    c.a = Pathfinder.Distance(tile, Player.actor.tile) <= Player.actor.data.GetStat(StatType.WalkRange).GetValue() ? .5f : .25f;

                    mr = t.GetComponentInChildren<MeshRenderer>();
                    mr.material.SetColor("_UnlitColor", c);

                    _items.Add(t);
                }
                break;
            case MapType.LineOfSight:
                foreach(Tile tile in _map.Keys)
                {
                    if(Pathfinder.Distance(tile, Player.actor.tile) <= 5 || tile.luminosity >= Player.actor.data.GetStat(StatType.SightThreshold).GetValue())
                    {
                        GameObject t = Object.Instantiate(_prefab, tile.position, Quaternion.identity, _root.transform);

                        c.a = Pathfinder.Distance(tile, Player.actor.tile) <= 5 ? 1f : tile.luminosity;

                        mr = t.GetComponentInChildren<MeshRenderer>();
                        mr.material.SetColor("_UnlitColor", c);

                        _items.Add(t);
                    }
                }
                break;
        }
    }
    static void UpdateMap()
    {
        if(_current == MapType.LineOfSight)
            _map = Pathfinder.LineOfSight(_map, Player.actor.tile, (int)Player.actor.data.GetStat(StatType.SightRange).GetValue());
        else
            _map = Pathfinder.Dijkstra(_map, Player.actor.tile, _mapRange, _current == MapType.Movement);
    }

    static void Toggle()
    {
        _isHidden = !_isHidden;
        _root.SetActive(_isHidden);
    }
    static void Clear()
    {
        for (int i = 0; i < _items.Count; i++)
            Object.Destroy(_items[i]);
    }
}