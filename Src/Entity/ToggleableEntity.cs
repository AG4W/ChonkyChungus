using UnityEngine;

public class ToggleableEntity : Entity
{
    [SerializeField]GameObject[] _objects;

    [SerializeField]AudioClip[] _onToggleSFX;

    public override void Interact(Actor interactee)
    {
        base.Interact(interactee);

        for (int i = 0; i < _objects.Length; i++)
            _objects[i].SetActive(!_objects[i].activeSelf);

        AudioManager.PlayOneshot(_onToggleSFX.Random(), this.transform.position, .2f, .3f, .75f, 1.25f);
    }
}
