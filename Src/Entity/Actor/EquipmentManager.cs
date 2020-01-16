using UnityEngine;

using System;

public class EquipmentManager : MonoBehaviour
{
    [SerializeField]Transform _rightEquipPoint;
    [SerializeField]Transform _leftEquipPoint;
    [SerializeField]Transform _shieldEquipPoint;

    [SerializeField]GameObject _model;

    SkinnedMeshRenderer[] _renderers;

    SkinnedMeshRenderer _hair;
    SkinnedMeshRenderer _underwear;

    IKManager _ik;
    Animator _animator;
    AnimatorEventCallbackManager _callbackManager;

    Action _castCallback;

    void Awake()
    {
        _ik = this.GetComponentInChildren<IKManager>();
        _animator = this.GetComponentInChildren<Animator>();

        _callbackManager = this.GetComponentInChildren<AnimatorEventCallbackManager>();
        _callbackManager.OnAnimationEventCalled += OnAnimationEventCalled;

        _animator.SetLayerWeight(2, 0f);

        GatherRenderers();
    }

    void GatherRenderers()
    {
        _renderers = new SkinnedMeshRenderer[Enum.GetNames(typeof(EquipSlot)).Length];
    
        for (int i = 2; i < _renderers.Length; i++)
        {
            _renderers[i] = new GameObject(((EquipSlot)i).ToString(), typeof(SkinnedMeshRenderer)).GetComponent<SkinnedMeshRenderer>();
            _renderers[i].transform.SetParent(_model.transform);
        }

        _hair = _model.transform.Find("hair").GetComponent<SkinnedMeshRenderer>();
        _underwear = _model.transform.Find("underwear").GetComponent<SkinnedMeshRenderer>();

        _hair.enabled = false;
        _underwear.enabled = false;
    }

    public void OnEquipped(Equipable equipable)
    {
        if (equipable == null)
            return;

        GameObject g = equipable.prefab;

        if (equipable is Holdable holdable)
        {
            //instantiate prefab
            g = Instantiate(g);

            Transform equipPoint;

            if (holdable is Weapon)
                equipPoint = _rightEquipPoint;
            else
                equipPoint = _leftEquipPoint;

            g.transform.position = equipPoint.position;
            g.transform.rotation = equipPoint.rotation;
            g.transform.SetParent(equipPoint);

            if (holdable is Weapon weapon)
            {
                _animator.SetBool("hasWeapon", true);
                _animator.SetLayerWeight(1, 1f);
            }
            else
                _animator.SetLayerWeight(2, 1f);

            _ik.SetIKTarget(AvatarIKGoal.LeftHand, g.transform.Find("left IK"));
            _ik.SetIKStatus(AvatarIKGoal.LeftHand, holdable.useLeftHandIK);

            _animator.SetFloat("animationSet", (int)holdable.animationSet);
        }
        else if (equipable is Armour armour)
        {
            SkinnedMeshRenderer smr = equipable.prefab.GetComponentInChildren<SkinnedMeshRenderer>();
            Transform[] bones = new Transform[smr.bones.Length];

            for (int i = 0; i < bones.Length; i++)
                bones[i] = _model.transform.FindChildByName(smr.bones[i].name);

            _renderers[(int)armour.slot].bones = bones;
            _renderers[(int)armour.slot].materials = smr.sharedMaterials;
            _renderers[(int)armour.slot].sharedMesh = smr.sharedMesh;
            
            if (armour.slot == EquipSlot.Head)
                _hair.enabled = false;
            if (armour.slot == EquipSlot.Legs)
                _underwear.enabled = false;
        }
    }
    public void OnUnequipped(Equipable equipable)
    {
        if (equipable == null)
            return;

        if(equipable is Holdable holdable)
        {
            for (int i = 0; i < (holdable.slot == EquipSlot.LeftHandItem ? _leftEquipPoint : _rightEquipPoint).childCount; i++)
                Destroy((holdable.slot == EquipSlot.LeftHandItem ? _leftEquipPoint : _rightEquipPoint).GetChild(i).gameObject);

            if (holdable is Weapon)
            {
                _animator.SetBool("hasWeapon", false);
                _animator.SetLayerWeight(1, 0f);
            }
            else
                _animator.SetLayerWeight(2, 0f);

            _ik.SetIKTarget(AvatarIKGoal.LeftHand, null);
            _ik.SetIKStatus(AvatarIKGoal.LeftHand, false);
        }
        //if armour
        else
        {
            _renderers[(int)equipable.slot].sharedMesh = null;

            if (equipable.slot == EquipSlot.Head)
                _hair.enabled = true;
            if (equipable.slot == EquipSlot.Legs)
                _underwear.enabled = true;
        }
    }

    public void OnAttack()
    {
        _animator.SetFloat("random", UnityEngine.Random.Range(0, 2));
        _animator.SetTrigger("attack");
    }
    public void OnAttacked()
    {
        _animator.SetFloat("random", UnityEngine.Random.Range(0, 2));
        _animator.SetTrigger("attacked");
    }
    public void OnInteract()
    {
        _animator.SetTrigger("interact");
    }
    public void OnCast()
    {
        //start animation
        _animator.SetTrigger("cast");
    }

    public void SetCastCallback(Action action)
    {
        _castCallback = action;
    }
    void OnAnimationEventCalled(AnimationEvent ae)
    {
        switch (ae.stringParameter)
        {
            case "cast":
                _castCallback?.Invoke();
                break;
            default:
                break;
        }
    }

    public void Instantiate(Actor actor)
    {
        //some more data like changing gender/hairstyle etc to come here

        for (int i = 0; i < Enum.GetNames(typeof(EquipSlot)).Length; i++)
        {
            Equipable equipable = actor.data.GetEquipment((EquipSlot)i);

            //spawn objects for equipped items
            if(equipable != null)
                OnEquipped(equipable);
            //clear stuff that doesnt have anything equipped
            else
            {
                if ((EquipSlot)i == EquipSlot.LeftHandItem || (EquipSlot)i == EquipSlot.RightHandItem)
                {
                    for (int j = 0; j < ((EquipSlot)i == EquipSlot.LeftHandItem ? _leftEquipPoint : _rightEquipPoint).childCount; j++)
                        Destroy(((EquipSlot)i == EquipSlot.LeftHandItem ? _leftEquipPoint : _rightEquipPoint).GetChild(i).gameObject);
                }
                else
                {
                    _renderers[i].sharedMesh = null;

                    if ((EquipSlot)i == EquipSlot.Head)
                        _hair.enabled = true;
                    if ((EquipSlot)i == EquipSlot.Legs)
                        _underwear.enabled = true;
                }
            }
        }

        _animator.SetFloat("raceAnimationSet", (int)actor.data.raceAnimationSet);
    }
}
