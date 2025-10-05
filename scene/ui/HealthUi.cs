using Godot;
using System;

public partial class HealthUi : HBoxContainer
{
	[Export]
	public bool show_max_hp;

	public Label health_label;
	public Label max_health_label;

	public override void _Ready()
	{
		health_label = GetNode<Label>("HealthLabel");
		max_health_label = GetNode<Label>("MaxHealthLabel");

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void UpdateStats(Stats stats)
	{
		health_label.Text = stats.Health.ToString();
		max_health_label.Text = $"/{stats.MaxHealth.ToString()}";
		max_health_label.Visible = show_max_hp;
		
	}



}
