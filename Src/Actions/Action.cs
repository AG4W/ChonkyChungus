using UnityEngine;

using Object = UnityEngine.Object;

using MoonSharp.Interpreter;
using System;

namespace ag4w.Actions
{
    [MoonSharpUserData]
    public class Action
    {
        Script _lua;

        public string header { get; private set; }

        public Sprite icon { get; private set; }

        //let's not comment the part that touches the one language you're not familiar with
        //fucking genius
        public Action(string header, Sprite icon, string lua)
        {
            this.header = header;
            this.icon = icon;

            _lua = new Script();

            //map lua print to unity debug
            _lua.Options.DebugPrint = s => { Debug.Log(s); };

            //register static classes
            _lua.Globals["GlobalEvents"] = typeof(GlobalEvents);
            _lua.Globals["Pathfinder"] = typeof(Pathfinder);

            //runtime compile
            _lua.DoString(lua);
        }

        public bool Validate(params object[] args)
        {
            return _lua.Call(_lua.Globals["validate"], new ActionContext(args[0] as Entity, args[1] as Item, this)).Boolean;
        }
        /// <summary>
        /// Takes an actor and an item and self-creates a new actioncontext
        /// </summary>
        /// <param name="args"></param>
        public ActionContext Activate(params object[] args)
        {
            return _lua.Call(_lua.Globals["activate"], new ActionContext(args[0] as Entity, args[1] as Item, this)).ToObject<ActionContext>();
        }
        public ActionContext Activate(ActionContext context)
        {
            return _lua.Call(_lua.Globals["activate"], context).ToObject<ActionContext>();
        }
        public void Execute(ActionContext context)
        {
            _lua.Call(_lua.Globals["execute"], context);
        }

        public string GetDescription(Actor caster, Item item)
        {
            return _lua.Call(_lua.Globals["getDescription"], new ActionContext(caster, item, this)).String;
        }

        public override string ToString()
        {
            return header;
        }
    }
}