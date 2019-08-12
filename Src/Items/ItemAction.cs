using UnityEngine;

using MoonSharp.Interpreter;

public class ItemAction
{
    Script _lua;

    public string header { get; private set; }
    public string description { get; private set; }
    //kanske flavor text

    public Sprite icon { get; private set; }

    public bool requiresTarget { get; private set; }

    public ItemAction(string header, string description, Sprite icon, string lua)
    {
        this.header = header;
        this.description = description;

        this.icon = icon;

        _lua = new Script();
        _lua.DoString(lua);

        UserData.RegisterAssembly();
    }

    public bool InvokeFunction(params object[] args)
    {
        return InvokeFunction(Function.Execute, args);
    }
    public bool InvokeFunction(Function function, params object[] args)
    {
        object func = _lua.Globals[function.ToString().ToLower()];

        if(func == null)
        {
            Debug.LogError("Lua function '" + function + "' is not defined in " + this.header);
            Debug.Log("Did you make a typo?");
            return false;
        }

        DynValue result = _lua.Call(func, args);

        if (result.Type == DataType.Boolean)
            return result.Boolean;
        else if (result.Type == DataType.String && result.String != null)
        {
            Debug.LogError("Lua function '" + function + "' exited with the following error message: " + result.String);
            return false;
        }
        else
            return true;
    }

    public override string ToString()
    {
        return header + "\n" + "<i><color=grey>" + description + "</color></i>";
    }

    public enum Function
    {
        Validate,
        Execute,
    }
}