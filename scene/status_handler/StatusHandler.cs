using Godot;
using Godot.Collections;
using System;
using System.Linq;

public partial class StatusHandler : GridContainer
{

	public static PackedScene STATUS_UI = (PackedScene)GD.Load("res://scene/status_handler/status_ui.tscn");
	[Signal]
	public delegate void StatusesAppliedEventHandler(status.Type type);
	public const double STATUS_APPLY_INTERVAL = 0.25;

	[Export]
	public Node2D status_owner;

	public void ApplyStatusesByType(status.Type type)
	{
		if (type == status.Type.EVENT_BASED)
		{
			return;
		}
		var status_queue = GetAllStatuses().Where(status => status.type == type)
		.ToArray();

		if (status_queue.Count() == 0)
		{
			EmitSignal(SignalName.StatusesApplied, Variant.From(type));
			return;
		}
		var tween = CreateTween();
		foreach (status Status in status_queue)
		{
			tween.TweenCallback(Callable.From(() => Status.apply_status(status_owner)));
			tween.TweenInterval(STATUS_APPLY_INTERVAL);

		}
		tween.Finished += () => { EmitSignal(SignalName.StatusesApplied, Variant.From(type)); };
		




	}
	public override void _Ready()
	{


	}
	

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void AddStatus(status Status)
	{
		var stackable = Status.stack_type != status.StackType.NONE;
		//检查是否已有此状态
		if (!HasStatus(Status.id))
		{
			var new_status_ui = STATUS_UI.Instantiate() as StatusUi;
			AddChild(new_status_ui);
			new_status_ui.importStatus = Status;
			new_status_ui.status.StatusApplied += OnStatusApplied;
			new_status_ui.status.initialize_status(status_owner);
			return;
		}

		//已有,判断是否可堆叠及过期
		if (!Status.can_expire && !stackable)
		{
			return;
		}
		//持续时间堆叠
		if (Status.can_expire && Status.stack_type == status.StackType.DURATION)
		{
			GetStatus(Status.id).importduration += Status.duration;
		}
		//效果堆叠
		if (Status.stack_type == status.StackType.INTENSITY)
		{
			GetStatus(Status.id).importstacks += Status.stacks;
		}

	}

	public bool HasStatus(string id)
	{
		foreach (StatusUi status_ui in GetChildren())
		{
			if (status_ui.status.id == id)
			{
				return true;
			}
		}
		return false;
	}


	public status GetStatus(string id)
	{
		foreach (StatusUi status_ui in GetChildren())
		{
			if (status_ui.status.id == id)
			{
				return status_ui.status;
			}
		}
		return null;
	}
	public Array<status> GetAllStatuses()
	{
		Array<status> statuses = new Array<status>();
		foreach (StatusUi status_ui in GetChildren())
		{
			statuses.Add(status_ui.status);
		}

		return statuses;
	}

	public void OnStatusApplied(status Status)
	{
		if (Status.can_expire)
		{
			Status.importduration -= 1;
		}
	}

	public void OnGuiInput(InputEvent @event)
	{
		if (@event.IsActionPressed("鼠标左键"))
		{
			events.instance.EmitSignal(events.SignalName.StatusTooltipRequested, GetAllStatuses());

		}
		
	}

}
