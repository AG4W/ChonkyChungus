using UnityEngine;

public class Loader : MonoBehaviour
{
    void Start()
    {
        VFX.Initialize();
        ItemGenerator.Initialize();

        GridManager.Initialize();

        Synched.SetSeed(Random.Range(0, int.MaxValue));

        GameManager.Initialize();
    }

    //void InitializeDebugPlayerData()
    //{
    //    for (int i = 0; i < Player.party.Length; i++)
    //    {
    //        if(Player.party[i] == null)
    //        {
    //            Player.party[i] = Resources.Load<ActorTemplate>("ActorTemplates/player").Instantiate();

    //            for (int j = 2; j < System.Enum.GetNames(typeof(EquipSlot)).Length; j++)
    //            {
    //                EquipSlot slot = (EquipSlot)j;
    //                Player.party[i].SetEquipment(ItemGenerator.GetArmour(slot, ItemRarity.Common));
    //            }

    //            Weapon w = ItemGenerator.GetWeapon((WeaponType)Random.Range(0, System.Enum.GetNames(typeof(WeaponType)).Length), ItemRarity.Common);

    //            if (!w.requiresBothHands)
    //                Player.party[i].SetEquipment(ItemGenerator.GetLightSource(ItemRarity.Common));

    //            Player.party[i].SetEquipment(w);
    //        }
    //    }
    //}
}