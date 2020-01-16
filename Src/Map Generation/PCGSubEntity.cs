using UnityEngine;

public class PCGSubEntity : MonoBehaviour
{
    [SerializeField]PCGSubEntityType _type;

    [SerializeField]GameObject[] _prefabs;

    [SerializeField]Vector3 _spawnOffset;
    [SerializeField]Vector3Int _rotationOffset = new Vector3Int(0, 0, 0);

    [SerializeField]float _scaleVariance;

    [SerializeField]bool _ignoreSpawnProbability = false;

    public PCGSubEntityType type { get { return _type; } }

    public GameObject[] prefabs { get { return _prefabs; } }

    public Vector3 spawnOffset { get { return _spawnOffset; } }
    public Vector3Int rotationOffset { get { return _rotationOffset; } }

    public float scaleVariance { get { return _scaleVariance; } }

    public bool ignoreSpawnProbability { get { return _ignoreSpawnProbability; } }
}
public enum PCGSubEntityType
{
    Foliage,
    Treasure,
}