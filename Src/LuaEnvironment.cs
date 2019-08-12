using UnityEngine;

using MoonSharp.Interpreter;

public class LuaEnvironment : MonoBehaviour
{
    public static LuaEnvironment getInstance { get; private set; }

    void Awake()
    {
        getInstance = this;

        UserData.RegisterAssembly();
        Script.DefaultOptions.DebugPrint = (s) => Debug.Log(s);
    }
}
