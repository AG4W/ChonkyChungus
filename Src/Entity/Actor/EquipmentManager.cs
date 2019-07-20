using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    [SerializeField]Transform _rightEquipPoint;
    [SerializeField]Transform _leftEquipPoint;
    [SerializeField]Transform _shieldEquipPoint;

    [SerializeField]Light _playerAmbientLight;

    IKManager _ik;
    Animator _animator;

    void Start()
    {
        _ik = this.GetComponentInChildren<IKManager>();
        _animator = this.GetComponentInChildren<Animator>();

        _animator.SetLayerWeight(1, 0f);
        _animator.SetLayerWeight(2, 0f);
    }

    public void OnEquipmentChanged(EquipSlot slot, Item item)
    {
        GameObject g = null;

        if(item != null)
        {
            g = item.Instantiate();

            if(slot == EquipSlot.LeftHand || slot == EquipSlot.RightHand)
            {
                for (int i = 0; i < (slot == EquipSlot.LeftHand ? _leftEquipPoint : _rightEquipPoint).childCount; i++)
                    Destroy((slot == EquipSlot.LeftHand ? _leftEquipPoint : _rightEquipPoint).GetChild(i).gameObject);

                g.transform.position = slot == EquipSlot.LeftHand ? _leftEquipPoint.position : _rightEquipPoint.position;
                g.transform.rotation = slot == EquipSlot.LeftHand ? _leftEquipPoint.rotation : _rightEquipPoint.rotation;
                g.transform.SetParent(slot == EquipSlot.LeftHand ? _leftEquipPoint : _rightEquipPoint);

                _animator.SetFloat(slot == EquipSlot.LeftHand ? "leftAnimationSet" : "rightAnimationSet", (int)item.damageType.animationSet);
                _animator.SetLayerWeight(slot == EquipSlot.LeftHand ? 1 : 2, 1f);
            }
            else
            {
                _animator.SetLayerWeight(slot == EquipSlot.LeftHand ? 1 : 2, 0f);
            }
        }
        else
        {
            if(slot == EquipSlot.LeftHand || slot == EquipSlot.RightHand)
            {
                for (int i = 0; i < (slot == EquipSlot.LeftHand ? _leftEquipPoint : _rightEquipPoint).childCount; i++)
                    Destroy((slot == EquipSlot.LeftHand ? _leftEquipPoint : _rightEquipPoint).GetChild(i).gameObject);

                _animator.SetLayerWeight(slot == EquipSlot.LeftHand ? 1 : 2, 0f);
            }
        }
    }

    public void OnAttack()
    {
        _animator?.SetTrigger("attack");
    }
}
