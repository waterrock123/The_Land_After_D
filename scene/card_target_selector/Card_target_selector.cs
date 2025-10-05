using Godot;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

public partial class Card_target_selector : Node2D
{
	const int arc_points = 8;
	Area2D area_2d;
	Line2D card_arc;
	CardUi current_card;
	bool targeting = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		area_2d = GetNode<Area2D>("Area2D");
		card_arc = GetNode<Line2D>("CanvasLayer/CardArc");
		var eventsNode = GetNode("/root/Events");

		// 连接信号 card_aim_started 到本类的回调方法
		eventsNode.Connect(events.SignalName.CardAimStarted, new Callable(this, nameof(OnCardAimStarted)));
		eventsNode.Connect(events.SignalName.CardAimEnded, new Callable(this, nameof(OnCardAimEnded)));
		

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!targeting)
		{
			return;
		}
		area_2d.Position = ToLocal(GetGlobalMousePosition());
		card_arc.Points = _get_points();
	}
	public Vector2[] _get_points()
	{
		var points = new List<Vector2>();

		// 起点（卡片中心）
		var start = current_card.GlobalPosition;
		start.X += current_card.Get("size").AsVector2().X / 2f;

		// 鼠标目标点（相对当前节点坐标系）
		var target = GetGlobalMousePosition();
		var distance = target - start;

		for (int i = 0; i < arc_points; i++)
		{
			float t = (1.0f / arc_points) * i;
			float x = start.X + (distance.X / arc_points) * i;
			float y = start.Y + EaseOutCubic(t) * distance.Y;
			points.Add(new Vector2(x, y));
		}

		points.Add(target);

		return points.ToArray();
	}

	public float EaseOutCubic(float number)
	{
		return 1.0f - (float)Math.Pow(1.0f - number, 3.0f);
	}
	public void OnCardAimStarted(CardUi card)
	{
		if (!card.card.IsSingleTargeted())
		{
			return;
		}
		
		targeting = true;
		area_2d.Monitoring = true;
		area_2d.Monitorable = true;
		current_card = card;

	}
	public void OnCardAimEnded(CardUi card)
	{
		targeting = false;
		card_arc.ClearPoints();
		area_2d.Position = Vector2.Zero;
		area_2d.Monitoring = false;
		area_2d.Monitorable = false;
		current_card = null;
	}

	public void OnArea2dAreaEntered(Area2D area)
	{
		if (current_card == null || !targeting)
		{
			return;
		}
		if (!current_card.targets.Contains(area))
		{

			current_card.targets.Add(area);
			current_card.RequestTooltip();
		}

	}
	public void OnArea2dAreaExited(Area2D area)
	{
		if (current_card == null || !targeting)
		{
			return;
		}
		current_card.targets.Remove(area);
		current_card.RequestTooltip();
	}
}
