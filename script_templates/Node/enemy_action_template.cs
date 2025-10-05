using Godot;
using System;
//敌人行动逻辑
//
public partial class enemy_action_template : EnemyAction
{
	//行动
	public override void PerformAction()
	{
		if (enemy == null || target == null)
		{
			return;
		}
		Tween tween = CreateTween().SetTrans(Tween.TransitionType.Quint);
		var start = enemy.GlobalPosition;
		var end = target.GlobalPosition + Vector2.Right * 32;

		sfxsound_player.instance.play(sound);
		events.instance.EmitSignal(events.SignalName.EnemyActionCompleted, enemy);
	}

	//可以在此自定义更新意图表示
	//对于攻击意图，可以用DMG TAKEN修饰器修饰伤害数字做到动态UI
	public override void UpdateIntentText()
	{
		var player = target as player;
		if (player == null)
		{
			return;
		}
		var modified_dmg = player.modifier_handler.GetModifiedValue(6, Modifier.Type.DMG_TAKEN);//指玩家受到的伤害也就是怪物的攻击伤害

		Intent.current_text = string.Format(Intent.base_text,modified_dmg);
	}
}
