using UnityEngine;

using System.Collections.Generic;
using System;

using MoonSharp.Interpreter;

[MoonSharpUserData]
public class ActorData
{
    Stat[] _stats;
    Attribute[] _attributes;
    Vital[] _vitals;

    Item[] _equipment;

    Actor _actor;

    public string name { get; private set; }

    public List<Item> inventory { get; private set; }
    public List<Spell> spellbook { get; private set; }

    public GameObject prefab { get; private set; }

    public AudioClip[] damageSFX { get; private set; }
    public AudioClip[] deathSFX { get; private set; }

    public ActorData(string name, ActorTemplate template)
    {
        this.name = name;
        this.prefab = template.prefab;

        this.damageSFX = template.damageSFX;
        this.deathSFX = template.deathSFX;

        InitializeStats(template);
        InitializeEquipment();
        InitializeInventory();
        InitializeSpellbook();
    }
    public void Initialize(Actor actor)
    {
        _actor = actor;
    }

    void InitializeStats(ActorTemplate template)
    {
        //stats
        _stats = new Stat[] {
            //misc
            new Stat(StatType.SightRange, StatCategory.Misc,
                () => { return Race.GetBaseSightRange(template.race); },
                () => { return "Base: " + Race.GetBaseSightRange(template.race); }),
            new Stat(StatType.SightThreshold, StatCategory.Misc,
                () => { return Race.GetBaseLuminosityThreshold(template.race); },
                () => { return "Base: " + Race.GetBaseLuminosityThreshold(template.race); }),
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
            new Attribute(AttributeType.Strength, template.GetModifier(AttributeType.Strength) + template.strength,
            () => {
                return "<i><color=grey>Physical strength, it governs your damage values and carry weight.</color></i>\n\n" +
                    "Assigned: +" + GetAttribute(AttributeType.Strength).assigned + " / 25\n\n" +
                    template.race.ToString() + ": +" + template.GetModifier(AttributeType.Strength) + "\n" +
                    "Birth: +" + template.strength;
            }),
            new Attribute(AttributeType.Vitality, template.GetModifier(AttributeType.Vitality) + template.vitality,
            () => {
                return "<i><color=grey>Physical conditioning and fitness, it governs your health and stamina.</color></i>\n\n" +
                    "Assigned: +" + GetAttribute(AttributeType.Vitality).assigned + " / 25\n\n" +
                    template.race.ToString() + ": +" + template.GetModifier(AttributeType.Vitality) + "\n" +
                    "Birth: +" + template.vitality;
            }),
            new Attribute(AttributeType.Quickness, template.GetModifier(AttributeType.Quickness) + template.quickness,
            () => {
                return "<i><color=grey>Reflexes, reactions and subconscious instincts, it governs parry, dodge and ripose chances.</color></i>\n\n" +
                    "Assigned: +" + GetAttribute(AttributeType.Quickness).assigned + " / 25\n\n" +
                    template.race.ToString() + ": +" + template.GetModifier(AttributeType.Quickness) + "\n" +
                    "Birth: +" + template.quickness;
            }),
            new Attribute(AttributeType.Accuracy, template.GetModifier(AttributeType.Accuracy) + template.accuracy,
            () => {
                return "<i><color=grey>Hand-to-eye coordination and precision, governs hit and critical chances.</color></i>\n\n" +
                    "Assigned: +" + GetAttribute(AttributeType.Accuracy).assigned + " / 25\n\n" +
                    template.race.ToString() + ": +" + template.GetModifier(AttributeType.Accuracy) + "\n" +
                    "Birth: +" + template.accuracy;
            }),
            new Attribute(AttributeType.Willpower, template.GetModifier(AttributeType.Willpower) + template.willpower,
            () => {
                return "<i><color=grey>Mental fortitude and conditioning, governs your resistance to corruption and spell damage.</color></i>\n\n" +
                    "Assigned: +" + GetAttribute(AttributeType.Willpower).assigned + " / 25\n\n" +
                    template.race.ToString() + ": +" + template.GetModifier(AttributeType.Willpower) + "\n" +
                    "Birth: +" + template.willpower;
            })
        };

        for (int i = 0; i < _attributes.Length; i++)
            _attributes[i].OnAttributeChanged += (AttributeType at) =>
            {
                for (int a = 0; a < _vitals.Length; a++)
                    OnVitalChanged((VitalType)a);

                OnAttributeChanged(at);
            };

        //vitals
        _vitals = new Vital[] {
            new Vital(VitalType.Health,
                () => {
                    return GetAttribute(AttributeType.Vitality).value / 2 + 3;
                },
                () => {
                    return
                    "<i><color=grey>Health represents your physical wellness, if it reaches zero, you are dead permanently.</color></i>\n\n" +
                    "Vitality: +" + (GetAttribute(AttributeType.Vitality).value / 2 + 3);
                }),
            new Vital(VitalType.Corruption,
                () => {
                    return GetAttribute(AttributeType.Willpower).value * 2;
                },
                () => {
                    return
                    "<i><color=grey>Corruption represents the detrimental effects of being exposed to magic, both hostile and your own.\n" +
                    "Upon reaching zero, your mind is gone forever.</color></i>\n\n" +
                    "Willpower: +" + (GetAttribute(AttributeType.Willpower).value * 2);
                }),
            new Vital(VitalType.Stamina,
                () => {
                    return GetAttribute(AttributeType.Vitality).value / 4 + 5;
                },
                () => {
                    return
                    "<i><color=grey>Stamina represents your physical conditioning, it resets at the start of every turn.</color></i>\n\n" +
                    "Vitality: +" + (GetAttribute(AttributeType.Vitality).value / 4 + 5); })
        };

        for (int i = 0; i < _vitals.Length; i++)
            _vitals[i].OnVitalChanged += (VitalType vt) => OnVitalChanged(vt);
    }
    void InitializeEquipment()
    {
        _equipment = new Item[Enum.GetNames(typeof(EquipSlot)).Length];
    }
    void InitializeInventory()
    {
        this.inventory = new List<Item>();

        GlobalEvents.Subscribe(GlobalEvent.NewTurn, (object[] args) =>
        {
            if(args[0] is Actor a && a == _actor)
                TickItemTurnEffects();
        });
    }
    void InitializeSpellbook()
    {
        this.spellbook = new List<Spell>();
    }

    public void AddSpell(Spell spell)
    {
        spellbook.Add(spell);
        GlobalEvents.Raise(GlobalEvent.ActorSpellbookChanged, _actor);
    }
    public void RemoveSpell(Spell spell)
    {
        spellbook.Remove(spell);
        GlobalEvents.Raise(GlobalEvent.ActorSpellbookChanged, _actor);
    }

    public void AddItem(Item item)
    {
        item.OnPickUp(_actor);

        if (item != null)
        {
            inventory.Add(item);
            GlobalEvents.Raise(GlobalEvent.ActorInventoryChanged, _actor);
        }
        else
        {
            Exception e = new NullReferenceException();
            Debug.LogWarning("Adding null item to inventory!");
            Debug.Log(e.StackTrace);
        }
    }
    public void RemoveItem(Item item)
    {
        item.OnDropped();

        for (int i = 0; i < _equipment.Length; i++)
        {
            if(_equipment[i] == item)
            {
                new SetEquipmentCommand(_actor, (EquipSlot)i, null);
                break;
            }
        }

        inventory.Remove(item);
        GlobalEvents.Raise(GlobalEvent.ActorInventoryChanged, _actor);
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

    public void SetEquipment(EquipSlot slot, Item item)
    {
        if(item != null)
        {
            //if not in inventory
            if (this.inventory.IndexOf(item) == -1)
                AddItem(item);

            //if already equipped
            if (_equipment.IndexOf(item) != -1)
                SetEquipment((EquipSlot)_equipment.IndexOf(item), null);

            //hands
            if (slot == EquipSlot.LeftHand || slot == EquipSlot.RightHand)
            {
                //if we're equipping something that requires both hands, unequip other hand
                if (item.damageType.requiresBothHands)
                    SetEquipment(slot == EquipSlot.LeftHand ? EquipSlot.RightHand : EquipSlot.LeftHand, null);
                //if we're equipping something that doesn't require both hands, check if we already have something
                //in the other hand that requires both hands, and unequip it
                else
                {
                    Item otherHand = GetEquipment(slot == EquipSlot.LeftHand ? EquipSlot.RightHand : EquipSlot.LeftHand);

                    //if 
                    if (otherHand != null)
                        if (otherHand.damageType.requiresBothHands)
                            SetEquipment(slot == EquipSlot.LeftHand ? EquipSlot.RightHand : EquipSlot.LeftHand, null);
                }
            }
        }

        //old
        _equipment[(int)slot]?.OnUnequip();
        //assign
        _equipment[(int)slot] = item;
        //new
        _equipment[(int)slot]?.OnEquip();

        OnEquipmentChanged?.Invoke(slot);
        GlobalEvents.Raise(GlobalEvent.ActorEquipmentChanged, _actor, slot);
    }
    public Item GetEquipment(EquipSlot slot)
    {
        return _equipment[(int)slot];
    }

    void TickItemTurnEffects()
    {
        for (int i = 0; i < this.inventory.Count; i++)
            this.inventory[i]?.OnNewTurn();
    }

    public delegate void EquipmentChangedEvent(EquipSlot slot);
    public EquipmentChangedEvent OnEquipmentChanged;

    public delegate void AttributeChangedEvent(AttributeType at);
    public AttributeChangedEvent OnAttributeChanged;
    public delegate void VitalChangedEvent(VitalType vt);
    public VitalChangedEvent OnVitalChanged;
}
