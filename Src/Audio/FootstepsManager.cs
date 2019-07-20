using UnityEngine;

public class FootstepsManager : MonoBehaviour
{
    [SerializeField]AudioClip[] _footsteps;

    [SerializeField]GameObject _oneshotPrefab;

    public void PlayFootstep()
    {
        GameObject g = Instantiate(_oneshotPrefab, this.transform.position, Quaternion.identity, null);

        AudioSource source = g.GetComponent<AudioSource>();
        source.pitch = Random.Range(.75f, 1.25f);
        source.volume = Random.Range(.05f, .1f);
        source.clip = _footsteps.Random();
        source.Play();

        Destroy(g, 1f);
    }
}
