using UnityEngine;

using System.IO;

public static class ItemGenerator
{
    static DamageType[] _weaponDamageTypes;
    static DamageType _defaultDamageType;

    static PrefabCollection[] _prefabs;

    public static void Initialize()
    {
        LoadDamageTypes();
        LoadPrefabCollections();
    }

    static void LoadDamageTypes()
    {
        string defaultTypePath = Application.dataPath + "/Resources/Configs/defaultDamageType.json";
        string weaponDamageTypesPath = Application.dataPath + "/Resources/Configs/weaponDamageTypes.json";

        _defaultDamageType = JsonUtility.FromJson<DamageType>(File.ReadAllText(defaultTypePath));
        _weaponDamageTypes = JsonHelper.GetJsonArray<DamageType>(File.ReadAllText(weaponDamageTypesPath));
    }
    static void LoadPrefabCollections()
    {
        _prefabs = new PrefabCollection[System.Enum.GetNames(typeof(ItemType)).Length];

        for (int i = 0; i < _prefabs.Length; i++)
            _prefabs[i] = Resources.Load<PrefabCollection>("PrefabCollections/" + ((ItemType)i).ToString());
    }

    public static Item Get(ItemType itemType, ItemRarity rarity, DamageType type = null)
    {
        type = type ?? (itemType == ItemType.Weapon ? _weaponDamageTypes.Random() : _defaultDamageType);

        GameObject prefab = _prefabs[(int)itemType].GetRandom();
        string name = prefab.name;

        switch (itemType)
        {
            case ItemType.Weapon:
                return new Item(name, "n/a", type, prefab, rarity);
            case ItemType.Armour:
                return new Item(name, "n/a", type, prefab, rarity);
            case ItemType.Consumable:
                return new Item(name, "n/a", type, prefab, rarity);
            case ItemType.LightSource:
                return new LightSource(name, "n/a", type, prefab, rarity, 15, 14);
            default:
                return null;
        }
    }
}