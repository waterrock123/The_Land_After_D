using Godot;
using Godot.Collections;
using System;
using System.Security.Cryptography.X509Certificates;
//敌人行动逻辑
//
public partial class toxic_ghost_attack_action : EnemyAction
{
	public Card Toxin = GD.Load<Card>("res://common_cards/toxin.tres");
	[Export]
	public int damage = 10;
	//行动
	public override void PerformAction()
	{
		if (enemy == null || target == null)
		{
			return;
		}
		var player = target as player;
		if (player == null)
		{
			return;
		}
		Tween tween = CreateTween().SetTrans(Tween.TransitionType.Quint);
		var start = enemy.GlobalPosition;
		var end = target.GlobalPosition + Vector2.Right * 32;
		var damage_effect = new damage_effect();
		Array<Node> target_array = [target];
		var modified_dmg = enemy.modifier_handler.GetModifiedValue(damage, Modifier.Type.DMG_DEALT);

		damage_effect.amount = modified_dmg;
		damage_effect.sound = sound;

		tween.TweenProperty(enemy, "global_position", end, 0.4);
		tween.TweenCallback(Callable.From(() => damage_effect.execute(target_array)));
		tween.TweenCallback(Callable.From(() => player.stats.draw_pile.add_card((Card)Toxin.Duplicate())));
		tween.TweenInterval(0.25);
		tween.TweenProperty(enemy, "global_position", start, 0.4);


		tween.Finished += () => { events.instance.EmitSignal(events.SignalName.EnemyActionCompleted, enemy); };
	}


	public override void UpdateIntentText()
	{
		var player = target as player;
		if (player == null)
		{
			return;
		}
		var base_dmg = enemy.modifier_handler.GetModifiedValue(damage, Modifier.Type.DMG_DEALT);
		var modified_dmg = player.modifier_handler.GetModifiedValue(base_dmg, Modifier.Type.DMG_TAKEN);//指玩家受到的伤害也就是怪物的攻击伤害

		Intent.current_text = string.Format(Intent.base_text,modified_dmg);
	}
}

