using Godot;
using Godot.Collections;
using System;


public partial class shaker : Node
{


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}
	
	
	public void shake(Node2D thing, float strenth, float duration = 0.2f)
	{
		if (thing == null)
		{
			return;
		}
		var orig_pos = thing.Position;//原始位置
		var shake_count = 10;//持续时间内的抖动次数
		var tween = CreateTween();
		tween.BindNode(thing);
		for (int i = 0; i < shake_count; i++)
		{
			var shake_offset = new Vector2((float)GD.RandRange(-1.0, 1.0), (float)GD.RandRange(-1.0, 1.0));
			var target = orig_pos + (strenth * shake_offset);
			if (i % 2 == 0)
			{
				target = orig_pos;
			}
			tween.TweenProperty(thing, "position", target, duration / shake_count);
			strenth *= 0.75f;

		}
		tween.Finished += () =>
		{
			thing.Position = orig_pos;
		};



	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.

}
