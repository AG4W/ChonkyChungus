using MoonSharp.Interpreter;

using ag4w.Actions;

[MoonSharpUserData]
public class ActionContext
{
    public readonly Entity caster;
    public readonly Item item;
    public readonly Action action;

    public ActionContext(Entity caster, Item item, Action action)
    {
        this.caster = caster;
        this.item = item;
        this.action = action;
    }

    public override string ToString()
    {
        return caster?.ToString() + "\n\n" + item?.ToString() + "\n\n" + action?.ToString();
    }
}
