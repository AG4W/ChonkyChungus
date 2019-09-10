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
    
    LineRenderer _gridOutline;
    List<GameObject> _gridMarkers = new List<GameObject>();

    Camera _camera;

    Tile _current;
    List<Tile> _currentPath;

    KeyCode[] _alphaKeys = new KeyCode[]
    {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5,
        KeyCode.Alpha6,
        KeyCode.Alpha7,
        KeyCode.Alpha8,
        KeyCode.Alpha9,
        KeyCode.Alpha0,
        KeyCode.Pipe
    };

    bool _showSprintRange;
    bool _inTargetingMode = false;
    
    void Awake()
    {
        _marker = GameObject.Find("movementMarker");
        _markerRenderer = _marker.GetComponentInChildren<MeshRenderer>();
        _markerLine = _marker.GetComponentInChildren<LineRenderer>();
        _markerLine.startWidth = .05f;
        _markerLine.endWidth = .05f;
        _markerPulseEffect = _marker.GetComponentInChildren<ScalePulseEffect>();

        _camera = Camera.main;

        GlobalEvents.Subscribe(GlobalEvent.ToggleMovement, (object[] args) => ToggleSprint());

        GlobalEvents.Subscribe(GlobalEvent.NewTurn, (object[] args) =>
        {
            if ((Actor)args[0] != Player.actor)
                return;

            _marker.SetActive(true);
        });
        GlobalEvents.Subscribe(GlobalEvent.EndTurn, (object[] args) =>
        {
            if ((Actor)args[0] != Player.actor)
                return;

            _marker.SetActive(false);
        });
        
        GlobalEvents.Subscribe(GlobalEvent.ActorVitalChanged, (object[] args) =>
        {
            if((Actor)args[0] == Player.actor && (VitalType)args[1] == VitalType.Stamina)
                UpdateMarkerLine();
        });

        GlobalEvents.Subscribe(GlobalEvent.EnterTargetingMode, (object[] args) => SetTargetingMode(true));
        GlobalEvents.Subscribe(GlobalEvent.ExitTargetingMode, (object[] args) => SetTargetingMode(false));
    }
    void Update()
    {
        if (_inTargetingMode)
        {
            //move to next
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Tab))
                GlobalEvents.Raise(GlobalEvent.StepTargetIndex, false);
            if (Input.GetKeyDown(KeyCode.Tab))
                GlobalEvents.Raise(GlobalEvent.StepTargetIndex, true);
            //confirm target selection
            if (Input.GetKeyDown(KeyCode.Space))
                GlobalEvents.Raise(GlobalEvent.ExitTargetingMode, true);
            //exit target selection
            if (Input.GetKeyDown(KeyCode.Escape))
                GlobalEvents.Raise(GlobalEvent.ExitTargetingMode, false);
        }
        else
        {
            if (Player.actor.isBusy || !Player.actor.hasTurn)
                return;

            //iterate alphas
            for (int i = 0; i < _alphaKeys.Length; i++)
                if (Input.GetKeyDown(_alphaKeys[i]))
                    GlobalEvents.Raise(GlobalEvent.HotkeyPressed, i);

            if (Input.GetKeyDown(KeyCode.M))
                Player.data.AddItem(ItemGenerator.Get(ItemType.Weapon, ItemRarity.Common));
            if (Input.GetKeyDown(KeyCode.N))
                Player.data.AddItem(ItemGenerator.Get(ItemType.LightSource, ItemRarity.Common));
            if (Input.GetKeyDown(KeyCode.B))
                Player.data.AddSpell(ItemGenerator.GetRandom());
            if (Input.GetKeyDown(KeyCode.V))
                Player.IncreaseExperience(Player.level * 1000);

            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Q))
                GlobalEvents.Raise(GlobalEvent.SetGridMap, MapType.Movement);
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.W))
                GlobalEvents.Raise(GlobalEvent.SetGridMap, MapType.LineOfSight);

            if (Input.GetKeyDown(KeyCode.I))
                GlobalEvents.Raise(GlobalEvent.ToggleInventory);
            else if (Input.GetKeyDown(KeyCode.C))
                GlobalEvents.Raise(GlobalEvent.ToggleCharacter);

            if (EventSystem.current.IsPointerOverGameObject())
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
                GlobalEvents.Raise(GlobalEvent.ToggleGridVisibility);
        }
    }
    void OnGUI()
    {
        if (!Input.GetKey(KeyCode.L))
            return;

        foreach (Tile tile in Player.actor.GetMap(MapType.LineOfSight).Keys)
        {
            if (Pathfinder.Distance(tile, Player.actor.tile) <= 5 || tile.luminosity >= Player.actor.data.GetStat(StatType.SightThreshold).GetValue())
            {
                string text = tile.ToString();

                var position = _camera.WorldToScreenPoint(tile.position);
                var textSize = GUI.skin.label.CalcSize(new GUIContent(text));
                GUI.Label(new Rect(position.x, Screen.height - position.y, textSize.x, textSize.y), text);
            }
        }
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

    void ToggleSprint()
    {
        _showSprintRange = !_showSprintRange;

        UpdateMarkerLine();
    }

    void SetTargetingMode(bool status)
    {
        _inTargetingMode = status;
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