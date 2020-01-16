using UnityEngine;

[CreateAssetMenu(menuName = "Collections/Weapon Collection")]
public class WeaponCollection : ScriptableObject
{
    [SerializeField]WeaponPrefabData[] _weapons;

    public WeaponPrefabData[] weapons { get { return _weapons; } }
}
