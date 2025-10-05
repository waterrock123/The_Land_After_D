using Godot;
using Godot.Collections;
using System;

public partial class Treasure : Control
{
	[Export]
	public Array<Relic> treasure_relic_pool;
	[Export]
	public RelicHandler relic_handler;
	[Export]
	public CharaStats char_stats;


	public AnimationPlayer animation_player;
	public Relic found_relic;

	public override void _Ready()
	{
		animation_player = GetNode<AnimationPlayer>("%AnimationPlayer");

	}

	public void GenerateRelic()
	{
		var availablie_relics = new Array<Relic>();
		foreach (Relic relic in treasure_relic_pool)
		{
			var can_appear = relic.CanApperAsReward(char_stats);
			var already_had_it = relic_handler.HasRelic(relic.id);
			if (can_appear && !already_had_it)
			{
				availablie_relics.Add(relic);
			}
		}
		if (availablie_relics.Count == 0)
		{
			return;
		}
		found_relic = (Relic)rng.ArrayPickRandomRelic(availablie_relics);
	}

	public void OnTreasureChestGuiInput(InputEvent @event)
	{
		if (animation_player.CurrentAnimation == "open")
		{
			return;
		}
		if (@event.IsActionPressed("鼠标左键"))
		{
			animation_player.Play("open");
		}



	}
	//开宝箱动画结束时调用
	public void OnTreasureOpened()
	{
		events.instance.EmitSignal(events.SignalName.TreasureRoomExited, found_relic);
	}

}
