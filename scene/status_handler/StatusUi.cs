using Godot;
using System;
using System.Threading.Tasks;

public partial class StatusUi : Control
{
	public status status;
	[Export]
	public status importStatus
	{
		get => status;
		set => SetStatus(value);
	}

	public TextureRect icon;
	public Label duration;
	public Label stacks;

	public override void _Ready()
	{
		icon = GetNode<TextureRect>("Icon");
		duration = GetNode<Label>("Duration");
		stacks = GetNode<Label>("Stacks");
		



	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public async Task SetStatus(status new_status)
	{
		if (!IsNodeReady())
		{
			await ToSignal(this, "ready");
		}

		status = new_status;
		icon.Texture = status.icon;
		duration.Visible = status.stack_type == status.StackType.DURATION;
		stacks.Visible = status.stack_type == status.StackType.INTENSITY;
		CustomMinimumSize = icon.Size;

		if (duration.Visible)
		{
			CustomMinimumSize = duration.Size + duration.Position;
		}
		else if (stacks.Visible)
		{
			CustomMinimumSize = stacks.Size + stacks.Position;
		}

		status.StatusChanged -= OnStatusChanged;
		status.StatusChanged += OnStatusChanged;

		OnStatusChanged();


	}

	public void OnStatusChanged()
	{
		if (status == null)
		{
			return;
		}
		if (status.can_expire && status.duration <= 0)
		{
			QueueFree();
		}
		if (status.stack_type == status.StackType.INTENSITY && status.stacks == 0)
		{
			QueueFree();
		}
		duration.Text = status.duration.ToString();
		stacks.Text = status.stacks.ToString();



	}


}
