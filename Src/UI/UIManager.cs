using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [SerializeField]Color _empty = new Color(.5f, .5f, .5f, .5f);

    [SerializeField]Transform _itemList;
    [SerializeField]GameObject _actorItem;
    [SerializeField]GameObject _actorBarItem;

    [SerializeField]Transform _popupList;
    [SerializeField]GameObject _popup;

    Dictionary<Actor, GameObject> _items;

    Camera _camera;

    void Awake()
    {
        _items = new Dictionary<Actor, GameObject>();
        _camera = Camera.main;

        GlobalEvents.Subscribe(GlobalEvent.ActorAdded, (object[] args) => OnActorAdded((Actor)args[0]));
        GlobalEvents.Subscribe(GlobalEvent.ActorVisibilityChanged, (object[] args) => OnActorVisibilityChanged((Actor)args[0], (bool)args[1]));
        GlobalEvents.Subscribe(GlobalEvent.ActorVitalChanged, (object[] args) => OnActorVitalChanged((Actor)args[0], (VitalType)args[1]));
        GlobalEvents.Subscribe(GlobalEvent.ActorRemoved, (object[] args) => OnActorRemoved((Actor)args[0]));

        GlobalEvents.Subscribe(GlobalEvent.PopupRequested, (object[] args) =>
        {
            if (args.Length == 2)
                CreatePopup((Vector3)args[0], (string)args[1]);
            else if (args.Length == 3)
                CreatePopup((Vector3)args[0], (string)args[1], (float)args[2]);
            else
                CreatePopup((Vector3)args[0], (string)args[1], (float)args[2], (float)args[3]);
        });
    }
    void Update()
    {
        foreach (var item in _items)
            if(item.Value.gameObject.activeSelf)
                item.Value.transform.position = _camera.WorldToScreenPoint(item.Key.transform.position);
    }

    void OnActorAdded(Actor a)
    {
        GameObject g = Instantiate(_actorItem, _itemList);

        g.transform.Find("name").GetComponent<Text>().text = a.data.name;

        _items.Add(a, g);

        OnActorVitalChanged(a, VitalType.Health);
        OnActorVitalChanged(a, VitalType.Corruption);
        OnActorVitalChanged(a, VitalType.Stamina);
    }
    void OnActorVisibilityChanged(Actor a, bool isVisible)
    {
        _items[a].SetActive(isVisible);
    }
    void OnActorVitalChanged(Actor a, VitalType vt)
    {
        UpdateVital(_items[a].transform.Find(vt.ToString().ToLower()), a.data.GetVital(vt).current, a.data.GetVital(vt).GetMax(), Vital.ToColor(vt));
    }
    void OnActorRemoved(Actor a)
    {
        Destroy(_items[a].gameObject);

        _items.Remove(a);
    }

    void UpdateVital(Transform root, int currentValue, int maxValue, Color color)
    {
        color.a = .5f;
    
        if(root.childCount != maxValue)
        {
            for (int i = 0; i < root.childCount; i++)
                Destroy(root.GetChild(i).gameObject);
            for (int i = 0; i < maxValue; i++)
                Instantiate(_actorBarItem, root).GetComponent<Image>().color = i < currentValue ? color : _empty;
        }
        else
            for (int i = 0; i < maxValue; i++)
                root.GetChild(i).GetComponent<Image>().color = i < currentValue ? color : _empty;
    }
    void CreatePopup(Vector3 position, string text, float lifetime = 2f, float speed = .25f)
    {
        GameObject g = Instantiate(_popup, _popupList);
    
        g.GetComponent<PopupEntity>().Initialize(text, lifetime, speed, position);
    }
}