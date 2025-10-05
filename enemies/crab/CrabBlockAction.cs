using Godot;
using System;

public partial class CrabBlockAction : EnemyAction
{
	[Export]
	int block = 6;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public override void PerformAction()
	{
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
