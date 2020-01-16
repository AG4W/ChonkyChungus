using UnityEngine;

public class FootstepsManager : MonoBehaviour
{
    [SerializeField]AudioClip[] _footsteps;

    [SerializeField]GameObject _oneshotPrefab;

    AnimatorEventCallbackManager _callbackManager;

    void Awake()
    {
        _callbackManager = this.GetComponentInChildren<AnimatorEventCallbackManager>();
        _callbackManager.OnAnimationEventCalled += OnAnimationEventCalled;
    }

    void OnAnimationEventCalled(AnimationEvent ae)
    {
        if (ae.stringParameter == "footstep" && ae.animatorClipInfo.weight > .5f)
            PlayFootstep();
    }
    void PlayFootstep()
    {
        GameObject g = Instantiate(_oneshotPrefab, this.transform.position, Quaternion.identity, null);

        AudioSource source = g.GetComponent<AudioSource>();
        source.pitch = Random.Range(.85f, 1.15f);
        source.clip = _footsteps.Random();
        source.volume = 1f;
        source.Play();

        Destroy(g, 1f);
    }
}
