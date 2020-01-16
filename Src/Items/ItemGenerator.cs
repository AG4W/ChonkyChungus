using UnityEngine;

using System.Collections.Generic;
using System.Linq;

using ag4w.Actions;

public static class ItemGenerator
{
    static Action[] _itemActions;
    static Action[] _spells;

    static WeaponCollection[] _weapons;
    static PrefabCollection[] _armours;
    static WeaponCollection[] _offhands;
    static PrefabCollection[] _consumables;

    public static void Initialize()
    {
        LoadCollections();
        LoadActions();
    }

    static void LoadCollections()
    {
        _armours = new PrefabCollection[System.Enum.GetNames(typeof(EquipSlot)).Length];

        for (int i = 2; i < _armours.Length; i++)
            _armours[i] = Resources.Load<PrefabCollection>("Collections/Armours/" + ((EquipSlot)i).ToString());

        _weapons = new WeaponCollection[System.Enum.GetNames(typeof(WeaponType)).Length];
    
        for (int i = 0; i < _weapons.Length; i++)
            _weapons[i] = Resources.Load<WeaponCollection>("Collections/Weapons/" + ((WeaponType)i).ToString());

        _offhands = Resources.LoadAll<WeaponCollection>("Collections/Offhands/");

        _consumables = new PrefabCollection[System.Enum.GetNames(typeof(ConsumableType)).Length];

        for (int i = 0; i < _consumables.Length; i++)
            _consumables[i] = Resources.Load<PrefabCollection>("Collections/Consumables/" + ((ConsumableType)i).ToString());
    }
    static void LoadActions()
    {
        List<Action> actions = new List<Action>();

        foreach (ActionTemplate at in Resources.LoadAll<ActionTemplate>("Templates/Item Actions/"))
            actions.Add(at.Instantiate());

        //todo
        //load additional user data from disk
        _itemActions = actions.ToArray();

        actions.Clear();

        foreach (ActionTemplate at in Resources.LoadAll<ActionTemplate>("Templates/Spells/"))
            actions.Add(at.Instantiate());

        //todo
        //load additional user data from disk
        _spells = actions.ToArray();
    }

    public static Weapon GetWeapon(WeaponType type, ItemRarity rarity)
    {
        WeaponPrefabData wpd = _weapons[(int)type].weapons.Random();
        Weapon weapon = new Weapon(wpd.prefab.name, "n/A", rarity, wpd.animationSet.Random(), wpd.prefab, wpd.requiresBothHands, wpd.useLeftHandIK, type);

        //assign default actions
        switch (type)
        {
            case WeaponType.OnehandedSword:
                break;
            case WeaponType.TwohandedSword:
                break;
            case WeaponType.OnehandedBlunt:
                break;
            case WeaponType.TwohandedBlunt:
                break;
            case WeaponType.Polearm:
                break;
            case WeaponType.Ranged:
                break;
            default:
                break;
        }

        weapon.GetActions(ActionCategory.Activateable).Add(_itemActions.FirstOrDefault(a => a.header == "Precise Strike"));
        //weapon.GetActions(ActionCategory.Activateable).Add(_itemActions.FirstOrDefault(a => a.header == "OP Action of Epic Asspounding"));

        return weapon;
    }
    public static Armour GetArmour(EquipSlot slot, ItemRarity rarity)
    {
        GameObject prefab = _armours[(int)slot].prefabs.Random();

        return new Armour(prefab.name, "n/A", rarity, slot, prefab);
    }
    public static Armour GetSpecificArmour(string filter, EquipSlot slot, ItemRarity rarity)
    {
        Debug.Log(slot);

        GameObject prefab = _armours[(int)slot].prefabs.Where(p => p.name.Contains(filter)).ToArray().Random();

        if(prefab == null)
        {
            Debug.LogWarning("Warning, no prefabs with the word: " + filter + " exist!");
            prefab = _armours[(int)slot].prefabs.Random();
        }

        return new Armour(prefab.name, "n/A", rarity, slot, prefab);
    }
    public static Holdable GetLightSource(ItemRarity rarity)
    {
        WeaponPrefabData wpd = _offhands.Random().weapons.Random();

        return new Holdable("Torch", "A torch", rarity, EquipSlot.LeftHandItem, wpd.prefab, wpd.animationSet.Random(), false, wpd.useLeftHandIK);
    }

    public static Item GetPotion(ItemRarity rarity)
    {
        Item i = new Item("Potion", "n/A", rarity);

        i.GetActions(ActionCategory.Activateable).Add(_itemActions.FirstOrDefault(a => a.header == "Throw"));

        return i;
    }

    //spells
    public static Action GetSpell(string name)
    {
        return _spells.FirstOrDefault(s => s.header == name);
    }
    public static Action GetSpellRandom()
    {
        return _spells.Random();
    }
}