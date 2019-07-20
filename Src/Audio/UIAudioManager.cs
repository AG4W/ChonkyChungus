using UnityEngine;
using UnityEngine.Audio;

public class UIAudioManager : MonoBehaviour
{
    public static UIAudioManager instance { get; private set; }

    [SerializeField]AudioClip[] _inventoryToggles;
    [SerializeField]AudioClip[] _characterToggles;
    [SerializeField]AudioClip[] _skillToggles;
    [SerializeField]AudioClip[] _clicks;
    [SerializeField]AudioClip[] _enterExits;
    [SerializeField]AudioClip[] _itemAdded;
    [SerializeField]AudioClip[] _itemRemoved;
    [SerializeField]AudioClip[] _itemConsumed;

    AudioClip[][] _clips;

    [SerializeField]AudioSource _ui;

    void Awake()
    {
        instance = this;

        _clips = new AudioClip[][] 
        {
            _inventoryToggles,
            _characterToggles,
            _skillToggles,
            _clicks,
            _enterExits,
            _itemAdded,
            _itemRemoved,
            _itemConsumed
        };
    }

    public static void Play(UISoundType type)
    {
        instance.PlayInternal(type);
    }
    void PlayInternal(UISoundType type)
    {
        _ui.pitch = Random.Range(.75f, 1.25f);
        _ui.clip = _clips[(int)type].Random();
        _ui.Play();
    }
}
public enum UISoundType
{
    ToggleInventory,
    ToggleCharacter,
    ToggleSkills,
    Click,
    EnterExit,

    ItemAdded,
    ItemRemoved,
    ItemConsumed,
}