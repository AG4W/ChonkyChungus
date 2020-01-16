using UnityEngine;

[CreateAssetMenu(menuName = "Collections/Loot Table")]
public class LootTable : ScriptableObject
{
    [SerializeField]GameObject[] _loot;

    public GameObject[] loot { get { return _loot; } }
}
