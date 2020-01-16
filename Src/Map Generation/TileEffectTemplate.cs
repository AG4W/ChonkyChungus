using UnityEngine;

[CreateAssetMenu(menuName = "Templates/Tile Effect")]
public class TileEffectTemplate : ScriptableObject
{
    [SerializeField]int _duration;
    [SerializeField]GameObject _vfx;

    [SerializeField]TextAsset _lua;

    public TileEffect Instantiate()
    {
        return new TileEffect(this.name, _duration, _vfx, _lua.text);
    }
}
