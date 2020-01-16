using UnityEngine;
using UnityEngine.UI;

using System;

public class InventoryUIManager : TabBehaviour
{
    [SerializeField]Transform _list;

    [SerializeField]GameObject _categoryItem;
    [SerializeField]GameObject _inventoryItem;

    GameObject[] _categories;
    GameObject[] _equipmentItems;

    protected override void Awake()
    {
        base.Awake();

        CreateCategories();

        GlobalEvents.Subscribe(GlobalEvent.ActorSelected, (object[] args) => {
            UpdateEquipment(args[0] as Actor);
            UpdateInventory(args[0] as Actor);
        });
        GlobalEvents.Subscribe(GlobalEvent.ActorEquipmentChanged, (object[] args) => UpdateEquipment(args[0] as Actor));
        GlobalEvents.Subscribe(GlobalEvent.ActorInventoryChanged, (object[] args) => UpdateInventory(args[0] as Actor));
    }

    void UpdateEquipment(Actor a)
    {
        for (int i = 0; i < Enum.GetNames(typeof(EquipSlot)).Length; i++)
        {
            EquipSlot slot = (EquipSlot)i;
            Equipable e = a.data.GetEquipment((EquipSlot)i);

            if (e == null)
            {
                _equipmentItems[(int)slot].transform.Find("header").GetComponent<Text>().text = "Empty (" + slot.ToString() + ")";
                _equipmentItems[(int)slot].GetComponent<GenericPointerHandler>().Initialize(
                    null,
                    null,
                    null,
                    null,
                    null);
            }
            else
            {
                bool isEquipped = a.data.GetEquipment(e.slot) == e;

                _equipmentItems[(int)e.slot].transform.Find("header").GetComponent<Text>().text = isEquipped ? e.NameToString() : "Empty (" + e.slot.ToString() + ")";

                if (isEquipped)
                {
                    _equipmentItems[(int)e.slot].GetComponent<GenericPointerHandler>().Initialize(
                        () => Tooltip.Open(e.ToTooltip()),
                        null,
                        null,
                        null,
                        () => Tooltip.Close());
                }
                else
                {
                    _equipmentItems[(int)e.slot].GetComponent<GenericPointerHandler>().Initialize(
                        null,
                        null,
                        null,
                        null,
                        null);
                }
            }
        }
    }
    void UpdateInventory(Actor a)
    {
        for (int i = 0; i < _categories[2].transform.Find("list").childCount; i++)
            Destroy(_categories[2].transform.Find("list").GetChild(i).gameObject);

        for (int i = 0; i < a.data.inventorySize; i++)
        {
            Item item = a.data.GetItem(i);
            GameObject g = Instantiate(_inventoryItem, _categories[2].transform.Find("list"));

            g.transform.Find("header").GetComponent<Text>().text = item == null ?  "Empty" : item.NameToString();

            if(a.data.GetItem(i) != null)
                g.GetComponent<GenericPointerHandler>().Initialize(
                    () => Tooltip.Open(item.ToTooltip()),
                    null,
                    null,
                    null,
                    () => Tooltip.Close());
            else
                g.GetComponent<GenericPointerHandler>().Initialize(
                    null,
                    null,
                    null,
                    null,
                    null);
        }
    }

    void CreateCategories()
    {
        _categories = new GameObject[4];
        _equipmentItems = new GameObject[Enum.GetNames(typeof(EquipSlot)).Length];

        _categories[0] = Instantiate(_categoryItem, _list);
        _categories[0].transform.Find("header").GetComponent<Text>().text = "WEAPONS";
        _categories[0].transform.Find("header").GetComponent<GenericPointerHandler>().Initialize(
            null,
            () => _categories[0].transform.Find("list").gameObject.SetActive(!_categories[0].transform.Find("list").gameObject.activeSelf),
            null,
            null,
            null);

        _categories[1] = Instantiate(_categoryItem, _list);
        _categories[1].transform.Find("header").GetComponent<Text>().text = "EQUIPMENT";
        _categories[1].transform.Find("header").GetComponent<GenericPointerHandler>().Initialize(
            null,
            () => _categories[1].transform.Find("list").gameObject.SetActive(!_categories[1].transform.Find("list").gameObject.activeSelf),
            null,
            null,
            null);

        _categories[2] = Instantiate(_categoryItem, _list);
        _categories[2].transform.Find("header").GetComponent<Text>().text = "CONSUMABLES";
        _categories[2].transform.Find("header").GetComponent<GenericPointerHandler>().Initialize(
            null,
            () => _categories[2].transform.Find("list").gameObject.SetActive(!_categories[2].transform.Find("list").gameObject.activeSelf),
            null,
            null,
            null);

        for (int i = 0; i < Enum.GetNames(typeof(EquipSlot)).Length; i++)
        {
            GameObject g = Instantiate(_inventoryItem, (i <= (int)EquipSlot.LeftHandItem ? _categories[0].transform.Find("list") : _categories[1].transform.Find("list")));
            g.transform.Find("header").GetComponent<Text>().text = "Empty (" + ((EquipSlot)i).ToString() + ")";

            _equipmentItems[i] = g;
            _equipmentItems[i].GetComponent<GenericPointerHandler>().Initialize(
                null,
                null,
                null,
                null,
                null);
        }
    }
}
