using Godot;
using System;

public partial class card_dragging_state : card_state
{


	double drag_minimum_threshold = 0.05;
	bool minimum_drag_time_elapsed = false;
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
		var ui_layer = GetTree().GetFirstNodeInGroup("ui_layer");
		if (ui_layer != null)
		{
			cardUi.Reparent(ui_layer);

		}
		cardUi.cardvisuals.panel.AddThemeStyleboxOverride("panel", cardUi.DRAG_STYLEBOX);
		events.instance.EmitSignal(events.SignalName.CardDragStarted, cardUi);

		minimum_drag_time_elapsed = false;
		var thresholdTimer = GetTree().CreateTimer(drag_minimum_threshold, false);

		// 连接信号（用 lambda 写匿名函数）
		thresholdTimer.Timeout += () => {
    		minimum_drag_time_elapsed = true;
		};

	}
	public override void Exit()
	{
		events.instance.EmitSignal(events.SignalName.CardDragEnded, cardUi);
	}
	public override void OnInput(InputEvent @event)
	{
		var single_targeted = cardUi.card.IsSingleTargeted();
		var mouse_motion = @event is InputEventMouseMotion;
		var cancel = @event.IsActionPressed("鼠标右键");
		var confirm = @event.IsActionReleased("鼠标左键") || @event.IsActionPressed("鼠标左键");
		GD.Print($"[Debug] single_targeted={single_targeted}, mouse_motion={mouse_motion}, targets={cardUi.targets.Count}, CanDrop={cardUi.CanDrop}, confirm={confirm}");

		if (single_targeted && cardUi.targets.Count > 0 && cardUi.CanDrop && confirm)
		{
			GD.Print("进瞄准状态");
			events.instance.EmitSignal(events.SignalName.CardSlotRequested);
			EmitSignal(SignalName.TransitionRequested, this, (long)card_state.State.AIMING);
			return;
		}

		if (mouse_motion)
		{
			cardUi.GlobalPosition = cardUi.GetGlobalMousePosition() - cardUi.PivotOffset;//卡牌跟随鼠标
		}
		if (cancel)
		{
			EmitSignal(SignalName.TransitionRequested, this, (long)(int)card_state.State.BASE);

		}
		else if (confirm && cardUi.CanDrop)//minimum_drag_time_elapsed不知道干啥的，先放起来
		{
			GD.Print("进入牌阵储存状态");
			events.instance.EmitSignal(events.SignalName.CardSlotRequested);
			// GetViewport().SetInputAsHandled();
			EmitSignal(SignalName.TransitionRequested, this, (long)(int)card_state.State.INSLOT);
			return;
		}
		

	}

}
