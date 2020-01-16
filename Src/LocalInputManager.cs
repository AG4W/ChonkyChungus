using UnityEngine;
using UnityEngine.EventSystems;

using System.Collections.Generic;

public class LocalInputManager : MonoBehaviour
{
    string _debugText;

    int _selectionRangeRestriction = 0;

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
    };

    bool _showSprintRange;
    bool _inTargetingMode = false;
    bool _restrictSelectionToActorsOnly = false;

    void Awake()
    {
        _camera = Camera.main;

        GlobalEvents.Subscribe(GlobalEvent.ToggleMovement, (object[] args) => ToggleSprint());
        GlobalEvents.Subscribe(GlobalEvent.EnterTargetingMode, (object[] args) => _inTargetingMode = true);
        GlobalEvents.Subscribe(GlobalEvent.ExitTargetingMode, (object[] args) => _inTargetingMode = false);
    }
    void Update()
    {
        if (GameManager.turnIndex != 0)
            return;

        if (_inTargetingMode)
        {
            //confirm target selection
            if (Input.GetKeyDown(KeyCode.Space))
                GlobalEvents.Raise(GlobalEvent.ExitTargetingMode, true);
            //exit target selection
            if (Input.GetKeyDown(KeyCode.Escape))
                GlobalEvents.Raise(GlobalEvent.ExitTargetingMode, false);
        }
        else
        {
            UpdateCurrentTile();

            if (Player.selectedActor.isBusy)
                return;

            //iterate alphas
            for (int i = 0; i < _alphaKeys.Length; i++)
                if (Input.GetKeyDown(_alphaKeys[i]))
                    GlobalEvents.Raise(GlobalEvent.HotkeyPressed, i);

            if (Input.GetKeyDown(KeyCode.M))
                Player.selectedActor.data.SetEquipment(ItemGenerator.GetWeapon((WeaponType)Synched.Next(0, System.Enum.GetNames(typeof(WeaponType)).Length), ItemRarity.Common));
            if (Input.GetKeyDown(KeyCode.N))
                Player.selectedActor.data.SetEquipment(ItemGenerator.GetArmour((EquipSlot)Synched.Next(2, System.Enum.GetNames(typeof(EquipSlot)).Length), ItemRarity.Common));
            if (Input.GetKeyDown(KeyCode.B))
                Player.selectedActor.data.AddSpell(ItemGenerator.GetSpellRandom());
            if (Input.GetKeyDown(KeyCode.V))
                Player.selectedActor.data.SetEquipment(ItemGenerator.GetLightSource(ItemRarity.Common));
            if (Input.GetKeyDown(KeyCode.K))
                Player.selectedActor.SetItemIfOpen(ItemGenerator.GetPotion(ItemRarity.Common));
            if (Input.GetKeyDown(KeyCode.I))
                GlobalEvents.Raise(GlobalEvent.ToggleInventory);
            else if (Input.GetKeyDown(KeyCode.C))
                GlobalEvents.Raise(GlobalEvent.ToggleCharacter);

            if (EventSystem.current.IsPointerOverGameObject())
                return;

            if (Input.GetKeyDown(KeyCode.Mouse0))
                ProcessLeftClick();
            else if (Input.GetKeyDown(KeyCode.Mouse1))
                ProcessRightClick();
            else if (Input.GetKeyDown(KeyCode.Space))
                new EndTurnCommand().Execute();
            else if (Input.GetKeyDown(KeyCode.LeftShift))
                GlobalEvents.Raise(GlobalEvent.ToggleMovement);
            else if (Input.GetKeyDown(KeyCode.Tab))
                GlobalEvents.Raise(GlobalEvent.ToggleGridVisibility);
        }
    }
    void OnGUI()
    {
        if(_current?.entity != null)
        {
            string text = _current.entity.interactionHeader;

            var position = _camera.WorldToScreenPoint(_current.entity.tile.position + _current.entity.headerOffset);
            var textSize = GUI.skin.label.CalcSize(new GUIContent(text));

            GUI.Label(new Rect(position.x, Screen.height - position.y, textSize.x, textSize.y), text);
        }

        if (Input.GetKey(KeyCode.L))
        {
            foreach (Tile tile in Player.selectedActor.GetMap(MapType.LineOfSight).Keys)
            {
                if (Pathfinder.Distance(tile, Player.selectedActor.tile) <= 5 || tile.luminosity >= Player.selectedActor.data.GetStat(StatType.SightThreshold).GetValue())
                {
                    string text = tile.ToString();

                    var position = _camera.WorldToScreenPoint(tile.position);
                    var textSize = GUI.skin.label.CalcSize(new GUIContent(text));
                    GUI.Label(new Rect(position.x, Screen.height - position.y, textSize.x, textSize.y), text);
                }
            }
        }
    }
    
    void ProcessLeftClick()
    {
        if (_current.status == TileStatus.Vacant)
            new MoveCommand(Player.selectedActor, _currentPath);
        else if (_current.entity != null)
        {
            if (_current.entity is Actor a)
            {
                //interact with friendlies? idk
                if (a.teamID == Player.selectedActor.teamID && a.data.GetVital(VitalType.Stamina).current > 0)
                    Player.SelectActor(a);
            }
            else if (Player.selectedActor.CanInteract(_current.entity, true))
                new InteractCommand(Player.selectedActor, _current.entity);
        }
    }
    void ProcessRightClick()
    {
        if(_current.entity != null)
            _current.entity.Examine(Player.selectedActor);
        else
            new RotateCommand(Player.selectedActor, _current.position);
    }

    void UpdateCurrentTile()
    {
        Tile t = MouseToTile();
        
        if (t != null && t != _current)
        {
            //no range restriction
            if(_selectionRangeRestriction == 0)
            {
                if (_restrictSelectionToActorsOnly)
                {
                    if (t.entity != null && t.entity is Actor)
                        UpdateCurrentPath(t);
                }
                else
                {
                    if (t.status != TileStatus.Blocked)
                        UpdateCurrentPath(t);
                }
            }
            else
            {
                if (_restrictSelectionToActorsOnly)
                {
                    if (t.entity != null && t.entity is Actor && Pathfinder.Distance(Player.selectedActor.tile, t) <= _selectionRangeRestriction)
                        UpdateCurrentPath(t);
                }
                else
                {
                    if (t.status != TileStatus.Blocked && Pathfinder.Distance(Player.selectedActor.tile, t) <= _selectionRangeRestriction)
                        UpdateCurrentPath(t);
                }
            }
        }
    }
    void UpdateCurrentPath(Tile tile)
    {
        _currentPath = Pathfinder.GetPath(Player.selectedActor.GetMap(MapType.Movement), tile, Player.selectedActor.tile);

        if (_currentPath == null || _currentPath.Count == 0)
            return;

        _current = _currentPath.Last();

        GlobalEvents.Raise(GlobalEvent.CurrentPathChanged, _currentPath);
    }

    void ToggleSprint()
    {
        _showSprintRange = !_showSprintRange;
    }

    Tile MouseToTile()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        Vector3 ip = ray.origin - (ray.direction / ray.direction.y) * ray.origin.y;

        int x = Mathf.RoundToInt(ip.x);
        int z = Mathf.RoundToInt(ip.z);
    
        if (x < 0 || x > Grid.size - 1 || z < 0 || z > Grid.size - 1 || Grid.Get(x, z).status == TileStatus.Blocked)
            return null;

        return Grid.Get(x, z);
    }
}