using MoonSharp.Interpreter;

using System.Collections.Generic;

[MoonSharpUserData]
public static class Player
{
    public static ActorData[] characters { get; private set; } = new ActorData[8];
    public static ActorData[] party { get; private set; } = new ActorData[8];

    public static List<Item> storage { get; private set; } = new List<Item>();

    public static Actor selectedActor { get; private set; }

    public static Dictionary<string, string> bestiary { get; private set; } = new Dictionary<string, string>()
    {
        { "Shambler", "Living dead; Very strong and tough, but easy to outmaneuver." },
        { "Crawler", "Living dead; Very strong and tough, but easy to outmaneuver." }
    };

    public static void SelectActor(Actor actor)
    {
        selectedActor = actor;
        GlobalEvents.Raise(GlobalEvent.ActorSelected, selectedActor);
    }
}