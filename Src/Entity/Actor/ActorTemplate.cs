using UnityEngine;

[CreateAssetMenu(menuName = "Templates/ActorTemplate")]
public class ActorTemplate : ScriptableObject
{
    [SerializeField]RaceType _race;

    [Range(0, 99)][SerializeField]int _strength = 1;
    [Range(0, 99)][SerializeField]int _vitality = 1;
    [Range(0, 99)][SerializeField]int _quickness = 1;
    [Range(0, 99)][SerializeField]int _accuracy = 1;
    [Range(0, 99)][SerializeField]int _willpower = 1;

    [SerializeField]GameObject _prefab;

    [SerializeField]AudioClip[] _damageSFX;
    [SerializeField]AudioClip[] _deathSFX;

    public RaceType race { get { return _race; } }

    public int strength { get { return _strength; } }
    public int vitality { get { return _vitality; } }
    public int quickness { get { return _quickness; } }
    public int accuracy { get { return _accuracy; } }
    public int willpower { get { return _willpower; } }

    public GameObject prefab { get { return _prefab; } }

    public AudioClip[] damageSFX { get { return _damageSFX; } }
    public AudioClip[] deathSFX { get { return _deathSFX; } }

    public ActorData Instantiate()
    {
        return new ActorData(this.name, this);
    }

    public int GetModifier(AttributeType attribute)
    {
        switch (attribute)
        {
            case AttributeType.Strength:
                switch (race)
                {
                    case RaceType.Human:
                        return 3;
                    case RaceType.Undead:
                        return 4;
                    default:
                        return -1;
                }
            case AttributeType.Vitality:
                switch (race)
                {
                    case RaceType.Human:
                        return 4;
                    case RaceType.Undead:
                        return 1;
                    default:
                        return -1;
                }
            case AttributeType.Quickness:
                switch (race)
                {
                    case RaceType.Human:
                        return 4;
                    case RaceType.Undead:
                        return 1;
                    default:
                        return -1;
                }
            case AttributeType.Accuracy:
                switch (race)
                {
                    case RaceType.Human:
                        return 3;
                    case RaceType.Undead:
                        return 1;
                    default:
                        return -1;
                }
            case AttributeType.Willpower:
                switch (race)
                {
                    case RaceType.Human:
                        return 2;
                    case RaceType.Undead:
                        return 99;
                    default:
                        return -1;
                }
            default:
                return -1;
        }
    }
}
