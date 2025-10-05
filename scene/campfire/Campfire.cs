using Godot;
using System;

public partial class Campfire : Control
{

	[Export]
	public CharaStats char_stats;

	public AnimationPlayer animation_player;

	public Button rest_button;
	public override void _Ready()
	{
		animation_player = GetNode<AnimationPlayer>("AnimationPlayer");
		rest_button = GetNode<Button>("%RestButton");

	}

	public void OnRestButtonPressed()
	{
		rest_button.Disabled = true;
		char_stats.Heal((int)Math.Ceiling(char_stats.MaxHealth * 0.3));
		animation_player.Play("fade_out");
	}

	//这是一个来自动画的调用方法
	public void OnFadeOutFinished()
	{
		events.instance.EmitSignal(events.SignalName.CampfireExited);
	}

}
