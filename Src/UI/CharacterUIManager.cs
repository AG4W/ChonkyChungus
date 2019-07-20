using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections.Generic;

public class CharacterUIManager : TabBehaviour
{
    [SerializeField]Text _header;

    [SerializeField]Transform _offensiveList;
    [SerializeField]Transform _defensiveList;
    [SerializeField]Transform _miscList;
    [SerializeField]Transform _attributeList;
    [SerializeField]Transform _vitalList;

    Transform[] _lists;

    [SerializeField]GameObject _headerValueItem;

    GameObject[] _statItems;
    GameObject[] _attributeItems;
    GameObject[] _vitalItems;

    protected override void Awake()
    {
        base.Awake();

        GlobalEvents.Subscribe(GlobalEvent.GameManagerInitialized, (object[] args) =>
        {
            InitializeStats();

            GlobalEvents.Subscribe(GlobalEvent.ActorAttributeChanged, OnAttributeChanged);
            GlobalEvents.Subscribe(GlobalEvent.ActorVitalChanged, OnVitalChanged);
            GlobalEvents.Subscribe(GlobalEvent.ActorEquipmentChanged, UpdateStats);
            GlobalEvents.Subscribe(GlobalEvent.ActorLeveledUp, ToggleAttributeLevelUps);
        });
    }

    void InitializeStats()
    {
        _lists = new Transform[] { _offensiveList, _defensiveList, _miscList, _attributeList, _vitalList };
    
        for (int i = 0; i < _lists.Length; i++)
        {
            int a = i;

            _lists[i].parent.Find("subHeader").GetComponent<GenericPointerHandler>().Initialize(
                null,
                () => ToggleList(_lists[a], _lists[a].parent.Find("subHeader/toggle").GetComponent<RectTransform>()),
                null,
                null,
                null);
        }

        _statItems = new GameObject[Enum.GetNames(typeof(StatType)).Length];
        _attributeItems = new GameObject[Enum.GetNames(typeof(AttributeType)).Length];
        _vitalItems = new GameObject[Enum.GetNames(typeof(VitalType)).Length];

        for (int i = 0; i < _attributeItems.Length; i++)
        {
            int a = i;
        
            _attributeItems[a] = CreateValueHeaderItem(
                ((AttributeType)a).ToString(),
                Player.data.GetAttribute((AttributeType)a).value.ToString(),
                Attribute.ToColor((AttributeType)a),
                _attributeList,
                () => Tooltip.Open(Player.data.GetAttribute((AttributeType)a).GetTooltip()),
                null,
                null,
                null,
                () => Tooltip.Close());

            _attributeItems[a].transform.Find("levelup").GetComponent<GenericPointerHandler>().Initialize(
                null,
                () => Player.IncrementAttribute((AttributeType)a),
                null,
                null,
                null);
            _attributeItems[a].transform.Find("levelup").gameObject.SetActive(Player.spendableLevels > 0);
        }
        for (int i = 0; i < _vitalItems.Length; i++)
        {
            int a = i;

            _vitalItems[a] = CreateValueHeaderItem(
                ((VitalType)i).ToString(),
                Player.data.GetVital((VitalType)i).current + " / " + Player.data.GetVital((VitalType)i).GetMax(),
                Vital.ToColor((VitalType)i),
                _vitalList,
                () => Tooltip.Open(Player.data.GetVital((VitalType)a).ToTooltip()),
                null,
                null,
                null,
                () => Tooltip.Close());
        }
        for (int i = 0; i < Enum.GetNames(typeof(StatType)).Length; i++)
        {
            Stat s = Player.data.GetStat((StatType)i);

            _statItems[i] = CreateValueHeaderItem(
                s.GetHeader(),
                s.GetValueFormatted(),
                s.category == StatCategory.Offensive ? Color.red : (s.category == StatCategory.Defensive ? Color.blue : Color.yellow),
                s.category == StatCategory.Offensive ? _offensiveList : (s.category == StatCategory.Defensive ? _defensiveList : _miscList),
                () => Tooltip.Open(s.ToTooltip()),
                null,
                null,
                null,
                () => Tooltip.Close());
        }
    }

    void OnAttributeChanged(object[] args)
    {
        if ((Actor)args[0] != Player.actor)
            return;

        _attributeItems[(int)args[1]].transform.Find("value").GetComponent<Text>().text = Player.data.GetAttribute((AttributeType)args[1]).value.ToString();
    
        UpdateStats(args);
        ToggleAttributeLevelUps(args);
    }
    void OnVitalChanged(object[] args)
    {
        if ((Actor)args[0] != Player.actor)
            return;

        _vitalItems[(int)args[1]].transform.Find("value").GetComponent<Text>().text = Player.data.GetVital((VitalType)args[1]).current + " / " + Player.data.GetVital((VitalType)args[1]).GetMax();

        UpdateStats(args);
    }
    void UpdateStats(object[] args)
    {
        if ((Actor)args[0] != Player.actor)
            return;

        for (int i = 0; i < Enum.GetNames(typeof(StatType)).Length; i++)
            _statItems[i].transform.Find("value").GetComponent<Text>().text = Player.data.GetStat((StatType)i).GetValueFormatted();
    }

    GameObject CreateValueHeaderItem(string header, string value, Color iconColor, Transform list, Action onEnter = null, Action onLeft = null, Action onScroll = null, Action onRight = null, Action onExit = null)
    {
        GameObject g = Instantiate(_headerValueItem, list);

        g.transform.Find("header").GetComponent<Text>().text = header;
        g.transform.Find("value").GetComponent<Text>().text = value;
        g.transform.Find("icon").GetComponent<Image>().color = iconColor;
        g.transform.Find("levelup").gameObject.SetActive(false);

        g.GetComponent<GenericPointerHandler>().Initialize(onEnter, onLeft, onScroll, onRight, onExit);

        return g;
    }
    void ToggleList(Transform list, RectTransform toggleImage)
    {
        list.gameObject.SetActive(!list.gameObject.activeSelf);
        toggleImage.rotation = Quaternion.Euler(0, 0, list.gameObject.activeSelf ? 180 : 0);
    }
    void ToggleAttributeLevelUps(object[] args)
    {
        if ((Actor)args[0] != Player.actor)
            return;

        for (int i = 0; i < _attributeItems.Length; i++)
            _attributeItems[i].transform.Find("levelup").gameObject.SetActive(Player.spendableLevels > 0 && Player.data.GetAttribute((AttributeType)i).assigned < 25);
    }
}
