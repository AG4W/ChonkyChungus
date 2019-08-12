using UnityEngine;

using System.IO;
using System.Collections.Generic;
using System.Linq;

public static class ItemGenerator
{
    static ItemAction[] _actions;

    static DamageType[] _weaponDamageTypes;
    static DamageType _defaultDamageType;

    static PrefabCollection[] _prefabs;

    public static void Initialize()
    {
        LoadDamageTypes();
        LoadPrefabCollections();
        LoadItemActions();
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
    static void LoadItemActions()
    {
        List<ItemAction> actions = new List<ItemAction>();

        foreach (ItemActionTemplate iat in Resources.LoadAll<ItemActionTemplate>("Templates/Item Actions/"))
            actions.Add(iat.Instantiate());
    
        _actions = actions.ToArray();
    }

    public static Item Get(ItemType itemType, ItemRarity rarity, DamageType type = null)
    {
        Item i;

        type = type ?? (itemType == ItemType.Weapon ? _weaponDamageTypes.Random() : _defaultDamageType);

        GameObject prefab = _prefabs[(int)itemType].GetRandom();
        string name = prefab.name;

        switch (itemType)
        {
            case ItemType.Weapon:
                i = new Item(name, "n/a", type, prefab, rarity);
                break;
            case ItemType.Armour:
                i = new Item(name, "n/a", type, prefab, rarity);
                break;
            case ItemType.Consumable:
                i = new Item(name, "n/a", type, prefab, rarity);
                break;
            case ItemType.LightSource:
                i = new LightSource(name, "n/a", type, prefab, rarity, 15, 14);
                break;
            default:
                i = null;
                break;
        }

        //setup default actions
        i.actions.Add(_actions.First(a => a.header == "Attack"));
        i.actions.Add(_actions.First(a => a.header == "Throw"));
        i.actions.Add(_actions.First(a => a.header.Contains("Magic")));

        i.newTurnEffects.Add(_actions.First(a => a.header.Contains("Vampiric")));

        return i;
    }
}