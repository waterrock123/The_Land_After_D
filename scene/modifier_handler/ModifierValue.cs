using Godot;
using System;

public partial class ModifierValue : Node
{
	public enum Type
	{
		PERCENT_BASED,//百分比
		FLAT//固定值
	}

	[Export]
	public Type type;
	[Export]
	public float percent_value;
	[Export]
	public int flat_value;
	[Export]
	public string source;

	public static ModifierValue CreateNewModifier(string modifier_source, Type what_type)
	{
		var new_modifier = new ModifierValue();
	
		new_modifier.source = modifier_source;
		new_modifier.type = what_type;
		return new_modifier;
	}
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
