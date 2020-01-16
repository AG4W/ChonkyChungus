using UnityEngine;

[CreateAssetMenu(menuName = "Templates/PCGTemplate")]
public class PCGTemplate : ScriptableObject
{
    [SerializeField]RegionProfile _type;

    [Header("Walls")]
    [SerializeField]GameObject[] _walls;
    [SerializeField]GameObject[] _floors;
    [SerializeField]GameObject[] _doors;

    [Header("Materials")]
    [SerializeField]Material[] _floorMaterials;
    [SerializeField]Material[] _wallMaterials;

    [Range(-1f, 1f)][SerializeField]float _averageDirtiness = 0f;
    [Range(-1f, 1f)][SerializeField]float _dirtinessVariance = 0f;
    [SerializeField]float _wetNoiseScale = 2f;

    [Header("Rubble and Decals")]
    [SerializeField]GameObject[] _rubble;
    [SerializeField]GameObject[] _grunges;
    [SerializeField]GameObject[] _puddles;
    [SerializeField]GameObject[] _dirt;

    [Header("Furniture")]
    [SerializeField]GameObject[] _tables;
    [SerializeField]GameObject[] _chairs;
    [SerializeField]GameObject[] _benches;
    [SerializeField]GameObject[] _shelves;
    [SerializeField]GameObject[] _containers;

    [SerializeField]GameObject[] _lights;

    [Header("Pillars")]
    [SerializeField]GameObject[] _pillars;
    [SerializeField]GameObject[] _statues;
    [Header("Misc")]
    [SerializeField]GameObject[] _misc;
    [Header("Type Specific")]
    [SerializeField]GameObject[] _specific;

    [Header("Probabilities")]
    [Range(0f, 1f)][SerializeField]float _pillarSpawnProbability = .75f;
    [Range(0f, 1f)][SerializeField]float _statueSpawnProbability = .5f;
    [Range(0f, 1f)][SerializeField]float _lightSpawnProbability = .01f;
    [Range(0f, 1f)][SerializeField]float _lootSpawnProbability = .05f;

    [Range(0f, 1f)][SerializeField]float _puddleDensity = .25f;
    [Range(0f, 1f)][SerializeField]float _dirtDensity = .5f;

    [Header("SubEntity Densities")]
    [Range(0f, 1f)][SerializeField]float _foliageDensity = .5f;
    [Range(0f, 1f)][SerializeField]float _treasureDensity = 1f;

    [SerializeField]ItemType[] _allowedLootTypes;

    #region Properties
    public RegionProfile profile { get { return _type; } }

    public GameObject[] floors { get { return _floors; } }
    public GameObject[] walls { get { return _walls; } }
    public GameObject[] doors { get { return _doors; } }

    public Material[] floorMaterials { get { return _floorMaterials; } }
    public Material[] wallMaterials { get { return _wallMaterials; } }

    public float averageDirtiness { get { return _averageDirtiness; } }
    public float dirtinessVariance { get { return _dirtinessVariance; } }
    public float wetNoiseScale { get { return _wetNoiseScale; } }

    public GameObject[] rubble { get { return _rubble; } }
    public GameObject[] grunges { get { return _grunges; } }
    public GameObject[] puddles { get { return _puddles; } }
    public GameObject[] dirt { get { return _dirt; } }

    public GameObject[] benches { get { return _benches; } }
    public GameObject[] tables { get { return _tables; } }
    public GameObject[] chairs { get { return _chairs; } }

    public GameObject[] pillars { get { return _pillars; } }
    public GameObject[] statues { get { return _statues; } }
    public GameObject[] shelves { get { return _shelves; } }

    public GameObject[] containers { get { return _containers; } }

    public GameObject[] lights { get { return _lights; } }

    public GameObject[] misc { get { return _misc; } }

    public GameObject[] specific { get { return _specific; } }

    public float pillarSpawnProbability { get { return _pillarSpawnProbability; } }
    public float statueSpawnProbability { get { return _statueSpawnProbability; } }
    public float lootSpawnProbability { get { return _lootSpawnProbability; } }
    public float lightSpawnProbability { get { return _lightSpawnProbability; } }

    public float puddleDensity { get { return _puddleDensity; } }
    public float dirtDensity { get { return _dirtDensity; } }

    public float[] subEntitiyDensities
    {
        get
        {
            return new float[] { _foliageDensity, _treasureDensity };
        }
    }
    public ItemType[] lootWhitelist { get { return _allowedLootTypes; } }
    #endregion
}
