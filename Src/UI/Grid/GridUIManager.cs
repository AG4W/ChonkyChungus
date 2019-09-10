using UnityEngine;
using UnityEngine.UI;

using System;

public class GridUIManager : MonoBehaviour
{
    [SerializeField]GameObject _prefab;
    [SerializeField]Transform _root;

    [SerializeField]Sprite[] _icons;

    void Start()
    {
        for (int i = 0; i < Enum.GetNames(typeof(MapType)).Length; i++)
        {
            MapType m = (MapType)i;

            GameObject b = Instantiate(_prefab, _root);

            b.transform.Find("index").gameObject.SetActive(false);

            b.transform.Find("icon").GetComponent<Image>().sprite = _icons[i];
            b.transform.Find("icon").GetComponent<Image>().color = Map.GetColor(m);

            b.GetComponent<GenericPointerHandler>().Initialize(
                () => Tooltip.Open("Click to toggle visualization of " + Map.StringFormatted(m)),
                () => GlobalEvents.Raise(GlobalEvent.SetGridMap, m),
                null,
                null,
                () => Tooltip.Close());
        }
    }
}
