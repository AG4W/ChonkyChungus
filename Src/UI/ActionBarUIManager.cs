using UnityEngine;
using UnityEngine.UI;

using ag4w.Actions;

public class ActionBarUIManager : MonoBehaviour
{
    [SerializeField]Transform _left;
    [SerializeField]Transform _right;

    [SerializeField]Text _leftHeader;
    [SerializeField]Text _rightHeader;

    [SerializeField]GameObject _actionItem;

    [SerializeField]Transform _targets;
    [SerializeField]GameObject _targetItem;

    void Awake()
    {
        UpdateItemHeader(_left, _leftHeader, null);
        UpdateItemHeader(_right, _rightHeader, null);

        GlobalEvents.Subscribe(GlobalEvent.ActorEquipmentChanged, OnEquipmentChanged);
        GlobalEvents.Subscribe(GlobalEvent.ActorMapChanged, OnActorMapChanged);

        GlobalEvents.Subscribe(GlobalEvent.ActorAdded, delegate(object[] args) {
            if(args[0] is Actor a && a == Player.actor)
                UpdateTargetsBar();
        });
        GlobalEvents.Subscribe(GlobalEvent.ActorAdded, (object[] args) => UpdateTargetsBar());

        //fix this
        //GlobalEvents.Subscribe(GlobalEvent.ActorTargetsChanged, (object[] args) => UpdateTargetsBar());
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

        UpdateTargetsBar();
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
        if(item != null)
            for (int i = 0; i < item.GetActions(ActionCategory.Attack).Count; i++)
                CreateItemAction(item, item.GetActions(ActionCategory.Attack)[i], root, i);
    }
    void CreateItemAction(Item item, Action action, Transform parent, int index)
    {
        GameObject g = Instantiate(_actionItem, parent);

        g.transform.Find("background").Find("icon").GetComponent<Image>().sprite = action.icon;
        g.transform.Find("index").GetComponent<Text>().text = index.ToString();

        g.GetComponent<GenericPointerHandler>().Initialize(
           () => Tooltip.Open(action.ToString()),
           () => action.Activate(Player.actor, item),
           null,
           null,
           () => Tooltip.Close());
        g.GetComponent<GenericPointerHandler>().SetInteractable(action.Validate(Player.actor, item));
    }

    void OnActorMapChanged(object[] args)
    {
        if(args[0] is Actor a && a == Player.actor)
            UpdateTargetsBar();
    }
    void UpdateTargetsBar()
    {
        for (int i = 0; i < _targets.childCount; i++)
            Destroy(_targets.GetChild(i).gameObject);
        for (int i = 0; i < Player.actor.visibleActors.Count; i++)
            CreateTargetItem(Player.actor.visibleActors[i]);
    }
    void CreateTargetItem(Actor target)
    {
        GameObject g = Instantiate(_targetItem, _targets);

        //g.transform.Find("index").GetComponent<Text>().text = g.transform.GetSiblingIndex().ToString();
        g.GetComponent<GenericPointerHandler>().Initialize(
            () => GlobalEvents.Raise(GlobalEvent.SetTargetIndex, Player.actor.visibleActors.IndexOf(target)),
            null,
            null,
            null,
            null);
    }
}
