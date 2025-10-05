using Godot;
using System;

public partial class true_strength_form : status
{
	public status MUSCLE_STATUS = (status)GD.Load("res://statuses/muscle.tres");

	public int stacks_per_turn = 2;
	
	public override void apply_status(Node target)
	{
		var status_effect = new StatusEffect();
		var muscle = (status)MUSCLE_STATUS.Duplicate();
		muscle.stacks = stacks_per_turn;
		status_effect.Status = muscle;
		status_effect.execute([target]);


		EmitSignal(SignalName.StatusApplied, this);


	}
}

