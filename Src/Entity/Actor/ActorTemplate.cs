using UnityEngine;

[CreateAssetMenu(menuName = "Templates/ActorTemplate")]
public class ActorTemplate : ScriptableObject
{
    [SerializeField]string _name;
    [SerializeField]string[] _randomNameList = new string[]
    {
        "Axel",
        "Erik",
        "Albin",
        "Johan",
        "Veronica",
        "Torbjörn",
        "Carolina",
        "William",
        "Alexander",
        "Viola",
        "Emma",
        "Lisa",
    };

    [SerializeField]RaceType _race;
    [SerializeField]RaceAnimationSet _raceAnimationSet;

    [Range(0, 99)][SerializeField]int _strength = 1;
    [Range(0, 99)][SerializeField]int _vitality = 1;
    [Range(0, 99)][SerializeField]int _movement = 1;
    [Range(0, 99)][SerializeField]int _willpower = 1;

    [Range(0, 99)][SerializeField]int _sightRange;
    [Range(0f, 1f)][SerializeField]float _sightThreshold;

    [SerializeField]GameObject _prefab;

    [SerializeField]AudioClip[] _damageSFX;
    [SerializeField]AudioClip[] _deathSFX;

    public RaceType race { get { return _race; } }
    public RaceAnimationSet raceAnimationSet { get { return _raceAnimationSet; } }

    public int strength { get { return _strength; } }
    public int vitality { get { return _vitality; } }
    public int movement { get { return _movement; } }
    public int willpower { get { return _willpower; } }

    public int sightRange { get { return _sightRange; } }
    public float sightThreshold { get { return _sightThreshold; } }

    public GameObject prefab { get { return _prefab; } }

    public AudioClip[] damageSFX { get { return _damageSFX; } }
    public AudioClip[] deathSFX { get { return _deathSFX; } }

    public ActorData Instantiate()
    {
        return new ActorData(_name == null || _name.Length == 0 ? _randomNameList.Random() : _name, this);
    }
}
public enum RaceAnimationSet
{
    Humanoid,
    UndeadShambling,
    UndeadCrawling,
}
