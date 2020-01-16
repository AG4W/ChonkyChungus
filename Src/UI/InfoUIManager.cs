using UnityEngine;
using UnityEngine.UI;

public class InfoUIManager : MonoBehaviour
{
    [SerializeField]Text _timeSpentHeader;
    
    int _timeSpentInDungeon = 0;

    void Awake()
    {
        _timeSpentHeader.text = _timeSpentInDungeon + (_timeSpentInDungeon > 1 ? " turns" : " turn");

        GlobalEvents.Subscribe(GlobalEvent.EndTurn, (object[] args) => UpdateTime());
    }

    void UpdateTime()
    {
        _timeSpentInDungeon++;
        _timeSpentHeader.text = _timeSpentInDungeon + (_timeSpentInDungeon > 1 ? " turns" : " turn");
    }
}
