using UnityEngine;
using UnityEngine.EventSystems;

using System.Collections.Generic;
using System.Linq;

public class LocalInputManager : MonoBehaviour
{
    string _debugText;

    [SerializeField]Color _main;
    [SerializeField]Color _secondary;

    [SerializeField]float _gridTransparencyDivider = 25f;

    GameObject _marker;
    MeshRenderer _markerRenderer;
    LineRenderer _markerLine;
    ScalePulseEffect _markerPulseEffect;

    GameObject _grid;
    GameObject _gridMarker;
    LineRenderer _gridOutline;
    List<GameObject> _gridMarkers = new List<GameObject>();

    Camera _camera;

    Tile _current;
    List<Tile> _currentPath;

    bool _showSprintRange;
    
    void Awake()
    {
        _marker = GameObject.Find("movementMarker");
        _markerRenderer = _marker.GetComponentInChildren<MeshRenderer>();
        _markerLine = _marker.GetComponentInChildren<LineRenderer>();
        _markerLine.startWidth = .05f;
        _markerLine.endWidth = .05f;
        _markerPulseEffect = _marker.GetComponentInChildren<ScalePulseEffect>();

        _grid = new GameObject("grid visuals root");
        _gridMarker = Resources.Load<GameObject>("gridMarker");

        _camera = Camera.main;

        GlobalEvents.Subscribe(GlobalEvent.ToggleMovement, (object[] args) => ToggleSprint());

        GlobalEvents.Subscribe(GlobalEvent.NewTurn, (object[] args) =>
        {
            if ((Actor)args[0] != Player.actor)
                return;

            _grid.SetActive(true);
            _marker.SetActive(true);
        });
        GlobalEvents.Subscribe(GlobalEvent.EndTurn, (object[] args) =>
        {
            if ((Actor)args[0] != Player.actor)
                return;

            _grid.SetActive(false);
            _marker.SetActive(false);
        });

        GlobalEvents.Subscribe(GlobalEvent.ActorMapChanged, (object[] args) =>
        {
            if ((Actor)args[0] == Player.actor && (MapType)args[1] == MapType.Movement)
                UpdateGridMarkers(Player.actor.GetMap(MapType.Movement));
        });
        GlobalEvents.Subscribe(GlobalEvent.ActorVitalChanged, (object[] args) =>
        {
            if((Actor)args[0] == Player.actor && (VitalType)args[1] == VitalType.Stamina)
                UpdateMarkerLine();
        });
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
            Player.data.AddItem(ItemGenerator.Get(ItemType.Weapon, ItemRarity.Common));
        if (Input.GetKeyDown(KeyCode.Alpha2))
            Player.data.AddItem(ItemGenerator.Get(ItemType.LightSource, ItemRarity.Common));
        if (Input.GetKeyDown(KeyCode.Alpha8))
            Player.IncreaseExperience(Player.level * 1000);

        if (Input.GetKeyDown(KeyCode.I))
            GlobalEvents.Raise(GlobalEvent.ToggleInventory);
        else if (Input.GetKeyDown(KeyCode.C))
            GlobalEvents.Raise(GlobalEvent.ToggleCharacter);
        else if (Input.GetKeyDown(KeyCode.N))
            GlobalEvents.Raise(GlobalEvent.ToggleSkills);

        if (EventSystem.current.IsPointerOverGameObject())
            return;
    
        if (Player.actor.isBusy || !Player.actor.hasTurn)
            return;

        UpdateCurrentTile();

        if (Input.GetKeyDown(KeyCode.Mouse0))
            ProcessLeftClick();
        else if (Input.GetKeyDown(KeyCode.Mouse1))
            ProcessRightClick();
        else if (Input.GetKeyDown(KeyCode.Space))
            new EndTurnCommand(Player.actor);
        else if (Input.GetKeyDown(KeyCode.LeftShift))
            GlobalEvents.Raise(GlobalEvent.ToggleMovement);
        else if (Input.GetKeyDown(KeyCode.Tab))
            ToggleGridMarkers();
    }
    void OnGUI()
    {
        if (_current == null)
            return;

        _debugText = _current.ToString();
    
        var position = _camera.WorldToScreenPoint(_current.position);
        var textSize = GUI.skin.label.CalcSize(new GUIContent(_debugText));
        GUI.Label(new Rect(position.x, Screen.height - position.y, textSize.x, textSize.y), _debugText);
    }

    void ProcessLeftClick()
    {
        if (_current.isTraversable)
            new MoveCommand(Player.actor, _currentPath);
        else if (_current.entity != null)
        {
            if (_current.entity is Actor a)
            {
                //interact with friendlies? idk
                if(a.teamID == Player.actor.teamID)
                {
                    Debug.Log("Clicked on friendly");
                    new InteractCommand(Player.actor, a);
                }
                //else if (Player.actor.CanAttack(a, true))
                //{
                //    Debug.Log("Clicked on enemy");
                //    new AttackCommand(Player.actor, a);
                //}
            }
            else if (Player.actor.CanInteract(_current.entity, true))
            {
                Debug.Log("Clicked on interactable");
                new InteractCommand(Player.actor, _current.entity);
            }
        }
    }
    void ProcessRightClick()
    {
        if(_current.entity != null)
            _current.entity.Examine(Player.actor);
        else
            new RotateCommand(Player.actor, _current.position);
    }

    void UpdateCurrentTile()
    {
        Tile t = MouseToTile();
        
        if (t != null)
        {
            _current = t;

            if (t.isTraversable)
            {
                if (!Player.actor.GetMap(MapType.Movement).ContainsKey(t))
                    return;

                _currentPath = Pathfinder.GetPath(Player.actor.GetMap(MapType.Movement), t, Player.actor.tile);

                UpdateMovementMarker();
            }
            else if (t.entity != null)
            {
                Tile cn = Grid.GetNeighbours(t, true).OrderBy(tile => Pathfinder.Distance(Player.actor.tile, tile)).FirstOrDefault();

                if (cn != null)
                {
                    _currentPath = Pathfinder.GetPath(Player.actor.GetMap(MapType.Movement), t, Player.actor.tile);

                    UpdateMovementMarker();
                }
            }
        }
    }
    void UpdateMovementMarker()
    {
        _marker.transform.position = _current.position;
    
        Color c = Color.magenta;
    
        if (_current.isTraversable)
        {
            _markerPulseEffect.OnTraversable();

            c = _currentPath.Count > Player.data.GetStat(StatType.WalkRange).GetValue() ? _secondary : _main;
        }
        else if (_current.entity != null)
        {
            _markerPulseEffect.OnInteractable();

            if (_current.entity is Actor)
            {
                Actor a = _current.entity as Actor;

                //interact with friendlies? idk
                if (a.teamID == Player.actor.teamID)
                    c = Color.green;
                //else if (Player.actor.CanAttack(a, false))
                //    c = Color.red;
                else
                    c = Color.yellow;
            }
            else
                c = Color.yellow;
        }
        else
            c = Color.red;
    
        _markerRenderer.material.color = c;
        _markerRenderer.material.SetColor("_EmissiveColor", c * 3f);

        UpdateMarkerLine();
    }
    void UpdateMarkerLine()
    {
        if (_currentPath == null)
            return;

        _markerLine.startColor = _currentPath.Count > Player.data.GetStat(StatType.WalkRange).GetValue() ? _secondary : _main;
        _markerLine.endColor = _currentPath.Count > Player.data.GetStat(StatType.WalkRange).GetValue() ? _secondary : _main;
        _markerLine.positionCount = _currentPath.Count + 1;
        _markerLine.SetPosition(0, Player.actor.tile.position + Vector3.up * .33f);

        for (int i = 0; i < _currentPath.Count; i++)
            _markerLine.SetPosition(i + 1, _currentPath[i].position + Vector3.up * .33f);
    }
    void UpdateGridMarkers(Dictionary<Tile, float> map)
    {
        for (int i = 0; i < _gridMarkers.Count; i++)
            Destroy(_gridMarkers[i]);
    
        _gridMarkers.Clear();
    
        MeshRenderer mr;
        Color c;
    
        foreach (Tile tile in map.Keys)
        {
            GameObject g = Instantiate(_gridMarker, _grid.transform);
            g.transform.position = tile.position;
        
            c = Pathfinder.Distance(Player.actor.tile, tile) > Player.data.GetStat(StatType.WalkRange).GetValue() ? _secondary : _main;
            c.a = tile.luminosity;
            //c.a /= _gridTransparencyDivider;

            mr = g.GetComponentInChildren<MeshRenderer>();
            mr.material.SetColor("_UnlitColor", c);
            //mr.material.SetColor("_EmissiveColor", mr.material.color * 3f);

            _gridMarkers.Add(g);
        }
    }

    void ToggleSprint()
    {
        _showSprintRange = !_showSprintRange;

        UpdateMarkerLine();
    }
    void ToggleGridMarkers()
    {
        _grid.SetActive(!_grid.activeSelf);
    }

    Tile MouseToTile()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        Vector3 ip = ray.origin - (ray.direction / ray.direction.y) * ray.origin.y;

        int x = (int)ip.x;
        int z = (int)ip.z;
    
        if (x < 0 || x > Grid.size - 1 || z < 0 || z > Grid.size - 1 || Grid.Get(x, z).status == TileStatus.Blocked)
            return null;

        return Grid.Get(x, z);
    }
}
