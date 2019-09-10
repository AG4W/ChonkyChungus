using UnityEngine;

using System.Collections.Generic;
using System.Linq;
using System.IO;

using ag4w.Actions;

public static class ItemGenerator
{
    static Action[] _itemActions;
    static Action[] _spells;

    static DamageType[] _weaponDamageTypes;
    static DamageType _defaultDamageType;

    static PrefabCollection[] _prefabs;

    public static void Initialize()
    {
        LoadDamageTypes();
        LoadPrefabCollections();
        LoadActions();
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
    static void LoadActions()
    {
        List<Action> actions = new List<Action>();

        foreach (ActionTemplate at in Resources.LoadAll<ActionTemplate>("Templates/Item Actions/"))
            actions.Add(at.Instantiate());

        _itemActions = actions.ToArray();

        actions.Clear();

        foreach (ActionTemplate at in Resources.LoadAll<ActionTemplate>("Templates/Spells/"))
            actions.Add(at.Instantiate());

        _spells = actions.ToArray();
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
                i = new LightSource(name, "n/a", type, prefab, rarity, 20, 14);
                break;
            default:
                i = null;
                break;
        }

        //setup default actions
        i.GetActions(ActionCategory.Attack).Add(_itemActions.First(a => a.categories.Contains(ActionCategory.Attack)));

        return i;
    }

    //spells
    public static Action Get(string name)
    {
        return _spells.FirstOrDefault(s => s.header == name);
    }
    public static Action GetRandom()
    {
        return _spells.Random();
    }
}