using UnityEngine;

using MoonSharp.Interpreter;

[MoonSharpUserData]
public class TileEffect
{
    Script _lua;
    GameObject _vfxInstance;

    public string name { get; private set; }
    public int duration { get; private set; }

    public GameObject vfx { get; private set; }

    public Tile tile { get; private set; }

    public TileEffect(string name, int duration, GameObject vfx, string lua)
    {
        this.name = name;
        this.duration = duration;

        this.vfx = vfx;

        _lua = new Script();
        _lua.Options.DebugPrint = s => { Debug.Log(s); };
        _lua.Globals["GlobalEvents"] = typeof(GlobalEvents);
        _lua.DoString(lua);
    }
    public void SetTile(Tile tile)
    {
        this.tile = tile;
    }

    public void Tick()
    {
        _lua.Call(_lua.Globals["tick"], this);

        if (duration < 0)
            return;

        duration--;

        if (duration == 0)
            Complete();
    }
    public void Complete()
    {
        Deinstantiate();
        OnComplete?.Invoke(this);
    }

    public void Instantiate(Vector3 position, Quaternion rotation, Transform root)
    {
        _vfxInstance = Object.Instantiate(this.vfx, position, rotation, root);
    }
    void Deinstantiate()
    {
        Object.Destroy(_vfxInstance);

        _vfxInstance = null;
    }

    public delegate void TileEffectEvent(TileEffect te);
    public TileEffectEvent OnComplete;
}
