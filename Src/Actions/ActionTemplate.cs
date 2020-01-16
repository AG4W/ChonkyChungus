using UnityEngine;

using ag4w.Actions;

[CreateAssetMenu(menuName = "Templates/Action")]
public class ActionTemplate : ScriptableObject
{
    [SerializeField]string _header;
    [SerializeField]Sprite _icon;
    [SerializeField]TextAsset _lua;

    public Action Instantiate()
    {
        return new Action(_header, _icon, _lua.text);
    }
}
