using UnityEngine;
using UnityEngine.UI;

using ag4w.Actions;

public class ActionBarUIManager : MonoBehaviour
{
    [SerializeField]Transform _actions;
    [SerializeField]GameObject _actionItem;
    [SerializeField]GameObject _actionSeparator;

    [SerializeField]Transform _targets;
    [SerializeField]GameObject _targetItem;

    [SerializeField]Transform _casterDetailsList;
    [SerializeField]Transform _targetDetailsList;
    [SerializeField]GameObject _detailListItem;

    [SerializeField]GameObject _info;

    Action _lastSelectedAction;

    void Awake()
    {
        GlobalEvents.Subscribe(GlobalEvent.ActorSelected, (object[] args) => {
            UpdateActionBar(Player.selectedActor);
        });
        GlobalEvents.Subscribe(GlobalEvent.ActorMoveEnd, (object[] args) => {
            UpdateActionBar(Player.selectedActor);
        });
        GlobalEvents.Subscribe(GlobalEvent.ActorRemoved, (object[] args) => { 
            if (_info.activeSelf) 
                DisableInfo();

            UpdateActionBar(Player.selectedActor);
        });

        GlobalEvents.Subscribe(GlobalEvent.ExitTargetingMode, (object[] args) => { 
            if (_info.activeSelf) 
                DisableInfo(); 
        });
        GlobalEvents.Subscribe(GlobalEvent.EndTurn, (object[] args) => { 
            if (_info.activeSelf) 
                DisableInfo(); 
        });

        _targets.gameObject.SetActive(false);
        _info.SetActive(false);
    }
    
    void UpdateActionBar(Actor actor)
    {
        ClearActionBar();

        int ai = 0;

        foreach (Item i in actor.data.GetAllItemsWithActions())
        {
            foreach (Action a in i.GetActions(ActionCategory.Activateable))
                CreateItemAction(i, a, _actions, ai++);

            Instantiate(_actionSeparator, _actions);
        }
    }
    void CreateItemAction(Item item, Action action, Transform parent, int index)
    {
        GameObject g = Instantiate(_actionItem, parent);

        g.transform.Find("icon").GetComponent<Image>().sprite = action.icon;
        g.transform.Find("index").GetComponent<Text>().text = index.ToString();

        g.GetComponent<GenericPointerHandler>().Initialize(
           () => Tooltip.Open(action.header + " (" + item.NameToString() + ")\n\n" + action.GetDescription(Player.selectedActor, item)),
           () => OnActionClicked(action, item),
           null,
           null,
           () => Tooltip.Close());

        g.GetComponent<GenericPointerHandler>().SetInteractable(action.Validate(Player.selectedActor, item));
    }
    void ClearActionBar()
    {
        for (int i = 0; i < _actions.childCount; i++)
            Destroy(_actions.GetChild(i).gameObject);
    }

    void OnActionClicked(Action action, Item item)
    {
        if (_info.activeSelf && _lastSelectedAction == action)
            DisableInfo();
        else if(!_info.activeSelf)
            EnableInfo(action, item);
    }

    void EnableInfo(Action action, Item item)
    {
        GlobalEvents.Raise(GlobalEvent.EnterTargetingMode);
        ActionContext context = action.Activate(Player.selectedActor, item);

        _info.SetActive(true);
        _info.transform.Find("confirmation").transform.Find("confirm").GetComponent<Text>().text = ">> " + action.header + " <<";
        _info.transform.Find("description").GetComponent<Text>().text = action.GetDescription(Player.selectedActor, item);
        _info.transform.Find("confirmation").GetComponent<GenericPointerHandler>().Initialize(
            null,
            () => {
                action.Execute(context);
                DisableInfo();
            },
            null,
            null,
            null);

        UpdateAvailableTargets(context);

        _targets.gameObject.SetActive(true);
        _lastSelectedAction = action;
    }
    void UpdateInfo(Actor target)
    {
        Transform ci = _info.transform.Find("criticalInfo");
        Weapon w = Player.selectedActor.data.GetEquipment(EquipSlot.RightHandItem) as Weapon;

        ci.transform.Find("hit").GetComponent<Text>().text = "<color=green>100</color>%";
        ci.transform.Find("dmg").GetComponent<Text>().text = "Damage: <color=red>" + w.minDamage + "</color> - <color=green>" + w.maxDamage + "</color>";
        ci.transform.Find("crit").GetComponent<Text>().text = "<color=yellow>5</color>%";

        ClearDetailItems();

        CreateDetailItem("Distance: " + Pathfinder.Distance(Player.selectedActor.tile, target.tile), "0%", _casterDetailsList);
        CreateDetailItem("Light: ", target.tile.luminosity * 100f + "%", _targetDetailsList);
    }
    void DisableInfo()
    {
        _targets.gameObject.SetActive(false);
        _info.SetActive(false);

        GlobalEvents.Raise(GlobalEvent.ExitTargetingMode);
    }

    void ClearDetailItems()
    {
        for (int i = 0; i < _casterDetailsList.childCount; i++)
            Destroy(_casterDetailsList.GetChild(i).gameObject);
        for (int i = 0; i < _targetDetailsList.childCount; i++)
            Destroy(_targetDetailsList.GetChild(i).gameObject);
    }
    void CreateDetailItem(string header, string value, Transform list)
    {
        GameObject g = Instantiate(_detailListItem, list);

        g.transform.Find("header").GetComponent<Text>().text = header;
        g.transform.Find("value").GetComponent<Text>().text = value;
    }

    void UpdateAvailableTargets(ActionContext context)
    {
        ClearAvailableTargets();

        for (int i = 0; i < context.actors.Count; i++)
        {
            Actor a = context.actors[i];
            GameObject g = Instantiate(_targetItem, _targets);

            g.GetComponent<GenericPointerHandler>().Initialize(
                () => {
                    GlobalEvents.Raise(GlobalEvent.JumpCameraTo, a.tile.position);
                    Player.selectedActor.transform.rotation = Quaternion.LookRotation((a.tile.position - Player.selectedActor.transform.position).normalized, Vector3.up);

                    context.actors.Clear();
                    context.actors.Add(a);

                    UpdateInfo(a);
                    Tooltip.Open(a.name);
                },
                null,
                null,
                null,
                () => {
                    Tooltip.Close();
                });
        }

        //autofocus on first target
        GlobalEvents.Raise(GlobalEvent.JumpCameraTo, context.actors[0].tile.position);
        Player.selectedActor.transform.rotation = Quaternion.LookRotation((context.actors[0].tile.position - Player.selectedActor.transform.position).normalized, Vector3.up);
        UpdateInfo(context.actors[0]);
    }
    void ClearAvailableTargets ()
    {
        for (int i = 0; i < _targets.childCount; i++)
            Destroy(_targets.GetChild(i).gameObject);
    }
}
