using UnityEngine;
using UnityEngine.UI;

public class ActionBarUIManager : MonoBehaviour
{
    [SerializeField]Transform _left;
    [SerializeField]Transform _right;

    [SerializeField]Text _leftHeader;
    [SerializeField]Text _rightHeader;

    [SerializeField]GameObject _actionItem;

    void Awake()
    {
        UpdateItemHeader(_left, _leftHeader, null);
        UpdateItemHeader(_right, _rightHeader, null);

        GlobalEvents.Subscribe(GlobalEvent.ActorEquipmentChanged, OnEquipmentChanged);
    }

    void OnEquipmentChanged(object[] args)
    {
        if (args[0] as Actor != Player.actor)
            return;

        EquipSlot slot = (EquipSlot)args[1];

        if (slot == EquipSlot.LeftHand)
            UpdateItemHeader(_left, _leftHeader, Player.data.GetEquipment(slot));
        else if (slot == EquipSlot.RightHand)
            UpdateItemHeader(_right, _rightHeader, Player.data.GetEquipment(slot));
    }
    void UpdateItemHeader(Transform root, Text header, Item item)
    {
        for (int i = 0; i < root.childCount; i++)
            if(root.GetChild(i) != header.transform)
                Destroy(root.GetChild(i).gameObject);

        //set header
        header.text = item == null ? "Empty" : item.NameToString();
        header.GetComponent<GenericPointerHandler>().Initialize(
            () => {
                if (item != null)
                    Tooltip.Open(item.ToTooltip());
            },
            null,
            null,
            null,
            () => Tooltip.Close());

        //create item actions
        CreateItemAction("Attack", root);

        if (item != null)
            CreateItemAction("Throw", root);
    }
    void CreateItemAction(string tooltip, Transform parent)
    {
        GameObject g = Instantiate(_actionItem, parent);

        g.transform.Find("index").GetComponent<Text>().text = g.transform.GetSiblingIndex().ToString();
        g.GetComponent<GenericPointerHandler>().Initialize(
            () => Tooltip.Open(tooltip),
            null,
            null,
            null,
            () => Tooltip.Close());
    }
}
