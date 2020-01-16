using UnityEngine;
using UnityEngine.UI;

public class PreparationUIManager : MonoBehaviour
{
    [Header("Equipment")]
    [SerializeField]GameObject _equipment;
    [SerializeField]GameObject _equipmentListItem;

    void Awake()
    {
        InitializeEquipment();
    }

    void InitializeEquipment()
    {
        Transform list = _equipment.transform.Find("list");

        for (int i = 0; i < System.Enum.GetNames(typeof(EquipSlot)).Length; i++)
        {
            GameObject g = Instantiate(_equipmentListItem, list);

            g.transform.Find("header").GetComponent<Text>().text = ((EquipSlot)i).ToString();
            g.transform.Find("name").GetComponent<Text>().text = "Empty";
            g.transform.Find("size").GetComponent<Text>().text = "";
        }
    }
}
