using Godot;
using System;


public partial class card_aiming_state : card_state
{
	const int mouse_y_snapback_whreshold = 684;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
    public override void _EnterTree()
    {
        base._EnterTree();
    }   
	public override void Enter()
	{
		cardUi.targets.Clear();
		var offset = new Vector2(cardUi.parent.Size.X / 2, -cardUi.Size.Y / 2);
		offset.X -= cardUi.Size.X / 2;
		// cardUi.animate_to_position(cardUi.parent.GlobalPosition + offset, 0.2f);
		cardUi.droppointdetec.Monitoring = false;
		middlevent.EmitSignal(events.SignalName.CardAimStarted, cardUi);
	}
    public override void Exit()
    {
        middlevent.EmitSignal(events.SignalName.CardAimEnded, cardUi);
    }
	public override void OnInput(InputEvent @event)
	{
		bool mouse_motion = @event is InputEventMouse;
		// bool mouse_at_bottom = cardUi.GetGlobalMousePosition().Y > mouse_y_snapback_whreshold;
		// (mouse_motion && mouse_at_bottom) || 
		if (@event.IsActionPressed("鼠标右键"))
		{
			cardUi.targets.Clear();
			events.instance.EmitSignal(events.SignalName.CardSlotOut, cardUi);
			EmitSignal(SignalName.TransitionRequested, this, (long)(int)card_state.State.BASE);
		}
		else if (@event.IsActionPressed("鼠标左键") || @event.IsActionReleased("鼠标左键"))
		{
			GetViewport().SetInputAsHandled();
			EmitSignal(SignalName.TransitionRequested, this, (long)(int)card_state.State.INSLOT);
		}
    }


}

