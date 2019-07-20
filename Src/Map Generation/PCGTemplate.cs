using UnityEngine;

[CreateAssetMenu(menuName = "Templates/PCGTemplate")]
public class PCGTemplate : ScriptableObject
{
    [SerializeField]RoomType _type;

    [Header("Rubble and Decals")]
    [SerializeField]GameObject[] _rubble;
    [SerializeField]GameObject[] _gravel;
    [SerializeField]GameObject[] _grunges;

    [Header("Furniture")]
    [SerializeField]GameObject[] _tables;
    [SerializeField]GameObject[] _chairs;
    [SerializeField]GameObject[] _benches;
    [SerializeField]GameObject[] _shelves;
    [SerializeField]GameObject[] _containers;

    [SerializeField]GameObject[] _lights;

    [Header("Pillars")]
    [SerializeField]GameObject[] _pillars;
    [Header("Misc")]
    [SerializeField]GameObject[] _misc;
    [Header("Type Specific")]
    [SerializeField]GameObject[] _specific;

    [Header("Clutter Scarcity")]
    [Range(0f, .2f)][SerializeField]float _rubbleDensity;
    [Range(0f, .2f)][SerializeField]float _rubbleDensityVariation;

    [Range(0f, .2f)][SerializeField]float _grungeDensity;
    [Range(0f, .2f)][SerializeField]float _grungeDensityVariation;

    [Range(0f, .2f)][SerializeField]float _gravelDensity;
    [Range(0f, .2f)][SerializeField]float _gravelDensityVariation;

    [Range(0f, .2f)][SerializeField]float _miscDensity;
    [Range(0f, .2f)][SerializeField]float _miscDensityVariation;

    [Range(0f, .2f)][SerializeField]float _lightDensity;
    [Range(0f, .2f)][SerializeField]float _lightDensityVariation;

    [Range(0f, .2f)][SerializeField]float _lootDensity;
    [Range(0f, .2f)][SerializeField]float _lootDensityVariation;

    [SerializeField]ItemType[] _allowedLootTypes;

    #region Properties
    public RoomType type { get { return _type; } }

    public GameObject[] rubble { get { return _rubble; } }
    public GameObject[] gravel { get { return _gravel; } }
    public GameObject[] grunges { get { return _grunges; } }

    public GameObject[] benches { get { return _benches; } }
    public GameObject[] tables { get { return _tables; } }
    public GameObject[] chairs { get { return _chairs; } }

    public GameObject[] pillars { get { return _pillars; } }
    public GameObject[] shelves { get { return _shelves; } }

    public GameObject[] containers { get { return _containers; } }

    public GameObject[] lights { get { return _lights; } }

    public GameObject[] misc { get { return _misc; } }

    public GameObject[] specific { get { return _specific; } }

    public float rubbleDensity { get { return _rubbleDensity; } }
    public float rubbleDensityVariation { get { return _rubbleDensityVariation; } }

    public float grungeDensity { get { return _grungeDensity; } }
    public float grungeDensityVariation { get { return _grungeDensityVariation; } }

    public float gravelDensity { get { return _gravelDensity; } }
    public float gravelDensityVariation { get { return _gravelDensityVariation; } }

    public float miscDensity { get { return _miscDensity; } }
    public float miscDensityVariation { get { return _miscDensityVariation; } }

    public float lightDensity { get { return _lightDensity; } }
    public float lightDensityVariation { get { return _lightDensityVariation; } }

    public float lootDensity { get { return _lootDensity; } }
    public float lootDensityVariation { get { return _lootDensityVariation; } }

    public ItemType[] lootWhitelist { get { return _allowedLootTypes; } }
    #endregion
}
