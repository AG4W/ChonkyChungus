using UnityEngine;

public class Loader : MonoBehaviour
{
    void Start()
    {
        //setup cursor
        ItemGenerator.Initialize();

        GridManager.Initialize();
        TargetingManager.Initalize();

        Synched.SetSeed(Random.Range(0, int.MaxValue));
        GameManager.Initialize();
    }
}