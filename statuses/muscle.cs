using Godot;
using System;

public partial class muscle : status
{
	public int member_var = 0;


	public override void initialize_status(Node target)
	{

		StatusChanged += ()=>OnStatusChanged(target);
		OnStatusChanged(target);
	}

    public override string get_tooltip()
    {
		return string.Format(tooltip, stacks);
    }

	public void OnStatusChanged(Node target)
	{
		//寻找力量状态，如果没有则创建一个新的力量修饰符，并给与堆叠数
		var modifierHandler = target.Get("modifier_handler").As<ModifierHandler>();
		if (modifierHandler == null)
			throw new InvalidOperationException($"No modifiers on {target}");
		var dmg_dealt_modifier = modifierHandler.GetModifier(Modifier.Type.DMG_DEALT);
		if (dmg_dealt_modifier == null)
			throw new InvalidOperationException($"No dmg dealt modifier on {target}");
		var muscle_modifier_value = dmg_dealt_modifier.GetValue("muscle");
		if (muscle_modifier_value == null)
		{
			muscle_modifier_value = ModifierValue.CreateNewModifier("muscle", ModifierValue.Type.FLAT);

		}
		muscle_modifier_value.flat_value = stacks;
		dmg_dealt_modifier.AddNewValue(muscle_modifier_value);


	}
	
}

