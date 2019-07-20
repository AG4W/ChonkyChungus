using UnityEngine;

public class TabBehaviour : MonoBehaviour
{
    [SerializeField]GlobalEvent _toggleEvent;
    [SerializeField]UISoundType _toggleSound;

    [SerializeField]GameObject _tab;

    TabBehaviour[] _tabs;

    public GameObject tab { get { return _tab; } }
    public bool isOpen { get { return _tab.activeSelf; } }

    protected virtual void Awake()
    {
        _tabs = this.GetComponents<TabBehaviour>();
        _tab.SetActive(false);

        GlobalEvents.Subscribe(_toggleEvent, (object[] args) => Toggle());
    }
    void Toggle()
    {
        for (int i = 0; i < _tabs.Length; i++)
            _tabs[i].tab.SetActive(_tabs[i] == this ? !_tabs[i].tab.activeSelf : false);

        if(!tab.activeSelf)
            Tooltip.Close();

        UIAudioManager.Play(_toggleSound);
    }
}