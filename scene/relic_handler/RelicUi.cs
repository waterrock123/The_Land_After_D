using Godot;
using System;
using System.Threading.Tasks;

public partial class RelicUi : Control
{

	public Relic relic;
	[Export]
	public Relic importrelic
	{
		get => relic;
		set => SetRelic(value);
	}

	public TextureRect icon;
	public AnimationPlayer animation_player;

	public override void _Ready()
	{
		icon = GetNode<TextureRect>("Icon");
		animation_player = GetNode<AnimationPlayer>("AnimationPlayer");

		
	}

	public async Task SetRelic(Relic new_relic)
	{
		if (!IsNodeReady())
		{
			await ToSignal(this, "ready");
		}
		relic = new_relic;
		icon.Texture = (Texture2D)relic.icon;
	}

	public void Flash()
	{
		animation_player.Play("flash");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnGuiInput(InputEvent @event)
	{
		if (@event.IsActionPressed("鼠标左键"))
		{
			events.instance.EmitSignal(events.SignalName.RelicTooltipRequested, relic);
		}
		
	}
}
