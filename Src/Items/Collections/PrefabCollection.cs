using UnityEngine;

[CreateAssetMenu(menuName = "Collections/Prefab Collection")]
public class PrefabCollection : ScriptableObject
{
    [SerializeField]GameObject[] _prefabs;

    public GameObject[] prefabs { get { return _prefabs; } }
}