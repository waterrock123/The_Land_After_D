using Godot;
using System;
//敌人行动逻辑
//

public partial class BatBlockAction : EnemyAction
{
	[Export]
	int block = 4;

	// Called when the node enters the scene tree for the first time.
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

