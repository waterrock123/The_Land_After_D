using Godot;
using Godot.Collections;
using System;

public partial class StatusView : Control
{
	public static PackedScene STATUS_TOOLTIP = (PackedScene)GD.Load("res://scene/ui/status_tooltip.tscn");

	public VBoxContainer status_tooltips;
	public override void _Ready()
	{
		status_tooltips = GetNode<VBoxContainer>("%StatusTooltips");

		foreach (StatusTooltip tooltip in status_tooltips.GetChildren())
		{
			tooltip.QueueFree();
		}
		events.instance.StatusTooltipRequested += ShowView;


	}

    public override void _Input(InputEvent @event)
    {
		if (@event.IsActionPressed("ui_cancel") && Visible)
		{
			HideView();
		}
    }

	public void ShowView(Array<status> statuses)
	{
		foreach (status status in statuses)
		{
			var new_status_tooltip = STATUS_TOOLTIP.Instantiate() as StatusTooltip;
			status_tooltips.AddChild(new_status_tooltip);
			new_status_tooltip.importstatus = status;

		}
		Show();
	}
	public void HideView()
	{
		foreach (StatusTooltip tooltip in status_tooltips.GetChildren())
		{
			tooltip.QueueFree();
		}
		Hide(); 
	}

	public void OnGuiInput(InputEvent @event)
	{
		if (@event.IsActionPressed("鼠标左键") && Visible)
		{
			HideView();
		}
	}

	public void OnBackbuttonPressed()
	{
		HideView();
	}
}
