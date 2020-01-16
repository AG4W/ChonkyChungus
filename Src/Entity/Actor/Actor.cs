using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using MoonSharp.Interpreter;

[MoonSharpUserData]
public class Actor : Entity
{
    [SerializeField]float _animationBlendSpeed = 5f;
    [SerializeField]float _rotationSpeed = 10f;

    GameObject _model;
    EquipmentManager _equipmentManager;

    Dictionary<Tile, float>[] _maps;
    Queue<Command> _commands = new Queue<Command>();

    public int teamID { get; private set; }
    public int commandCount { get { return _commands.Count; } }
    public int seenByIndex { get; private set; }

    public ActorData data { get; private set; }

    public List<Actor> visibleActors { get; } = new List<Actor>();

    public bool canAct { get; private set; }
    public bool inTwohandedStance { get; protected set; }

    public void Initialize(ActorData data, int teamID)
    {
        _maps = new Dictionary<Tile, float>[System.Enum.GetNames(typeof(MapType)).Length];
    
        for (int i = 0; i < _maps.Length; i++)
            _maps[i] = new Dictionary<Tile, float>();

        this.data = data;

        this.data.OnInventoryChanged += OnInventoryChanged;

        this.data.OnEquipped += OnEquipped;
        this.data.OnUnequipped += OnUnequipped;

        this.data.OnSpellsChanged += OnSpellsChanged;
        this.data.OnAttributeChanged += OnAttributeChanged;
        this.data.OnVitalChanged += OnVitalChanged;

        this.teamID = teamID;

        GlobalEvents.Subscribe(GlobalEvent.LightSourceInfluenceChanged, (object[] args) => UpdateLoSMap());

        GlobalEvents.Subscribe(GlobalEvent.ActorMoveEnd, (object[] args) => 
        {
            //redraw maps
            if (args[0] is Actor a)
            {
                if(Pathfinder.Distance(a.tile, this.tile) <= (int)this.data.GetStat(StatType.SightRange).GetValue())
                    UpdateLoSMap();

                //could prob. do some more distance pruning here
                if (this.canAct)
                    UpdateMovementMap();
            }
        });
        GlobalEvents.Subscribe(GlobalEvent.ActorAdded, (object[] args) =>
        {
            //can prune actors spawning outside of our sight range
            if (Pathfinder.Distance(this.tile, (args[0] as Actor).tile) <= (int)this.data.GetStat(StatType.SightRange).GetValue())
                UpdateLoSMap();

            UpdateMovementMap();
        });
        GlobalEvents.Subscribe(GlobalEvent.ActorRemoved, (object[] args) =>
        {
            if (Pathfinder.Distance(this.tile, (args[0] as Actor).tile) <= (int)this.data.GetStat(StatType.SightRange).GetValue())
                UpdateLoSMap();
        
            UpdateMovementMap();
        });

        Instantiate();
        StartCoroutine(CommandLoop());
    }

    public void AddCommand(Command c)
    {
        _commands.Enqueue(c);
    }
    IEnumerator CommandLoop()
    {
        while (this.canAct)
        {
            if (!this.isBusy)
            {
                if (_commands.Count > 0)
                    _commands.Dequeue().Execute();
            }

            yield return null;
        }
    }

    public virtual void OnNewTurn()
    {
        //reset vitals that needs to be reset
        this.data.GetVital(VitalType.Stamina).SetCurrent(data.GetVital(VitalType.Stamina).GetMax());
        //tick item turn effects
        this.data.TickItemTurnEffects();
        //update LoS
        UpdateLoSMap();
        UpdateActorStatus();
        this.StartCoroutine(CommandLoop());
    }
    void UpdateActorStatus()
    {
        this.canAct = this.data.GetVital(VitalType.Stamina).current > 0 && GameManager.turnIndex == this.teamID;
        GlobalEvents.Raise(GlobalEvent.ActorStatusChanged, this);
    }

    public void MoveTo(List<Tile> path, bool isSprinting)
    {
        if (path != null && path.Count > 0)
        {
            SetIsBusy(true);
            StartCoroutine(MoveBySpeed(path, isSprinting ? 1f : .25f, isSprinting));
        }
    }
    IEnumerator MoveBySpeed(List<Tile> path, float speed, bool isSprinting)
    {
        SetIsBusy(true);
        GlobalEvents.Raise(GlobalEvent.ActorMoveStart, this);

        Animator animator = this.GetComponentInChildren<Animator>();

        float targetSpeed = speed;
        float actualSpeed = 0f;
        float targetRandomIndex = Random.Range(0f, 1f);
        float actualRandomIndex = 0f;

        //enable root motion
        animator.applyRootMotion = true;
        animator.SetFloat("random", Random.Range(0, 1 + 1));

        int index = 0;

        while (index < path.Count)
        {
            //update target animation blend value
            actualSpeed = Mathf.Lerp(actualSpeed, targetSpeed, Time.deltaTime * _animationBlendSpeed);
            actualRandomIndex = Mathf.Lerp(actualRandomIndex, targetRandomIndex, Time.deltaTime * _animationBlendSpeed);

            animator.SetFloat("speed", actualSpeed);
            animator.SetFloat("random", actualRandomIndex);

            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(path[index].position - this.transform.position), _rotationSpeed * Time.deltaTime);

            if (Vector3.Distance(this.transform.position, path[index].position) < .25f)
            {
                targetRandomIndex = Random.Range(0f, 1f);
                index++;
            }

            yield return null;
        }

        animator.SetFloat("speed", 0f);
        animator.applyRootMotion = false;

        this.data.GetVital(VitalType.Stamina).Update(-path.Count);
        base.SetPosition(path.Last());

        //GlobalEvents.Raise(GlobalEvent.SetCameraTrackingTarget, null);
        GlobalEvents.Raise(GlobalEvent.ActorMoveEnd, this);
        SetIsBusy(false);
    }

    public bool CanInteract(Entity entity, bool raiseDialogues)
    {
        if(Pathfinder.Distance(base.tile, entity.tile) > entity.interactRange)
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
        _equipmentManager.Instantiate(this);
    }

    public void OnAttack(ActionContext context)
    {
        _equipmentManager.OnAttack();
    }
    public void OnAttacked(ActionContext context)
    {
        _equipmentManager.OnAttacked();
    }
    public void OnInteract()
    {
        _equipmentManager.OnInteract();
    }

    public override void Examine(Actor examineer)
    {
        if(Player.bestiary.ContainsKey(this.name))
            GlobalEvents.Raise(GlobalEvent.PopupRequested, this.transform.position + Vector3.up, Player.bestiary[this.name]);
    }

    public void SetItem(Item item, int index)
    {
        this.data.SetItem(item, index);
        item.OnPickUp(this);
    }
    public void SetItemIfOpen(Item item)
    {
        if(this.data.SetItemIfOpen(item))
            item.OnPickUp(this);
    }
    public void RemoveItem(int index)
    {
        Item i = this.data.RemoveItem(index);

        i?.OnDropped();

        if (i != null)
            GlobalEvents.Raise(GlobalEvent.PopupRequested, "Lost: " + i.NameToString() + "!");
    }

    void OnInventoryChanged(int index)
    {
        GlobalEvents.Raise(GlobalEvent.ActorInventoryChanged, this, index);
    }
    void OnEquipped(Equipable equipable)
    {
        _equipmentManager.OnEquipped(equipable);

        GlobalEvents.Raise(GlobalEvent.ActorEquipmentChanged, this, equipable);
    }
    void OnUnequipped(Equipable equipable)
    {
        _equipmentManager.OnUnequipped(equipable);

        GlobalEvents.Raise(GlobalEvent.ActorEquipmentChanged, this, equipable);
    }
    void OnSpellsChanged()
    {
        GlobalEvents.Raise(GlobalEvent.ActorSpellsChanged, this);
    }

    void OnAttributeChanged(AttributeType at)
    {
        GlobalEvents.Raise(GlobalEvent.ActorAttributeChanged, this, at);
    }
    void OnVitalChanged(VitalType vt, int change)
    {
        if(vt == VitalType.Health)
        {
            GlobalEvents.Raise(GlobalEvent.PopupRequested, this.transform.position + (Vector3.up * 2), (change == 0 ? "<color=yellow>" : (change < 0 ? "<color=red>" : "<color=green>")) + change + "</color>");

            if(data.GetVital(vt).current <= 0)
            {
                OnDeath();
                return;
            }
        }
        else if(vt == VitalType.Corruption)
        {
            GlobalEvents.Raise(GlobalEvent.PopupRequested, this.transform.position + (Vector3.up * 2), "<color=purple>Corruption:</color> " + (change == 0 ? "<color=yellow>" : (change < 0 ? "<color=red>" : "<color=green>")) + change + "</color>");

            if (data.GetVital(vt).current == data.GetVital(vt).GetMax())
            {
                OnDeath();
                return;
            }
        }
        else if(vt == VitalType.Stamina)
        {
            UpdateMovementMap();
            UpdateActorStatus();
        }

        GlobalEvents.Raise(GlobalEvent.ActorVitalChanged, this, vt);
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
        SetMap(MapType.Movement, Pathfinder.Dijkstra(this.GetMap(MapType.Movement), this.tile, (int)this.data.GetStat(StatType.SprintRange).GetValue(), TileStatus.Blocked, TileStatus.Occupied));
        GlobalEvents.Raise(GlobalEvent.ActorMapChanged, this, MapType.Movement);
    }
    void UpdateLoSMap()
    {
        //all actors can see their immediate surroundings, aswell as further depending on luminosity levels
        SetMap(MapType.LineOfSight,
            Pathfinder.Dijkstra(
            this.GetMap(MapType.LineOfSight),
            this.tile,
            (int)this.data.GetStat(StatType.SightRange).GetValue(),
            TileStatus.Blocked));

        //clear old targets
        for (int i = 0; i < this.visibleActors.Count; i++)
            if (this.teamID != this.visibleActors[i].teamID)
                this.visibleActors[i].UpdateSeenByIndex(-1);

        this.visibleActors.Clear();

        foreach (Tile t in GetMap(MapType.LineOfSight).Keys)
        {
            if (t.luminosity < this.data.GetStat(StatType.SightThreshold).GetValue() && Pathfinder.Distance(this.tile, t) > 3)
                continue;

            if(t.entity != null)
            {
                if (t.entity is Actor a)
                {
                    this.visibleActors.Add(a);

                    if (a.teamID != this.teamID)
                        a.UpdateSeenByIndex(1);
                }
            }
        }

        GlobalEvents.Raise(GlobalEvent.ActorMapChanged, this, MapType.LineOfSight);
    }

    public void UpdateSeenByIndex(int increment)
    {
        this.seenByIndex += increment;

        if (!(this is NPActor))
            return;

        _model.transform.GetChild(0).Find("renderers").gameObject.SetActive(this.seenByIndex > 0);

        GlobalEvents.Raise(GlobalEvent.ToggleActorUIVisibility, this, this.seenByIndex > 0);
    }

    public Dictionary<Tile, float> GetMap(MapType mt)
    {
        return _maps[(int)mt];
    }
    public void SetMap(MapType mt, Dictionary<Tile, float> map)
    {
        _maps[(int)mt] = map;
    }
}
