using UnityEngine;

using MoonSharp.Interpreter;

namespace ag4w.Actions
{
    [MoonSharpUserData]
    public class Action
    {
        Script _lua;

        public string header { get; private set; }
        public string description { get; private set; }
        //kanske flavor text

        public Sprite icon { get; private set; }

        public AnimationSet animationSet { get; private set; }
        public int animation { get; private set; }

        public ActionCategory[] categories { get; private set; }

        public Action(string header, string description, Sprite icon, AnimationSet animationSet, int animation, ActionCategory[] categories, string lua)
        {
            this.header = header;
            this.description = description;

            this.icon = icon;

            this.animationSet = animationSet;
            this.animation = animation;

            this.categories = categories;

            _lua = new Script();
            _lua.Globals["GlobalEvents"] = typeof(GlobalEvents);

            _lua.DoString(lua);
        }

        public bool Validate(params object[] args)
        {
            return _lua.Call(_lua.Globals["validate"], new ActionContext(args[0] as Entity, args[1] as Item, this)).Boolean;
        }
        public void Activate(params object[] args)
        {
            _lua.Call(_lua.Globals["activate"], new ActionContext(args[0] as Entity, args[1] as Item, this));
        }
        public void Execute(params object[] args)
        {
            _lua.Call(_lua.Globals["execute"], args);
        }

        public override string ToString()
        {
            return header + "\n" + "<i><color=grey>" + description + "</color></i>";
        }
    }
}