using Godot;
using System;
using System.Security.Cryptography.X509Certificates;

public partial class Modifier : Node
{
	//这个节点是装ModifierValue的
	public enum Type
	{
		DMG_DEALT,
		DMG_TAKEN,
		CARD_COST,
		SHOP_COST,
		NO_MODIFIER
	}

	[Export]
	public Type type;

	public override void _Ready()
	{


	}


	public ModifierValue GetValue(string source)
	{
		foreach (ModifierValue value in GetChildren())
		{
			if (value.source == source)
			{
				return value;
			}
		}
		return null;
	}

	public void AddNewValue(ModifierValue value)
	{
		var modifier_value = GetValue(value.source);
		if (modifier_value == null)
		{
			AddChild(value);
		}
		else
		{
			modifier_value.flat_value = value.flat_value;
			modifier_value.percent_value = value.percent_value;
		}


	}

	public void RemoveValue(string source)
	{
		foreach (ModifierValue value in GetChildren())
		{
			if (value.source == source)
			{
				value.QueueFree();
			}
		}
	}

	public void ClearValues()
	{
		foreach (ModifierValue value in GetChildren())
		{
			value.QueueFree();
		}
	}

	//计算逻辑，根据修饰符获取值
	public int GetModifiedValue(int @base)
	{
		var flat_result = @base;
		var percent_result = 1.0f;
		//应用修饰符
		foreach (ModifierValue value in GetChildren())
		{
			if (value.type == ModifierValue.Type.FLAT)
			{
				flat_result += value.flat_value;
			}

		}

		foreach (ModifierValue value in GetChildren())
		{
			if (value.type == ModifierValue.Type.PERCENT_BASED)
			{
				percent_result += value.percent_value;
			}
		}
		//返回最终结果值
		return (int)Math.Floor(flat_result * percent_result);
	} 
	
}
