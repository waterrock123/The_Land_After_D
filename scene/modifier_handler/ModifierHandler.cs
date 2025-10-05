using Godot;
using System;

public partial class ModifierHandler : Node
{

	public override void _Ready()
	{
	}
	//判断修饰符节点是否存在
	public bool HasModifier(Modifier.Type type)
	{
		foreach (Modifier modifier in GetChildren())
		{
			if (modifier.type == type)
			{
				return true;
			}

		}
		return false;
	}
	//根据类型返回修饰符节点
	public Modifier GetModifier(Modifier.Type type)
	{
		foreach (Modifier modifier in GetChildren())
		{
			if (modifier.type == type)
			{
				return modifier;
			}
		}
		return null;
	}
	//获取修饰符修饰后的值
	public int GetModifiedValue(int @base, Modifier.Type type)
	{
		var modifier = GetModifier(type);
		if (modifier == null)
		{
			return @base;
		}
		return modifier.GetModifiedValue(@base);
	}
	
	
}
