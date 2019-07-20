using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class Actor : Entity
{
    int _seenByCount = 0;

    GameObject _model;
    EquipmentManager _equipmentManager;

    Dictionary<Tile, float>[] _maps;
    Queue<Command> _commands = new Queue<Command>();

    public int teamID { get; private set; }

    public ActorData data { get; private set; }

    public List<Actor> visibleActors { get; } = new List<Actor>();

    public bool hasTurn { get; protected set; }

    public void Initialize(ActorData data, int teamID)
    {
        _maps = new Dictionary<Tile, float>[System.Enum.GetNames(typeof(MapType)).Length];
    
        for (int i = 0; i < _maps.Length; i++)
            _maps[i] = new Dictionary<Tile, float>();

        this.data = data;
        this.data.Initialize(this);

        this.data.OnEquipmentChanged += OnEquipmentChanged;
        this.data.OnAttributeChanged += OnAttributeChanged;
        this.data.OnVitalChanged += OnVitalChanged;

        this.teamID = teamID;

        GlobalEvents.Subscribe(GlobalEvent.LightSourceInfluenceChanged, (object[] args) => {
            if (Pathfinder.Distance(base.tile, (args[0] as LightSource).holder.tile) <= data.GetStat(StatType.SightRange).GetValue())
                UpdateLoSMap();
        });
        GlobalEvents.Subscribe(GlobalEvent.ActorAdded, (object[] args) =>
        {
            if (args[0] is Actor a && a == this && this != Player.actor)
                UpdateActorVisibility();
        });

        Instantiate();
    }

    public void AddCommand(Command c)
    {
        _commands.Enqueue(c);
    }
    IEnumerator CommandLoop()
    {
        while (hasTurn)
        {
            if (!isBusy)
            {
                if (_commands.Count > 0)
                {
                    Command c = _commands.Dequeue();
                    Debug.Log(this.name + " is executing: " + c.GetType());

                    c.Execute();
                }
                else if (data.GetVital(VitalType.Stamina).current == 0)
                    new EndTurnCommand(this);
            }

            yield return null;
        }
    }

    public virtual void OnNewTurn()
    {
        //reset stamina
        data.GetVital(VitalType.Stamina).SetCurrent(data.GetVital(VitalType.Stamina).GetMax());

        this.hasTurn = true;

        GlobalEvents.Raise(GlobalEvent.NewTurn, this);
        StartCoroutine(CommandLoop());
    }
    public virtual void OnEndTurn()
    {
        this.hasTurn = false;

        GlobalEvents.Raise(GlobalEvent.EndTurn, this);
    }

    public void MoveTo(List<Tile> path, bool isSprinting)
    {
        if (path != null && path.Count > 0)
        {
            SetIsBusy(true);
            StartCoroutine(MoveBySpeed(path, isSprinting ? 5f : 2.5f, isSprinting));
        }
    }
    IEnumerator MoveBySpeed(List<Tile> path, float speed, bool isSprinting)
    {
        float s;
        float t;

        Vector3 ld;

        if (this == Player.actor || Player.actor.visibleActors.Contains(this))
            GlobalEvents.Raise(GlobalEvent.SetCameraTrackingTarget, _model);

        GlobalEvents.Raise(GlobalEvent.ActorMoveStart, this);

        StartCoroutine(SetAnimatorFloatOverTime(0f, isSprinting ? .75f : .25f, .25f));
        ld = path[0].position - this.transform.position;

        for (int i = 0; i < path.Count - 1; i++)
        {
            s = (speed / (path[i].position - path[i + 1].position).magnitude) * Time.fixedDeltaTime;
            t = 0;

            ld = path[i + 1].position - path[i].position;

            while (t <= 1f)
            {
                t += s;
                this.transform.position = Vector3.Lerp(path[i].position, path[i + 1].position, t);
                this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(ld, Vector3.up), 10 * Time.deltaTime);
                yield return new WaitForFixedUpdate();
            }
        }

        base.SetPosition(path.Last());
        StartCoroutine(SetAnimatorFloatOverTime(isSprinting ? .75f : .25f, 0f, .25f));

        data.GetVital(VitalType.Stamina).Update(-path.Count);

        GlobalEvents.Raise(GlobalEvent.SetCameraTrackingTarget, null);
        GlobalEvents.Raise(GlobalEvent.ActorMoveEnd, this);
        SetIsBusy(false);
    }
    IEnumerator SetAnimatorFloatOverTime(float from, float to, float duration)
    {
        Animator _animator = this.GetComponentInChildren<Animator>();

        float t = 0f;

        while (t <= duration)
        {
            t += Time.fixedDeltaTime;
        
            _animator.SetFloat("speed", Mathf.Lerp(from, to, t / duration));

            yield return new WaitForFixedUpdate();
        }
    }

    public bool CanInteract(Entity entity, bool raiseDialogues)
    {
        if((int)Pathfinder.Distance(base.tile, entity.tile) > entity.interactRange)
        {
            if (raiseDialogues)
                GlobalEvents.Raise(GlobalEvent.PopupRequested,
                    this.transform.position + Vector3.up * 2,
                    "I cannot reach that!");

            return false;
        }
        if (data.GetVital(VitalType.Stamina).current < entity.interactCost)
        {
            if (raiseDialogues)
                GlobalEvents.Raise(GlobalEvent.PopupRequested,
                    this.transform.position + Vector3.up * 2,
                    "I don't have enough stamina!");

            return false;
        }

        return true;
    }

    void Instantiate()
    {
        _model = Instantiate(data.prefab, this.transform);
        _equipmentManager = _model.GetComponentInChildren<EquipmentManager>();
    }

    public void OnAttack()
    {
        _equipmentManager.OnAttack();
    }
    public void OnAttacked()
    {

    }

    void OnEquipmentChanged(EquipSlot slot)
    {
        _equipmentManager.OnEquipmentChanged(slot, data.GetEquipment(slot));
    }
    void OnAttributeChanged(AttributeType at)
    {
        GlobalEvents.Raise(GlobalEvent.ActorAttributeChanged, this, at);
    }
    void OnVitalChanged(VitalType vt)
    {
        if(vt == VitalType.Health && data.GetVital(vt).current <= 0)
            OnDeath();
        else if(vt == VitalType.Corruption && data.GetVital(vt).current == data.GetVital(vt).GetMax())
            OnDeath();
        else
        {
            GlobalEvents.Raise(GlobalEvent.ActorVitalChanged, this, vt);
        
            UpdateMovementMap();
        }
    }

    void OnDeath()
    {
        GlobalEvents.Raise(GlobalEvent.ActorRemoved, this);
        this.GetComponentInChildren<Animator>().SetTrigger("death");
        this.transform.GetChild(0).SetParent(null);

        Destroy(this.gameObject);
    }

    void UpdateMovementMap()
    {
        SetMap(MapType.Movement, Pathfinder.Dijkstra(this.GetMap(MapType.Movement), this.tile, (int)this.data.GetStat(StatType.SprintRange).GetValue(), true));
        GlobalEvents.Raise(GlobalEvent.ActorMapChanged, this, MapType.Movement);
    }
    void UpdateLoSMap()
    {
        SetMap(MapType.LineOfSight, Pathfinder.Dijkstra(this.GetMap(MapType.LineOfSight), this.tile, (int)this.data.GetStat(StatType.SightRange).GetValue(), false));

        foreach (Tile t in GetMap(MapType.LineOfSight).Keys)
        {
            if(t.entity != null)
            {
                //if actor
                if(t.entity is Actor a)
                {
                    //if we can see whatever is on that tile
                    if(Pathfinder.Distance(t, this.tile) <= this.data.GetStat(StatType.SightRange).GetValue() / 5 || t.luminosity >= this.data.GetStat(StatType.SightThreshold).GetValue())
                    {
                        if (this.visibleActors.IndexOf(a) == -1)
                        {
                            this.visibleActors.Add(a);
                            a.IncrementSeenByCount();
                        }
                    }
                    else
                    {
                        //if we're seeing an actor that we shouldnt see, remove it
                        if(this.visibleActors.IndexOf(a) != -1)
                        {
                            this.visibleActors.Remove(a);
                            a.DecrementSeenByCount();
                        }
                    }
                }
            }
        }

        GlobalEvents.Raise(GlobalEvent.ActorMapChanged, this, MapType.LineOfSight);
    }

    public Dictionary<Tile, float> GetMap(MapType mt)
    {
        return _maps[(int)mt];
    }
    public void SetMap(MapType mt, Dictionary<Tile, float> map)
    {
        _maps[(int)mt] = map;
    }

    public void IncrementSeenByCount()
    {
        _seenByCount++;

        UpdateActorVisibility();
    }
    public void DecrementSeenByCount()
    {
        _seenByCount--;
    
        UpdateActorVisibility();
    }
    void UpdateActorVisibility()
    {
        if (this == Player.actor)
            return;

        _model.SetActive(_seenByCount > 0);

        GlobalEvents.Raise(GlobalEvent.ActorVisibilityChanged, this, _seenByCount > 0);
    }
}
public enum MapType
{
    Movement,
    LineOfSight,
}