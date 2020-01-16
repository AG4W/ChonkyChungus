using UnityEngine;

public class PCGEntityData : MonoBehaviour
{
    //other stuff to come
    [Header("Offsets will be rounded to integers")]
    [SerializeField]Vector2Int _positiveSize = new Vector2Int(0, 0);
    [SerializeField]Vector2Int _negativeSize = new Vector2Int(0, 0);

    [Header("Tile Stuff")]
    [SerializeField]bool _blocksTile;
    [SerializeField]bool _blocksLineOfSight;

    public Vector2Int positiveSize { get { return _positiveSize; } }
    public Vector2Int negativeSize { get { return _negativeSize; } }

    public bool blocksTile { get { return _blocksTile; } }
    public bool blocksLineOfSight { get { return _blocksLineOfSight; } }

    public PCGSubEntity[] subEntities { get; private set; }

    void Awake()
    {
        this.subEntities = this.GetComponentsInChildren<PCGSubEntity>();
    }
}