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
	--GlobalEvents.Raise(29, context)

	--If you don't care about target(s) selection just call the line below
	execute(context)
end

--[[
	execute should contain logic operations for the action
	any string returned will be displayed as an error message.
--]]
function execute(context)
	for k,v in pairs(context.caster.targets) do
		v.data.GetVital(0).Update(-3)
	end

	context.caster.data.GetVital(1).Update(1)
	context.caster.data.GetVital(2).Update(-1)
end