using Godot;
using System;
using System.Security.Cryptography.X509Certificates;

public partial class Stats_ui : HBoxContainer
{
	public HealthUi health;
	public HBoxContainer block;
	public Label block_label;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		block = GetNode<HBoxContainer>("Block");
		
		health = GetNode<HealthUi>("Health");
		
		block_label = GetNode<Label>("%BlockLabel");
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	//更新数据方法
	public void update_stats(Stats stats)
	{
		block_label.Text = stats.Block.ToString();
		health.UpdateStats(stats);
		block.Visible = stats.Block > 0;
		health.Visible = stats.Health > 0;
	}

}
