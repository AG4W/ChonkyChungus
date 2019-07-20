using UnityEngine;
using UnityEngine.UI;

public class InventoryUIManager : TabBehaviour
{
    [SerializeField]Transform _list;
    [SerializeField]GameObject _inventoryItem;

    protected override void Awake()
    {
        base.Awake();

        GridLayoutGroup grid = base.tab.GetComponentInChildren<GridLayoutGroup>();

        float s = grid.GetComponent<RectTransform>().rect.width / 10f;
        grid.cellSize = new Vector2(s, s);

        GlobalEvents.Subscribe(GlobalEvent.GameManagerInitialized, (object[] args) =>
        {
            GlobalEvents.Subscribe(GlobalEvent.ActorInventoryChanged, UpdateInventory);
            GlobalEvents.Subscribe(GlobalEvent.ActorEquipmentChanged, UpdateInventory);

            UpdateInventory(Player.actor);
        });
    }

    void UpdateInventory(params object[] args)
    {
        if ((Actor)args[0] != Player.actor)
            return;

        for (int i = 0; i < _list.childCount; i++)
            Destroy(_list.GetChild(i).gameObject);
    
        for (int i = 0; i < Player.data.inventory.Count; i++)
        {
            int a = i;

            Item item = Player.data.inventory[a];
            GameObject g = Instantiate(_inventoryItem, _list);

            g.transform.Find("name").GetComponent<Text>().text = "<color=#" + ColorUtility.ToHtmlStringRGBA(item.GetColor()) + ">" + item.name + "</color>";
            g.transform.Find("status").GetComponent<Text>().text = "";
            g.GetComponent<GenericPointerHandler>().Initialize(
                () => Tooltip.Open(item.ToTooltip()),
                () => OnItemLeftClicked(item),
                null,
                () => OnItemRightclicked(item),
                () => Tooltip.Close());
        }
    }

    void OnItemLeftClicked(Item item)
    {
        if (Player.data.GetEquipment(EquipSlot.LeftHand) == item)
            Player.data.SetEquipment(EquipSlot.LeftHand, null);
        else
            Player.data.SetEquipment(EquipSlot.LeftHand, item);
    }
    void OnItemRightclicked(Item item)
    {
        if (Player.data.GetEquipment(EquipSlot.RightHand) == item)
            Player.data.SetEquipment(EquipSlot.RightHand, null);
        else
            Player.data.SetEquipment(EquipSlot.RightHand, item);
    }
}
