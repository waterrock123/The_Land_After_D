using Godot;
using System;
//敌人行动逻辑
//
public partial class toxic_ghost_muscle_buff_action : EnemyAction
{
	public static status MUSCLE_STATUS = GD.Load<status>("res://statuses/muscle.tres");
	[Export]
	public int stacks_per_action = 2;

	public int hp_threshold = 25;
	public int usages = 0;

	public override bool IsPerformable()
	{
		var hp_under_threshold = enemy.stats.Health <= hp_threshold;
		if (usages == 0 || (usages == 1 && hp_under_threshold))
		{
			usages += 1;
			return true;
		}
		return false;
    }



	//行动
	public override void PerformAction()
	{
		if (enemy == null || target == null)
		{
			return;
		}

		var status_effect = new StatusEffect();
		var muscle = MUSCLE_STATUS.Duplicate() as status;
		muscle.stacks = stacks_per_action;
		status_effect.Status = muscle;
		status_effect.execute([enemy]);


		sfxsound_player.instance.play(sound);
		events.instance.EmitSignal(events.SignalName.EnemyActionCompleted, enemy);
	}


}

