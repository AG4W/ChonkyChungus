using UnityEngine;
using UnityEngine.UI;

using ag4w.Actions;

public class SpellsUIManager : MonoBehaviour
{
    [SerializeField]GameObject _prefab;
    [SerializeField]Transform _list;

    void Start()
    {
        GlobalEvents.Subscribe(GlobalEvent.ActorSpellsChanged, (object[] args) => UpdateSpells());
        GlobalEvents.Subscribe(GlobalEvent.ActorSelected, (object[] args) => UpdateSpells());
        GlobalEvents.Subscribe(GlobalEvent.HotkeyPressed, OnHotkeyPressed);
    }

    void UpdateSpells()
    {
        for (int i = 0; i < _list.childCount; i++)
            Destroy(_list.GetChild(i).gameObject);
        for (int i = 0; i < Player.selectedActor.data.spells.Count; i++)
            CreateSpellItem(Player.selectedActor.data.spells[i], i);
    }
    void CreateSpellItem(Action action, int index)
    {
        GameObject g = Instantiate(_prefab, _list);

        //yuck
        g.transform.Find("background").Find("icon").GetComponent<Image>().sprite = action.icon;
        g.transform.Find("borders").GetComponent<Image>().color = Color.blue;
        g.transform.Find("index").GetComponent<Text>().text = index.ToString();

        g.GetComponent<GenericPointerHandler>().Initialize(
            () => Tooltip.Open(action.ToString()),
            () => 
            {
                if (action.Validate(Player.selectedActor, null))
                    action.Activate(Player.selectedActor, null);
            },
            null,
            null,
            () => Tooltip.Close());

        //g.GetComponent<GenericPointerHandler>().SetInteractable(action.Validate(Player.actor, null));
    }

    void OnHotkeyPressed(object[] args)
    {
        int index = (int)args[0];

        if (index < _list.childCount)
            _list.GetChild(index).GetComponent<GenericPointerHandler>().Invoke(UnityEngine.EventSystems.PointerEventData.InputButton.Left);
    }
}
