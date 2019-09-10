using UnityEngine;

public class PCGEntityData : MonoBehaviour
{
    //other stuff to come
    [Header("Currently not used.")]
    [SerializeField]int _sizeX;
    [SerializeField]int _sizeY;

    [Header("Tile Stuff")]
    [SerializeField]bool _blocksTile;
    [SerializeField]bool _blocksLineOfSight;

    public int sizeX { get { return _sizeX; } }
    public int sizeY { get { return _sizeY; } }

    public bool blocksTile { get { return _blocksTile; } }
    public bool blocksLineOfSight { get { return _blocksLineOfSight; } }
}
