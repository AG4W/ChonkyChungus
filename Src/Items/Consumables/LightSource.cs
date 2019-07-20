using UnityEngine;

using System.Collections.Generic;

public class LightSource : Item
{
    int _maxRange;

    Dictionary<Tile, float> _map = new Dictionary<Tile, float>();

    public int range { get; private set; }

    public int lifetime { get; private set; }
    public int remainingLifetime { get; private set; }

    public float inPercent { get { return remainingLifetime / (float)lifetime; } }

    public LightSource(string name, string flavor, DamageType damageType, GameObject prefab, ItemRarity rarity, int range, int lifetime) : base(name, flavor, damageType, prefab, rarity)
    {
        _maxRange = range;

        this.range = range;
        this.lifetime = lifetime;
        this.remainingLifetime = lifetime;

        GlobalEvents.Subscribe(GlobalEvent.ActorMoveStart, (object[] args) =>
        {
            if (base.holder is Actor a && args[0] is Actor b && a == b)
                OnMoveStart();
        });
        GlobalEvents.Subscribe(GlobalEvent.ActorMoveEnd, (object[] args) =>
        {
            if (base.holder is Actor a && args[0] is Actor b && a == b)
                OnMoveEnd();
        });
    }

    public override void OnEquip()
    {
        UpdateInfluenceMap();
        ApplyLuminosityInfluence();

        base.OnEquip();
    }
    public override void OnNewTurn()
    {
        base.OnNewTurn();

        if (!base.isEquipped || base.holder == null)
            return;

        //remove old first
        RemoveLuminosityInfluence();

        remainingLifetime--;
        range = (int)Mathf.Clamp(_maxRange * inPercent, _maxRange / 4, _maxRange);

        if (remainingLifetime == 0 && base.holder is Actor a)
            new RemoveItemCommand(a, this);
        else if(remainingLifetime > 0)
        {
            //reapply influence
            UpdateInfluenceMap();
            ApplyLuminosityInfluence();

            OnTicked?.Invoke(this);
        }
        //weird if statement,
        //but any lifetimes that are < 0 should be infinite
        //mainly for static map stuff
    }
    public override void OnUnequip()
    {
        RemoveLuminosityInfluence();
        base.OnUnequip();
    }

    void OnMoveStart()
    {
        RemoveLuminosityInfluence();
    }
    void OnMoveEnd()
    {
        UpdateInfluenceMap();
        ApplyLuminosityInfluence();
    }

    public float EvaluateVisualLuminosity()
    {
        //aay lmao
        //curve for lightsource visual effect
        return 0.3f + Mathf.Pow(inPercent, 0.3f) * 0.7f;
    }
    float EvaluateLuminosity(Tile target)
    {
        return 1 - (Pathfinder.Distance(base.holder.tile, target) / (float)this.range);
    }

    void ApplyLuminosityInfluence()
    {
        foreach (Tile tile in _map.Keys)
            tile.AddLuminosity(EvaluateLuminosity(tile));

        GlobalEvents.Raise(GlobalEvent.LightSourceInfluenceChanged, this);
    }
    void RemoveLuminosityInfluence()
    {
        foreach (Tile tile in _map.Keys)
            tile.SubtractLuminosity(EvaluateLuminosity(tile));

        GlobalEvents.Raise(GlobalEvent.LightSourceInfluenceChanged, this);
    }
    void UpdateInfluenceMap()
    {
        _map = Pathfinder.Dijkstra(_map, base.holder.tile, range, false);
    }

    public override GameObject Instantiate()
    {
        GameObject g = base.Instantiate();
        g.AddComponent<LightSourceEntity>().Initialize(this);

        return g;
    }

    public override string ToTooltip()
    {
        return base.ToTooltip() + "\n\n" +
            "Range: " + this.range + "\n" +
            "Duration: " + this.remainingLifetime;
    }

    public delegate void LightSourceEvent(LightSource ls);
    public event LightSourceEvent OnTicked;
}
