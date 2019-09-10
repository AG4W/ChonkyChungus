using UnityEngine;

using ag4w.Actions;

[CreateAssetMenu(menuName = "Templates/Action")]
public class ActionTemplate : ScriptableObject
{
    [SerializeField]string _header;
    [TextArea(3, 10)][SerializeField]string _description;

    [SerializeField]Sprite _icon;

    [SerializeField]AnimationSet _animationSet;
    [SerializeField]int _animation;

    [SerializeField]ActionCategory[] _categories;

    [TextArea(3, 50)][SerializeField]string _lua = @"
        --[[
            validate must always return a bool
            validate decides wether or not an action can be activated
        --]]
        function validate(context)
	        --this condition determines wether or not this action can be activated.
	        return true
        end

        --[[
            callbacks to c# and opens Target Selection
	        with the specified parameters

            any string returned will be displayed as an error message.
        --]]
        function activate(context)
	        --[[
                If you need target(s) selection, do your pre-target selection logic here

                and raise the follow line.
	        --]]
	        GlobalEvents.Raise(29, context)

	        --If you don't care about target(s) selection just call the line below
	        --execute(executee)
        end

        --[[
            execute should contain logic operations for the action

            any string returned will be displayed as an error message.
        --]]
        function execute(context)
	        for k,v in pairs(context.caster.targets) do
	        end
        end";

    public Action Instantiate()
    {
        return new Action(_header, _description, _icon, _animationSet, _animation, _categories, _lua);
    }
}
