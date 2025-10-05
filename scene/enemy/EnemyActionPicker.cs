using Godot;
using System;

public partial class EnemyActionPicker : Node
{
	public float total_weight;
	public Enemy enemy;
	[Export]
	public Enemy importenemy
	{
		get => enemy;
		set
		{
			SetEnemy(value);
		}
	}
	public Node2D target;
	[Export]
	public Node2D importtarget
	{
		get => target;
		set
		{
			SetTarget(value);
		}
	}

	public override void _Ready()
	{
		total_weight = 0.0f;
		importtarget = (Node2D)GetTree().GetFirstNodeInGroup("player");
		SetupChances();
	}
	public EnemyAction GetAction()
	{
		var action = GetFirstConditionalACtion();
		if (action != null)
		{
			return action;
		}
		return GetChanceBasedAction();
	}
	public EnemyAction GetFirstConditionalACtion()
	{
		EnemyAction action = null;

    	foreach (Node child in GetChildren())
    	{
        // 尝试把 child 转成 EnemyAction
        	action = child as EnemyAction;

        // 如果 child 不是 EnemyAction，或者它的 type 不是 CONDITIONAL，就跳过
        	if (action == null || action.type != EnemyAction.Type.CONDITIONAL)
        	{
            	continue;
        	}

        // 如果 action 符合条件（is_performable 返回 true），直接返回这个 action
        	if (action.IsPerformable())
        	{
            	return action;
        	}
    	}

    // 如果循环结束都没有找到合适的 action，就返回 null
    	return null;
	}
	public EnemyAction GetChanceBasedAction()
	{
		EnemyAction action = null;
		var roll = rng.instance.RandfRange(0.0f, total_weight);

		foreach (Node child in GetChildren())
		{
			action = child as EnemyAction;
			if (action == null || action.type != EnemyAction.Type.CHANCE_BASED)
			{
				continue;
			}
			if (action.accumulated_weight > roll)
			{
				return action;
			}
		}
		return null;
	}
	public void SetupChances()
	{
		EnemyAction action = null;
		foreach (Node child in GetChildren())
		{
			action = child as EnemyAction;
			if (action == null || action.type != EnemyAction.Type.CHANCE_BASED)
			{
				continue;
			}
			total_weight += (float)action.chance_weight;
			action.accumulated_weight = total_weight;
		}
	}
	public void SetEnemy(Enemy value)
	{
		enemy = value;
		foreach (EnemyAction action in GetChildren())
		{
			action.enemy = enemy;

		}
	}
	public void SetTarget(Node2D value)
	{
		target = value;
		foreach (EnemyAction action in GetChildren())
		{
			action.target = target;
		}
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
