using UnityEngine;

public class Loader : MonoBehaviour
{
    void Start()
    {
        //setup cursor
        ItemGenerator.Initialize();

        Synched.SetSeed(Random.Range(0, int.MaxValue));
        GameManager.Initialize();
    }
}