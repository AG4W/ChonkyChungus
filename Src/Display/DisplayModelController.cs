using UnityEngine;

using System;

public class DisplayModelController : MonoBehaviour
{
    [SerializeField]Transform _rightEquipPoint;
    [SerializeField]Transform _leftEquipPoint;

    [SerializeField]float _rotationSpeed = 5f;

    [SerializeField]bool _isControllable;

    SkinnedMeshRenderer[] _renderers;

    SkinnedMeshRenderer _hair;
    SkinnedMeshRenderer _underwear;

    ActorData _data;

    void Awake()
    {
        GatherRenderers();
    }
    void GatherRenderers()
    {
        _renderers = new SkinnedMeshRenderer[Enum.GetNames(typeof(EquipSlot)).Length];

        for (int i = 2; i < _renderers.Length; i++)
        {
            _renderers[i] = new GameObject(((EquipSlot)i).ToString(), typeof(SkinnedMeshRenderer)).GetComponent<SkinnedMeshRenderer>();
            _renderers[i].transform.SetParent(this.transform.GetChild(0));
        }
            
        _hair = this.transform.GetChild(0).transform.Find("hair").GetComponent<SkinnedMeshRenderer>();
        _underwear = this.transform.GetChild(0).transform.Find("underwear").GetComponent<SkinnedMeshRenderer>();

        _hair.enabled = false;
        _underwear.enabled = false;
    }

    public void SetActorData(ActorData data)
    {
        if(_data != null)
        {
            _data.OnEquipped -= OnEquipped;
            _data.OnUnequipped -= OnUnequipped;

            for (int i = 0; i < _rightEquipPoint.childCount; i++)
                Destroy(_rightEquipPoint.GetChild(i).gameObject);
            for (int i = 0; i < _leftEquipPoint.childCount; i++)
                Destroy(_leftEquipPoint.GetChild(i).gameObject);
            for (int i = 2; i < Enum.GetNames(typeof(EquipSlot)).Length; i++)
            {
                EquipSlot slot = (EquipSlot)i;

                _renderers[(int)slot].sharedMesh = null;

                if (slot == EquipSlot.Head)
                    _hair.enabled = true;
                if (slot == EquipSlot.Legs)
                    _underwear.enabled = true;
            }
        }

        _data = data;

        data.OnEquipped += OnEquipped;
        data.OnUnequipped += OnUnequipped;

        for (int i = 0; i < Enum.GetNames(typeof(EquipSlot)).Length; i++)
        {
            EquipSlot slot = (EquipSlot)i;
            Equipable e = data.GetEquipment(slot);

            if (e != null)
                OnEquipped(e);
            else
            {
                if(slot == EquipSlot.LeftHandItem || slot == EquipSlot.RightHandItem)
                {
                    for (int j = 0; j < (slot == EquipSlot.LeftHandItem ? _leftEquipPoint : _rightEquipPoint).childCount; j++)
                        Destroy((slot == EquipSlot.LeftHandItem ? _leftEquipPoint : _rightEquipPoint).GetChild(j).gameObject);
                }
                else
                {
                    _renderers[(int)slot].sharedMesh = null;

                    if (slot == EquipSlot.Head)
                        _hair.enabled = true;
                    if (slot == EquipSlot.Legs)
                        _underwear.enabled = true;
                }
            }
        }
    }

    void OnEquipped(Equipable equipable)
    {
        GameObject g = equipable.prefab;

        if (equipable is Holdable holdable)
        {
            //instantiate prefab
            g = Instantiate(g);

            Transform equipPoint = holdable is Weapon ? _rightEquipPoint : _leftEquipPoint;

            g.transform.position = equipPoint.position;
            g.transform.rotation = equipPoint.rotation;
            g.transform.SetParent(equipPoint);

            //_ik.SetIKTarget(AvatarIKGoal.LeftHand, g.transform.Find("left IK"));
            //_ik.SetIKStatus(AvatarIKGoal.LeftHand, holdable.useLeftHandIK);

            //_animator.SetFloat("animationSet", (int)holdable.animationSet);
        }
        else if (equipable is Armour armour)
        {
            if(equipable.prefab.GetComponentInChildren<SkinnedMeshRenderer>() != null)
            {
                SkinnedMeshRenderer smr = equipable.prefab.GetComponentInChildren<SkinnedMeshRenderer>();
                Transform[] bones = new Transform[smr.bones.Length];

                for (int i = 0; i < bones.Length; i++)
                    bones[i] = this.transform.GetChild(0).Find("Root").FindChildByName(smr.bones[i].name);

                _renderers[(int)armour.slot].bones = bones;
                _renderers[(int)armour.slot].materials = smr.sharedMaterials;
                _renderers[(int)armour.slot].sharedMesh = smr.sharedMesh;
            }

            if (armour.slot == EquipSlot.Head)
                _hair.enabled = false;
            if (armour.slot == EquipSlot.Legs)
                _underwear.enabled = false;
        }
    }
    void OnUnequipped(Equipable equipable)
    {
        if (equipable == null)
            return;

        if (equipable is Holdable holdable)
        {
            for (int i = 0; i < (holdable.slot == EquipSlot.LeftHandItem ? _leftEquipPoint : _rightEquipPoint).childCount; i++)
                Destroy((holdable.slot == EquipSlot.LeftHandItem ? _leftEquipPoint : _rightEquipPoint).GetChild(i).gameObject);

            //if (holdable is Weapon)
            //{
            //    _animator.SetBool("hasWeapon", false);
            //    _animator.SetLayerWeight(1, 0f);
            //}
            //else
            //    _animator.SetLayerWeight(2, 0f);

            //_ik.SetIKTarget(AvatarIKGoal.LeftHand, null);
            //_ik.SetIKStatus(AvatarIKGoal.LeftHand, false);
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

    void Update()
    {
        if (!_isControllable)
            return;

        if(Input.GetKey(KeyCode.Mouse0))
            this.transform.eulerAngles += new Vector3(0f, Input.GetAxisRaw("Mouse X") * _rotationSpeed * Time.fixedDeltaTime, 0f);
    }
}
