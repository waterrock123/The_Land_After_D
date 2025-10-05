using Godot;
using System;

public partial class CrabMegaBlockAction : EnemyAction
{
	[Export]
	int block = 15;
	[Export]
	int hp_threshold = 6;//生命值阈值，低于阈值将行动变为超级护甲
	public bool already_used = false;//标志此行动是否使用过
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override bool IsPerformable()
	{
		if (enemy == null || already_used)
		{
			return false;
		}
		var is_low = enemy.stats.Health <= hp_threshold;
		already_used = is_low;
		return is_low;
    }

	public override void PerformAction()
	{
		if (enemy == null || target == null)
		{
			return;
		}
		var block_effect = new block_effect();
		block_effect.amount = block;
		block_effect.sound = sound;
		block_effect.execute([enemy]);
		var timer = GetTree().CreateTimer(0.6f);
		timer.Timeout += () =>
		{
			events.instance.EmitSignal(events.SignalName.EnemyActionCompleted, enemy);
		};
	}
}
