using UnityEngine;

[System.Serializable]
public class ObjectEntry
{
    [SerializeField]string[] _args;
    [SerializeField]Object[] _objects;

    public string[] args { get { return _args; } }
    public Object[] objects { get { return _objects; } }
}