using UnityEngine;
using UnityEngine.UI;

using System;

public class DisplayUIManager : MonoBehaviour
{
    public static DisplayUIManager getInstance { get; private set; }

    [SerializeField]Transform _equipmentList;
    [SerializeField]Transform _storageList;
    [SerializeField]Transform _vitalList;
    [SerializeField]Transform _attributeList;
    [SerializeField]Transform _statList;

    [SerializeField]GameObject _categoryItem;
    [SerializeField]GameObject _equipmentItem;
    [SerializeField]GameObject _statItem;

    [SerializeField]GameObject _next;
    [SerializeField]GameObject _previous;
    [SerializeField]GameObject _toSelection;

    [SerializeField]InputField _nameField;

    [SerializeField]DisplayModelController _displayModel;

    GameObject[] _categories;

    int _dataIndex = 0;
    ActorData _data;


    void Awake()
    {
        getInstance = this;

        Synched.SetSeed((int)DateTime.Now.Ticks);

        ItemGenerator.Initialize();
        InitializeCategories();
        InitializeDebugPlayerData();
    }
    void Start()
    {
        _next.GetComponent<GenericPointerHandler>().Initialize(
            () => Tooltip.Open("Next party member."),
            () =>
            {
                _dataIndex++;

                if (_dataIndex > Player.characters.Length - 1)
                    _dataIndex = 0;

                SelectActorData(Player.characters[_dataIndex]);
                Tooltip.Close();
            },
            null,
            null,
            () => Tooltip.Close());
        _previous.GetComponent<GenericPointerHandler>().Initialize(
            () => Tooltip.Open("Previous party member."),
            () =>
            {
                _dataIndex--;

                if (_dataIndex < 0)
                    _dataIndex = Player.characters.Length - 1;

                SelectActorData(Player.characters[_dataIndex]);
                Tooltip.Close();
            },
            null,
            null,
            () => Tooltip.Close());
        _toSelection.GetComponent<GenericPointerHandler>().Initialize(
            () => Tooltip.Open("Select party members before you embark on your quest."),
            () =>
            {
                SelectionUIManager.getInstance.gameObject.SetActive(true);

                this.gameObject.SetActive(false);
                Tooltip.Close();

                HubCameraManager.getInstance.GoTo(HubCameraMode.Selection);
            },
            null,
            null,
            () => Tooltip.Close());

        SelectActorData(Player.characters[_dataIndex]);
        HubCameraManager.getInstance.GoTo(HubCameraMode.Customization);
    }
    void InitializeDebugPlayerData()
    {
        for (int i = 0; i < Player.characters.Length; i++)
        {
            if(Player.characters[i] == null)
            {
                Player.characters[i] = Resources.Load<ActorTemplate>("ActorTemplates/player").Instantiate();

                //weapons
                Player.characters[i].SetEquipment(ItemGenerator.GetWeapon((WeaponType)Synched.Next(0, Enum.GetNames(typeof(WeaponType)).Length), ItemRarity.Common));

                for (int j = 2; j < Enum.GetNames(typeof(EquipSlot)).Length; j++)
                    Player.characters[i].SetEquipment(ItemGenerator.GetArmour((EquipSlot)j, ItemRarity.Common));
            }
        }
    }
    void InitializeCategories()
    {
        _categories = new GameObject[3];

        for (int i = 0; i < _categories.Length; i++)
        {
            int a = i;

            _categories[i] = Instantiate(_categoryItem, _equipmentList);
            _categories[i].transform.Find("header").GetComponent<GenericPointerHandler>().Initialize(
                null,
                null,
                null,
                null,
                null);
        }

        _categories[0].transform.Find("header").GetComponent<Text>().text = "WEAPONS";
        _categories[1].transform.Find("header").GetComponent<Text>().text = "EQUIPMENT";
        _categories[2].transform.Find("header").GetComponent<Text>().text = "CONSUMABLES";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            Player.storage.Add(ItemGenerator.GetWeapon((WeaponType)Synched.Next(0, Enum.GetNames(typeof(WeaponType)).Length), ItemRarity.Common));
            UpdateStorage();
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            Player.storage.Add(ItemGenerator.GetArmour((EquipSlot)Synched.Next(2, Enum.GetNames(typeof(EquipSlot)).Length), ItemRarity.Common));
            UpdateStorage();
        }
        if(Input.GetKeyDown(KeyCode.B))
        {
            Player.storage.Add(ItemGenerator.GetLightSource(ItemRarity.Common));
            UpdateStorage();
        }
    }

    void SelectActorData(ActorData data)
    {
        _data = data;
        _displayModel.SetActorData(data);

        _nameField.onEndEdit.RemoveAllListeners();
        _nameField.text = data.name.ToUpper();
        _nameField.onEndEdit.AddListener((string s) => {
            data.SetName(s);
            _nameField.text = data.name.ToUpper();
        });

        UpdateEquipment();
        UpdateStorage();
        UpdateStats();
    }
    void UpdateEquipment()
    {
        for (int i = 0; i < _categories[0].transform.Find("list").childCount; i++)
            Destroy(_categories[0].transform.Find("list").GetChild(i).gameObject);
        for (int i = 0; i < _categories[1].transform.Find("list").childCount; i++)
            Destroy(_categories[1].transform.Find("list").GetChild(i).gameObject);

        for (int i = 0; i < Enum.GetNames(typeof(EquipSlot)).Length; i++)
        {
            Equipable e = _data.GetEquipment((EquipSlot)i);
            GameObject g = Instantiate(_equipmentItem, i > (int)EquipSlot.LeftHandItem ? _categories[1].transform.Find("list") : _categories[0].transform.Find("list"));

            if(e == null)
            {
                g.transform.Find("name").GetComponent<Text>().text = "Empty (" + ((EquipSlot)i).ToString() + ")";
                g.GetComponent<GenericPointerHandler>().Initialize(
                    null,
                    null,
                    null,
                    null,
                    null);
            }
            else
            {
                g.transform.Find("name").GetComponent<Text>().text = e.NameToString();
                g.GetComponent<GenericPointerHandler>().Initialize(
                    () => Tooltip.Open(e.ToTooltip()),
                    () => {
                        _data.Unequip(e.slot);
                        Player.storage.Add(e);
                    
                        Tooltip.Close();
                        UpdateEquipment();
                        UpdateStorage();
                    },
                    null,
                    null,
                    () => Tooltip.Close());
            }
        }
    }
    void UpdateStorage()
    {
        for (int i = 0; i < _storageList.childCount; i++)
            Destroy(_storageList.GetChild(i).gameObject);

        for (int i = 0; i < Player.storage.Count; i++)
        {
            Item item = Player.storage[i];
            GameObject g = Instantiate(_equipmentItem, _storageList);

            g.transform.Find("name").GetComponent<Text>().text = item.NameToString();
            g.GetComponent<GenericPointerHandler>().Initialize(
                    () => Tooltip.Open(item.ToTooltip()),
                    () => {

                        if (item is Equipable e)
                        {
                            if (_data.GetEquipment(e.slot) != null)
                            {
                                Player.storage.Add(_data.GetEquipment(e.slot));

                                _data.Unequip(e.slot);
                            }

                            _data.SetEquipment(e);
                        }

                        Player.storage.Remove(item);
                        Tooltip.Close();
                        UpdateStorage();
                        UpdateEquipment();
                    },
                    null,
                    () => {
                        Player.storage.Remove(item);
                        Tooltip.Close();
                        UpdateStorage();
                    },
                    () => Tooltip.Close());
        }
    }
    void UpdateStats()
    {
        for (int i = 0; i < _vitalList.childCount; i++)
            Destroy(_vitalList.GetChild(i).gameObject);
        for (int i = 0; i < _attributeList.childCount; i++)
            Destroy(_attributeList.GetChild(i).gameObject);
        for (int i = 0; i < _statList.childCount; i++)
            Destroy(_statList.GetChild(i).gameObject);

        for (int i = 0; i < Enum.GetNames(typeof(VitalType)).Length; i++)
        {
            GameObject g = Instantiate(_statItem, _vitalList);

            g.transform.Find("header").GetComponent<Text>().text = ((VitalType)i).ToString();
            g.transform.Find("value").GetComponent<Text>().text = _data.GetVital((VitalType)i).GetMax().ToString();
        }
        for (int i = 0; i < Enum.GetNames(typeof(AttributeType)).Length; i++)
        {
            GameObject g = Instantiate(_statItem, _attributeList);

            g.transform.Find("header").GetComponent<Text>().text = ((AttributeType)i).ToString();
            g.transform.Find("value").GetComponent<Text>().text = _data.GetAttribute((AttributeType)i).value.ToString();
        }
        for (int i = 0; i < Enum.GetNames(typeof(StatType)).Length; i++)
        {
            GameObject g = Instantiate(_statItem, _statList);

            g.transform.Find("header").GetComponent<Text>().text = ((StatType)i).ToString();
            g.transform.Find("value").GetComponent<Text>().text = _data.GetStat((StatType)i).GetValueFormatted();
        }
    }
}
