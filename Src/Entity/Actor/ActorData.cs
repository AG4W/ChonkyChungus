using UnityEngine;

using Action = ag4w.Actions.Action;
using System.Collections.Generic;
using System.Linq;
using System;

using MoonSharp.Interpreter;

[MoonSharpUserData]
public class ActorData
{
    Stat[] _stats;
    Attribute[] _attributes;
    Vital[] _vitals;

    Equipable[] _equipment;
    Item[] _inventory;

    public string name { get; private set; }
    public int inventorySize { get { return _inventory.Length; } }

    public List<Action> spells { get; private set; } = new List<Action>();

    public GameObject prefab { get; private set; }

    public AudioClip[] damageSFX { get; private set; }
    public AudioClip[] deathSFX { get; private set; }

    public RaceAnimationSet raceAnimationSet { get; private set; }

    public ActorData(string name, ActorTemplate template)
    {
        this.name = name;
        this.prefab = template.prefab;

        this.damageSFX = template.damageSFX;
        this.deathSFX = template.deathSFX;

        this.raceAnimationSet = template.raceAnimationSet;

        InitializeStats(template);
        InitializeEquipment();
        InitializeInventory();
    }

    void InitializeStats(ActorTemplate template)
    {
        //stats
        _stats = new Stat[] {
            //misc
            new Stat(StatType.SightRange, StatCategory.Misc,
                () => { return template.sightRange; },
                () => { return "Base: " + template.sightRange; }),
            new Stat(StatType.SightThreshold, StatCategory.Misc,
                () => { return template.sightThreshold; },
                () => { return "Base: " + template.sightThreshold; }),
            new Stat(StatType.WalkRange, StatCategory.Misc,
                () => { return GetVital(VitalType.Stamina).current; },
                () => { return "Stamina: " + GetVital(VitalType.Stamina).current; }),
            new Stat(StatType.SprintRange, StatCategory.Misc,
                () => { return GetVital(VitalType.Stamina).current * 2; },
                () => { return "Stamina x2: " + GetVital(VitalType.Stamina).current * 2; }),
         };

        //attributes
        _attributes = new Attribute[]
        {
            new Attribute(AttributeType.Strength, template.strength,
            () => {
                return "<i><color=grey>Physical strength, it governs your damage values and carry weight.</color></i>\n\n" +
                    "Assigned: +" + GetAttribute(AttributeType.Strength).assigned + " / 25\n\n" +
                    template.race.ToString() + ": +" + template.strength;
            }),
            new Attribute(AttributeType.Vitality, template.vitality,
            () => {
                return "<i><color=grey>Physical conditioning and fitness, it governs your health and stamina.</color></i>\n\n" +
                    "Assigned: +" + GetAttribute(AttributeType.Vitality).assigned + " / 25\n\n" +
                    template.race.ToString() + ": +" + template.vitality;
            }),
            new Attribute(AttributeType.Movement, template.movement,
            () => {
                return "<i><color=grey>Reflexes, reactions and subconscious instincts, it governs parry, dodge and riposte chances.</color></i>\n\n" +
                    "Assigned: +" + GetAttribute(AttributeType.Movement).assigned + " / 25\n\n" +
                    template.race.ToString() + ": +" + template.movement;
            }),
            new Attribute(AttributeType.Willpower, template.willpower,
            () => {
                return "<i><color=grey>Mental fortitude and conditioning, governs your resistance to corruption and spell damage.</color></i>\n\n" +
                    "Assigned: +" + GetAttribute(AttributeType.Willpower).assigned + " / 25\n\n" +
                    template.race.ToString() + ": +" + template.willpower;
            })
        };

        for (int i = 0; i < _attributes.Length; i++)
            _attributes[i].OnAttributeChanged += (AttributeType at) =>
            {
                for (int a = 0; a < _vitals.Length; a++)
                    OnVitalChanged((VitalType)a, 0);

                OnAttributeChanged(at);
            };

        //vitals
        _vitals = new Vital[] {
            new Vital(VitalType.Health,
                () => {
                    return GetAttribute(AttributeType.Vitality).value;
                },
                () => {
                    return
                    "<i><color=grey>Health represents your physical wellness, if it reaches zero, you are permanently dead.</color></i>\n\n" +
                    "Vitality: +" + GetAttribute(AttributeType.Vitality).value;
                }),
            new Vital(VitalType.Corruption,
                () => {
                    return GetAttribute(AttributeType.Willpower).value;
                },
                () => {
                    return
                    "<i><color=grey>Corruption represents the detrimental effects of being exposed to magic, both hostile and your own.\n" +
                    "Upon reaching zero, your mind is gone forever.</color></i>\n\n" +
                    "Willpower: +" + (GetAttribute(AttributeType.Willpower).value * 2);
                }),
            new Vital(VitalType.Stamina,
                () => {
                    return GetAttribute(AttributeType.Movement).value;
                },
                () => {
                    return
                    "<i><color=grey>Stamina represents your physical conditioning, it resets at the start of every turn.</color></i>\n\n" +
                    "Vitality: +" + GetAttribute(AttributeType.Movement).value; })
        };

        for (int i = 0; i < _vitals.Length; i++)
            _vitals[i].OnVitalChanged += (VitalType vt, int change) => OnVitalChanged(vt, change);
    }
    void InitializeEquipment()
    {
        _equipment = new Equipable[Enum.GetNames(typeof(EquipSlot)).Length];
    }
    void InitializeInventory()
    {
        _inventory = new Item[3];
    }

    public void AddSpell(Action spell)
    {
        spells.Add(spell);
        OnSpellsChanged?.Invoke();
    }
    public void RemoveSpell(Action spell)
    {
        spells.Remove(spell);
        OnSpellsChanged?.Invoke();
    }

    public Stat GetStat(StatType st)
    {
        return _stats[(int)st];
    }
    public Attribute GetAttribute(AttributeType at)
    {
        return _attributes[(int)at];
    }
    public Vital GetVital(VitalType vt)
    {
        return _vitals[(int)vt];
    }

    //items
    public void SetItem(Item item, int index)
    {
        if (item == null)
        {
            Debug.LogWarning("Setting null item!\n\n" + new NullReferenceException().StackTrace);
            return;
        }
        if (index > _inventory.Length - 1)
        {
            Debug.LogWarning("Index out of range when attempting to set inventory item!" + new IndexOutOfRangeException().StackTrace);
            return;
        }

        _inventory[index]?.OnDropped();
        _inventory[index] = item;

        OnInventoryChanged?.Invoke(index);
    }
    public bool SetItemIfOpen(Item item)
    {
        for (int i = 0; i < _inventory.Length; i++)
        {
            if(_inventory[i] == null)
            {
                SetItem(item, i);
                return true;
            }
        }

        return false;
    }
    public Item RemoveItem(int index)
    {
        Item item = _inventory[index];

        _inventory[index] = null;

        OnInventoryChanged?.Invoke(index);

        return item;
    }
    public Item GetItem(int index)
    {
        return _inventory[index];
    }

    //equipment
    public void SetEquipment(Equipable equipable)
    {
        if(equipable == null)
        {
            Debug.LogWarning("Equipping null equipable!\n\n" + new NullReferenceException().StackTrace);
            return;
        }

        if (this.GetEquipment(equipable.slot) != null)
            Unequip(equipable.slot);

        if (equipable is Holdable holdable)
        {
            //if we're equipping something that requires both hands, unequip other hand
            if (holdable.requiresBothHands)
                Unequip(equipable.slot == EquipSlot.LeftHandItem ? EquipSlot.RightHandItem : EquipSlot.LeftHandItem);
            //if we're equipping something that doesn't require both hands, check if we already have something
            //in the other hand that requires both hands, and unequip it
            else
            {
                Holdable otherHand = (Holdable)GetEquipment(equipable.slot == EquipSlot.LeftHandItem ? EquipSlot.RightHandItem : EquipSlot.LeftHandItem);

                //if true
                if (otherHand != null && otherHand.requiresBothHands)
                    Unequip(otherHand.slot);
            }
        }

        _equipment[(int)equipable.slot] = equipable;
        _equipment[(int)equipable.slot]?.OnEquip();

        OnEquipped?.Invoke(equipable);
    }
    public void Unequip(EquipSlot slot)
    {
        Equipable e = _equipment[(int)slot];

        _equipment[(int)slot]?.OnUnequip();
        _equipment[(int)slot] = null;

        OnUnequipped?.Invoke(e);
    }
    public Equipable GetEquipment(EquipSlot slot)
    {
        return _equipment[(int)slot];
    }

    public IEnumerable<Item> GetAllItemsWithActions()
    {
        return _equipment.Where(e => e?.GetActions(ActionCategory.Activateable).Count > 0).Concat(_inventory.Where(i => i?.GetActions(ActionCategory.Activateable).Count > 0));
    }

    public void TickItemTurnEffects()
    {
        for (int i = 0; i < _inventory.Length; i++)
            _inventory[i]?.OnNewTurn();
    }

    public void SetName(string name)
    {
        this.name = name;
    }

    public delegate void ActorEquipmentChangedEvent(Equipable equipable);
    public ActorEquipmentChangedEvent OnEquipped;
    public ActorEquipmentChangedEvent OnUnequipped;

    public delegate void ActorInventoryChangedEvent(int index);
    public ActorInventoryChangedEvent OnInventoryChanged;
    public delegate void ActorSpellsChangedEvent();
    public ActorSpellsChangedEvent OnSpellsChanged;

    public delegate void ActorAttributeChangedEvent(AttributeType at);
    public ActorAttributeChangedEvent OnAttributeChanged;
    public delegate void ActorVitalChangedEvent(VitalType vt, int change);
    public ActorVitalChangedEvent OnVitalChanged;
}
