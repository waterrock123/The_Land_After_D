using Godot;
using System;

public partial class GoldUi : HBoxContainer
{
	public run_stats RunStats;
	[Export]
	public run_stats importrun_stats
	{
		get => RunStats;
		set => SetRunStats(value);
	}
	public Label label;


	public override void _Ready()
	{
		label = GetNode<Label>("Label");
		label.Text = '0'.ToString();
	}
	public void SetRunStats(run_stats new_value)
	{
		RunStats = new_value;
		RunStats.GoldChanged -= UpdateGold;
		RunStats.GoldChanged += UpdateGold;
		UpdateGold();
	}
	public void UpdateGold()
	{
		label.Text = RunStats.gold.ToString();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
