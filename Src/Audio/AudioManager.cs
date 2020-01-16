using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

    [SerializeField]AudioClip[] _loadingCompleteStingers;

    AudioClip[][] _uiClips;

    [SerializeField]GameObject _oneshotPrefab;

    [SerializeField]AudioSource _stingers;

    void Awake()
    {
        instance = this;

        GlobalEvents.Subscribe(GlobalEvent.PCGComplete, OnGameInitialized);
    }

    void OnGameInitialized(object[] args)
    {
        _stingers.pitch = Random.Range(.85f, 1.15f);
        _stingers.clip = _loadingCompleteStingers.Random();
        _stingers.Play();
    }

    void Play(AudioClip clip, Vector3 position, float minVolume, float maxVolume, float minPitch, float maxPitch)
    {
        GameObject g = Instantiate(_oneshotPrefab, position, Quaternion.identity, null);

        AudioSource a = g.GetComponent<AudioSource>();

        a.clip = clip;
        a.volume = Random.Range(minVolume, maxVolume);
        a.pitch = Random.Range(minPitch, maxPitch);
        a.Play();

        Destroy(g, clip.length + .5f);
    }
    public static void PlayOneshot(AudioClip clip, Vector3 position, float minVolume, float maxVolume, float minPitch, float maxPitch)
    {
        instance.Play(clip, position, minVolume, maxVolume, minVolume, maxPitch);
    }
}

