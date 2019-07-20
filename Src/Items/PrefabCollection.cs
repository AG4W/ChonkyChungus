using UnityEngine;

using System.Linq;

[CreateAssetMenu(menuName = "Prefab Collection")]
public class PrefabCollection : ScriptableObject
{
    [SerializeField]PrefabCollectionEntry[] _entries;

    public GameObject[] Get(string key)
    {
        return _entries.FirstOrDefault(e => e.key.Equals(key, System.StringComparison.CurrentCultureIgnoreCase)).prefabs;
    }
    public GameObject GetRandom()
    {
        return _entries.Random().prefabs.Random();
    }
}
[System.Serializable]
public class PrefabCollectionEntry
{
    [SerializeField]string _key;
    [SerializeField]GameObject[] _prefabs;

    public string key { get { return _key; } }
    public GameObject[] prefabs { get { return _prefabs; } }

    public PrefabCollectionEntry(string key, GameObject[] prefabs)
    {
        _key = key;
        _prefabs = prefabs;
    }
}