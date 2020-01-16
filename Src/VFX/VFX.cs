using UnityEngine;

using System.Collections.Generic;

public static class VFX
{
    static Dictionary<string, GameObject> _prefabs = new Dictionary<string, GameObject>();

    public static void Initialize()
    {
        GameObject[] prefabs = Resources.LoadAll<GameObject>("VFX/");

        for (int i = 0; i < prefabs.Length; i++)
            _prefabs[prefabs[i].name] = prefabs[i];
    }

    public static GameObject Spawn(string name)
    {
        return Object.Instantiate(_prefabs[name]);
    }
}
