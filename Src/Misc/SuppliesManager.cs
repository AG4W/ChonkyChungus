using UnityEngine;
using UnityEngine.UI;

public class SuppliesManager : MonoBehaviour
{
    [SerializeField]GameObject _supplyWindow;

    int _turnsSinceLastResupply = 0;

    void Awake()
    {
        //GlobalEvents.Subscribe(GlobalEvent.NewTurn, (object[] args) => {
        //    if (GameManager.turnIndex != 0)
        //        return;

        //    _turnsSinceLastResupply++;

        //    if(_turnsSinceLastResupply > 8 && Synched.Next(0, 1 + 1) == 1)
        //    {
        //        RaiseSupplyWindow();
        //        //trigger resupply
        //        _turnsSinceLastResupply = 0;
        //    }
        //});
    }

    void RaiseSupplyWindow()
    {
        _supplyWindow.SetActive(true);
    }
    void CloseSupplyWindow()
    {
        _supplyWindow.SetActive(false);
    }
}
