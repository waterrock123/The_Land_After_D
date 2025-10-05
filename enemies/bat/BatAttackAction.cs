using Godot;
using System;
//敌人行动逻辑
//

public partial class BatAttackAction : EnemyAction
{
	[Export]
	int damage = 4;

	// Called when the node enters the scene tree for the first time.
	public override void PerformAction()
	{
		if (enemy == null || target == null)
		{
			return;
		}
		Tween tween = CreateTween().SetTrans(Tween.TransitionType.Quint);
		var start = enemy.GlobalPosition;
		var end = target.GlobalPosition + Vector2.Right * 32;
		var damage_effect = new damage_effect();
		var target_array = new Godot.Collections.Array<Node> { target };//创建目标组
		damage_effect.amount = damage;
		damage_effect.sound = sound;
		tween.TweenProperty(enemy, "global_position", end, 0.4);
		tween.TweenCallback(Callable.From(() => damage_effect.execute(target_array)));
		tween.TweenInterval(0.35);
		tween.TweenCallback(Callable.From(() => damage_effect.execute(target_array)));
		tween.TweenInterval(0.25);
		tween.TweenProperty(enemy, "global_position", start, 0.4);

		tween.Finished += () =>
		{
			events.instance.EmitSignal(events.SignalName.EnemyActionCompleted, enemy);
		};
	}

	public override void UpdateIntentText()
	{
		var player = target as player;
		if (player == null)
		{
			return;
		}
		var modified_dmg = player.modifier_handler.GetModifiedValue(damage, Modifier.Type.DMG_TAKEN);//指玩家受到的伤害也就是怪物的攻击伤害

		Intent.current_text = string.Format(Intent.base_text,modified_dmg);
	}

}

