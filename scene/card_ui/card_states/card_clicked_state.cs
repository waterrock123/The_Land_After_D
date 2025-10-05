using Godot;
using System;

public partial class card_clicked_state : card_state
{
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
		Area2D drop_point_detector = cardUi.droppointdetec;
		drop_point_detector.Monitoring = true;
		cardUi.OriginalIndex = cardUi.GetIndex();

	}

	public override void OnInput(InputEvent @event)
	{
		
		if (@event is InputEventMouseMotion)
		{
			EmitSignal(SignalName.TransitionRequested, this, (long)(int)card_state.State.DRAGGING);
		}
    }

}
