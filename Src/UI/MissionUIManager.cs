using UnityEngine;
using UnityEngine.UI;

public class MissionUIManager : MonoBehaviour
{
    [SerializeField]Transform _list;

    [SerializeField]GameObject _missionItem;
    [SerializeField]GameObject _taskItem;

    GameObject[] _taskItems;

    void Awake()
    {
        GlobalEvents.Subscribe(GlobalEvent.MissionAdded, OnMissionAdded);
    }

    void OnMissionAdded(object[] args)
    {
        Mission mission = args[0] as Mission;
        _taskItems = new GameObject[mission.tasks.Length];

        GameObject m = Instantiate(_missionItem, _list);
        m.transform.Find("title").GetComponent<Text>().text = mission.title;

        for (int i = 0; i < mission.tasks.Length; i++)
        {
            GameObject t = Instantiate(_taskItem, m.transform.Find("list"));
            t.transform.Find("description").GetComponent<Text>().text = mission.tasks[i].ToString();

            _taskItems[i] = t;
        }
    }
}
