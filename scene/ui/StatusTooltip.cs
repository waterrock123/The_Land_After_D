using Godot;
using System;
using System.Threading.Tasks;

public partial class StatusTooltip : HBoxContainer
{

	public status status;
	[Export]
	public status importstatus
	{
		get => status;
		set => SetStatus(value);
	}

	public TextureRect icon;
	public Label label;


	public override void _Ready()
	{
		icon = GetNode<TextureRect>("Icon");
		label = GetNode<Label>("Label");


	}

	public async Task SetStatus(status new_status)
	{
		if (!IsNodeReady())
		{
			await ToSignal(this, "ready");
		}
		status = new_status;
		icon.Texture = status.icon;
		label.Text = status.get_tooltip();
	}

	
}
