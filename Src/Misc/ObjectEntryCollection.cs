using UnityEngine;

[CreateAssetMenu(menuName = "Collections/Object Entry Collection")]
public class ObjectEntryCollection : ScriptableObject
{
    [SerializeField]ObjectEntry[] _objects;

    public ObjectEntry[] objects { get { return _objects; } }
}
