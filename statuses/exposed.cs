using Godot;
using System;

public partial class exposed : status
{
	public float modifier = 0.5f;


    public override string get_tooltip()
    {
		return string.Format(tooltip, duration);
    }


	public override void initialize_status(Node target)
	{
		var modifierHandler = target.Get("modifier_handler").As<ModifierHandler>();
		if (modifierHandler == null)
			throw new InvalidOperationException($"No modifiers on {target}");

		var dmg_taken_modifier = modifierHandler.GetModifier(Modifier.Type.DMG_TAKEN);
		if (dmg_taken_modifier == null)
			throw new InvalidOperationException($"No dmg taken modifier on {target}");
		var exposed_modifier_value = dmg_taken_modifier.GetValue("exposed");

		if (exposed_modifier_value == null)
		{
			exposed_modifier_value = ModifierValue.CreateNewModifier("exposed", ModifierValue.Type.PERCENT_BASED);
			exposed_modifier_value.percent_value = modifier;
			dmg_taken_modifier.AddNewValue(exposed_modifier_value);
		}

		StatusChanged -= () => OnStatusChanged(dmg_taken_modifier);
		StatusChanged += () => OnStatusChanged(dmg_taken_modifier);

	}

	public void OnStatusChanged(Modifier dmg_taken_modifier)
	{
		if (duration <= 0 && dmg_taken_modifier.GetChildCount()!=0)
		{
			dmg_taken_modifier.RemoveValue("exposed");
		}
		
	}



}

