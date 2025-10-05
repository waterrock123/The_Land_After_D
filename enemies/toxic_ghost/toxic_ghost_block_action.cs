using Godot;
using System;
using System.Security.Cryptography.X509Certificates;
//敌人行动逻辑
//
public partial class toxic_ghost_block_action : EnemyAction
{

	[Export]
	public int block = 10;
	//行动
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
		GetTree().CreateTimer(0.6f, false).Timeout += () => { events.instance.EmitSignal(events.SignalName.EnemyActionCompleted, enemy); };

	}

	
}

