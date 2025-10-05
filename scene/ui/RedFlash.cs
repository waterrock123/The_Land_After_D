using Godot;
using System;

public partial class RedFlash : CanvasLayer
{
	public ColorRect color_rect;
	public Timer timer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		color_rect = GetNode<ColorRect>("ColorRect");
		timer = GetNode<Timer>("Timer");
		events.instance.PlayerHit += OnPlayerHit;
		timer.Timeout += OnTimerTimeout;
	}
	public override void _ExitTree()
    {
        // 解绑，防止重载/切换时回调旧实例
        events.instance.PlayerHit -= OnPlayerHit;
        timer.Timeout -= OnTimerTimeout;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void OnPlayerHit()
	{
		var c = color_rect.Color;
		c.A = 0.2f;
		color_rect.Color = c;
		timer.Start();

	}
	public void OnTimerTimeout()
	{
		var c = color_rect.Color;
		c.A = 0;
		color_rect.Color = c;
	}
}
