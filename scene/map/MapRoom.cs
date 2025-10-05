using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class MapRoom : Area2D
{
	[Signal]
	public delegate void ClickedEventHandler(Room room);
	[Signal]
	public delegate void SelectedEventHandler(Room room);

	public static Dictionary<Room.Type, (Texture2D, Vector2)> ICONS =
		new Dictionary<Room.Type, (Texture2D, Vector2)>
		{
			{ Room.Type.NOT_ASSIGNED, (null, Vector2.One) },
			{ Room.Type.MONSTER, (GD.Load<Texture2D>("res://art/tile_0103.png"), Vector2.One) },
			{ Room.Type.TREASURE, (GD.Load<Texture2D>("res://art/tile_0089.png"), Vector2.One) },
			{ Room.Type.CAMPFIRE, (GD.Load<Texture2D>("res://art/player_heart.png"), new Vector2(0.6f, 0.6f)) },
			{ Room.Type.SHOP, (GD.Load<Texture2D>("res://art/gold.png"), new Vector2(0.6f, 0.6f)) },
			{ Room.Type.BOSS, (GD.Load<Texture2D>("res://art/tile_0105.png"), new Vector2(1.25f, 1.25f)) },
			{Room.Type.Event,(GD.Load<Texture2D>("res://art/rarity.png"), new Vector2(0.9f, 0.9f))}
		};



	public Sprite2D sprite_2d;
	public Line2D line_2d;
	public AnimationPlayer animation_player;

	public bool available = false;
	public bool importavailable
	{
		get => available;
		set => SetAvailable(value);
	}
	public Room room;
	public Room importroom
	{
		get => room;
		set => SetRoom(value);
	}
	public void SetAvailable(bool new_value)
	{
		available = new_value;
		if (available)
		{
			animation_player.Play("highlight");
		}
		else if (!room.selected)
		{
			animation_player.Play("RESET");
		}
	}

	public void SetRoom(Room new_data)
	{
		room = new_data;
		Position = room.position;
		line_2d.RotationDegrees = GD.RandRange(0, 360);
		sprite_2d.Texture = ICONS[room.type].Item1;
		sprite_2d.Scale = ICONS[room.type].Item2;



	}
	public override void _Ready()
	{
		sprite_2d = GetNode<Sprite2D>("Visuals/Sprite2D");
		line_2d = GetNode<Line2D>("Visuals/Line2D");
		animation_player = GetNode<AnimationPlayer>("AnimationPlayer");

		
	}
	//显示选中
	public void ShowSelected()
	{
		line_2d.Modulate = Colors.White;
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public void OnInputEvent(Node viewport, InputEvent @event, int shape_id)
	{
		if (!available | !@event.IsActionPressed("鼠标左键"))
		{
			return;
		}
		room.selected = true;
		EmitSignal(SignalName.Clicked, room);
		animation_player.Play("select");

	}
	//选择动画完成后发出选中信号,动画播放器节点会调用这个
	public void OnMapRoomSelected()
	{
		EmitSignal(SignalName.Selected, room);
	}
}
