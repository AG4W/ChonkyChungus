using UnityEngine;

using System.Collections.Generic;

public class CursorManager : MonoBehaviour
{
    [SerializeField]Color _walkDistance = Color.yellow;
    [SerializeField]Color _sprintDistance = Color.blue;

    [SerializeField]Texture2D _walk;
    [SerializeField]Texture2D _sprint;
    [SerializeField]Texture2D _target;
    [SerializeField]Texture2D _select;
    [SerializeField]Texture2D _interact;

    GameObject _marker;
    MeshRenderer _markerRenderer;
    LineRenderer _markerLine;
    ScalePulseEffect _markerPulseEffect;

    List<Tile> _currentPath;

    bool _inTargetingMode = false;

    void Awake()
    {
        _marker = GameObject.Find("movementMarker");

        _markerRenderer = _marker.GetComponentInChildren<MeshRenderer>();
        _markerRenderer.material.SetTexture("_BaseColorMap", _walk);
        _markerRenderer.material.SetTexture("_EmissiveColorMap", _walk);

        _markerLine = _marker.GetComponentInChildren<LineRenderer>();
        _markerLine.startWidth = .05f;
        _markerLine.endWidth = .05f;
        _markerPulseEffect = _marker.GetComponentInChildren<ScalePulseEffect>();

        GlobalEvents.Subscribe(GlobalEvent.NewTurn, (object[] args) =>
        {
            _marker.SetActive((int)args[0] == 0);
        });
        GlobalEvents.Subscribe(GlobalEvent.EndTurn, (object[] args) =>
        {
            _marker.SetActive(false);
        });
        GlobalEvents.Subscribe(GlobalEvent.EnterTargetingMode, (object[] args) => {
            _markerRenderer.gameObject.SetActive(false);
            _markerLine.gameObject.SetActive(false);
        });
        GlobalEvents.Subscribe(GlobalEvent.ExitTargetingMode, (object[] args) => {
            _markerRenderer.gameObject.SetActive(true);
            _markerLine.gameObject.SetActive(true);
        });
        GlobalEvents.Subscribe(GlobalEvent.CurrentPathChanged, OnCurrentPathChanged);
        GlobalEvents.Subscribe(GlobalEvent.TogglePathVisiblity, (object[] args) => {
            _markerRenderer.gameObject.SetActive((bool)args[0]);
            _markerLine.gameObject.SetActive((bool)args[0]);
        });
        GlobalEvents.Subscribe(GlobalEvent.ActorVitalChanged, (object[] args) => {
            if (args[0] is Actor a && a == Player.selectedActor)
                UpdateMarkerLine();
        });
        GlobalEvents.Subscribe(GlobalEvent.ActorMoveStart, (object[] args) => {
            _marker.SetActive(false);
        });
        GlobalEvents.Subscribe(GlobalEvent.ActorMoveEnd, (object[] args) => {
            _marker.SetActive(true);
        });
    }

    void OnCurrentPathChanged(object[] args)
    {
        _currentPath = args[0] as List<Tile>;

        UpdateMarkerColor();
        UpdateMarkerIcon();
        UpdateMarkerLine();

        _marker.transform.position = _currentPath.Last().position;
    }

    void UpdateMarkerColor()
    {
        Color color = Color.magenta;

        if (_inTargetingMode)
            color = Color.red;
        else
        {
            if (_currentPath.Last().status == TileStatus.Vacant)
                color = _currentPath.Count > Player.selectedActor.data.GetStat(StatType.WalkRange).GetValue() ? _sprintDistance : _walkDistance;
            else if (_currentPath.Last().entity != null)
            {
                if (_currentPath.Last().entity is Actor)
                {
                    Actor a = _currentPath.Last().entity as Actor;

                    //interact with friendlies? idk
                    if (a.teamID == Player.selectedActor.teamID)
                        color = Color.green;
                    //else if (Player.actor.CanAttack(a, false))
                    //    c = Color.red;
                    else
                        color = Color.red;
                }
                else
                    color = Color.cyan;
            }
        }

        _markerRenderer.material.SetColor("_BaseColor", color);
        _markerRenderer.material.SetColor("_EmissiveColor", color);
    }
    void UpdateMarkerIcon()
    {
        Texture2D icon = null;

        if (_inTargetingMode)
            icon = _target;
        else
        {
            if (_currentPath.Last().status == TileStatus.Vacant)
                icon = _currentPath.Count > Player.selectedActor.data.GetStat(StatType.WalkRange).GetValue() ? _sprint : _walk;
            else if (_currentPath.Last().entity != null)
            {
                if (_currentPath.Last().entity is Actor)
                {
                    Actor a = _currentPath.Last().entity as Actor;

                    //interact with friendlies? idk
                    if (a.teamID == Player.selectedActor.teamID)
                        icon = _select;
                    //else if (Player.actor.CanAttack(a, false))
                    //    c = Color.red;
                    else
                        icon = _target;
                }
                else
                    icon = _interact;
            }
        }

        _markerRenderer.material.SetTexture("_BaseColorMap", icon);
        _markerRenderer.material.SetTexture("_EmissiveColorMap", icon);
    }
    void UpdateMarkerLine()
    {
        if (_currentPath == null)
            return;

        _markerLine.startColor = _currentPath.Count > Player.selectedActor.data.GetStat(StatType.WalkRange).GetValue() ? _sprintDistance : _walkDistance;
        _markerLine.endColor = _currentPath.Count > Player.selectedActor.data.GetStat(StatType.WalkRange).GetValue() ? _sprintDistance : _walkDistance;
        _markerLine.positionCount = _currentPath.Count + 1;
        _markerLine.SetPosition(0, Player.selectedActor.tile.position + Vector3.up * .33f);

        for (int i = 0; i < _currentPath.Count; i++)
            _markerLine.SetPosition(i + 1, _currentPath[i].position + Vector3.up * .33f);
    }
}
