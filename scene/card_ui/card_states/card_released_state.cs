using Godot;
using System;

public partial class card_released_state : card_state
{

	bool played;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public override void Enter()
	{
		played = false;
		if (cardUi.targets.Count > 0)
		{
			events.instance.EmitSignal(events.SignalName.TooltipHideRequested);
			played = true;

			GD.Print("play card for target(s)", cardUi.targets);
			cardUi.play();
		}
		
    }
    public override void PostEntter()
    {
        EmitSignal(SignalName.TransitionRequested, this, (long)(int)card_state.State.BASE);
    }

	
	
		
    

}
