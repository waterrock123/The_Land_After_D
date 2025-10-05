using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections; // Godot Array 和 Dictionary
using System.Text.Json;  // 使用 C# 内置 JSON 序列化
public partial class Map : Node2D
{
	private bool isDragging = false;
	private Vector2 lastMousePos;
	public static int SCROLL_SPEED = 30;
	public static PackedScene MAP_ROOM = GD.Load<PackedScene>("res://scene/map/map_room.tscn");
	public static PackedScene MAP_LINE = GD.Load<PackedScene>("res://scene/map/map_line.tscn");
	//节点
	public MapGenerator map_generator;
	public Node2D lines;
	public Node2D rooms;
	public Node2D visuals;
	public Camera2D camera_2d;
	//地图数据变量
	public Godot.Collections.Array<Godot.Collections.Array<Room>> map_data;
	public int floors_climbed;
	public Room last_room;//上一个房间
	public float camera_edge_y;//相机边缘，地图坐标结束在哪

	public override void _Ready()
	{
		map_generator = GetNode<MapGenerator>("MapGenerator");
		lines = GetNode<Node2D>("%Lines");
		rooms = GetNode<Node2D>("%Rooms");
		visuals = GetNode<Node2D>("Visuals");
		camera_2d = GetNode<Camera2D>("Camera2D");

		camera_edge_y = MapGenerator.Y_DIST * (MapGenerator.FLOORS - 1);
	}
	//可以鼠标滚轮上下滚动
	public override void _UnhandledInput(InputEvent @event)
	{
		if (!Visible)
		{
			return;
		}
		// 鼠标滚轮滚动
		if (@event.IsActionPressed("scroll_up"))
		{
			camera_2d.Position -= new Vector2(0, SCROLL_SPEED);
		}
		else if (@event.IsActionPressed("scroll_down"))
		{
			camera_2d.Position += new Vector2(0, SCROLL_SPEED);
		}

		// 鼠标左键按下 -> 开始拖拽
		if (@event is InputEventMouseButton mouseButton)
		{
			if (mouseButton.ButtonIndex == MouseButton.Left)
			{
				if (mouseButton.Pressed)
				{
					isDragging = true;
					lastMousePos = mouseButton.Position;
				}
				else
				{
					isDragging = false;
				}
			}
		}

		// 鼠标移动 -> 拖拽时更新相机位置
		if (@event is InputEventMouseMotion mouseMotion && isDragging)
		{
			float deltaY = mouseMotion.Position.Y - lastMousePos.Y;
			camera_2d.Position -= new Vector2(0, deltaY); // 相机跟随鼠标移动
			lastMousePos = mouseMotion.Position;
		}

		// 限制相机范围
		camera_2d.Position = new Vector2(
			camera_2d.Position.X,
			Mathf.Clamp(camera_2d.Position.Y, -camera_edge_y, 0)
		);

	}

	public void GenerateNewMap()
	{
		floors_climbed = 0;
		map_data = map_generator.GenerateMap();
		CreateMap();

	}

	//根据数据初始视觉化地图
	public void CreateMap()
	{
		foreach (Godot.Collections.Array<Room> current_floor in map_data)
		{
			foreach (Room room in current_floor)
			{
				if (room.next_rooms.Count > 0)
				{
					SpawnRoom(room);//生成房间
				}
			}
		}
		//BOSS房没有nextrooms，所以要单独生成
		var middle = (int)Math.Floor(MapGenerator.MAP_WIDTH * 0.5);
		SpawnRoom(map_data[MapGenerator.FLOORS - 1][middle]);
		var map_width_pixels = MapGenerator.X_DIST * (MapGenerator.MAP_WIDTH - 1);
		visuals.Position = new Vector2((GetViewportRect().Size.X - map_width_pixels) / 2, (GetViewportRect().Size.Y / 2) - 200);


	}
	//解锁楼层
	public void UnlockFloor(int which_floor = -1)
	{
		if (which_floor == -1)
		{
			which_floor = floors_climbed;
		}
		foreach (MapRoom map_room in rooms.GetChildren())
		{
			if (map_room.room.row == which_floor)
			{
				map_room.importavailable = true;
			}

		}

	}
	//解锁下一个房间
	public void UnlockNextRooms()
	{
		foreach (MapRoom map_room in rooms.GetChildren())
		{
			if (last_room.next_rooms.Contains(map_room.room))
			{
				map_room.importavailable = true;
			}
		}
	}

	public void ShowMap()
	{
		Show();
		camera_2d.Enabled = true;
	}
	public void HideMap()
	{
		Hide();
		camera_2d.Enabled = false;
		isDragging = false;
	}
	//生成房间
	public void SpawnRoom(Room room)
	{
		var new_map_room = MAP_ROOM.Instantiate() as MapRoom;
		rooms.AddChild(new_map_room);
		new_map_room.importroom = room;
		new_map_room.Selected += OnMapRoomSelected;
		new_map_room.Clicked += OnMapRoomClicked;
		ConnectLines(room);

		if (room.selected && room.row < floors_climbed)
		{
			new_map_room.ShowSelected();
		}

	}
	public void LoadMap(Godot.Collections.Array<Godot.Collections.Array<Room>> map, int floors_completed, Room last_room_climbed)
	{
		floors_climbed = floors_completed;
		map_data = map;
		if (last_room_climbed != null)
		{
			last_room = last_room_climbed;
		}
		else
		{
			
		}
		
		// GD.Print("Loaded map rows: ", map_data.Count);
		// for (int i = 0; i < map_data.Count; i++)
		// 	GD.Print("Row ", i, " size: ", map_data[i].Count);
		CreateMap();

		if (floors_climbed>0)
		{
			UnlockNextRooms();
		}
		else
		{
			UnlockFloor();
		}
	}

	public void ConnectLines(Room room)
	{
		if (room.next_rooms.Count == 0)
		{
			return;
		}
		foreach (Room next in room.next_rooms)
		{
			var new_map_line = MAP_LINE.Instantiate() as Line2D;
			new_map_line.AddPoint(room.position);
			new_map_line.AddPoint(next.position);
			lines.AddChild(new_map_line);
		}
	}


	public void OnMapRoomSelected(Room room)
	{
		
		last_room = room;
		floors_climbed += 1;
		events.instance.EmitSignal(events.SignalName.MapExited, room);//连接地图退出信号
	}

	public void OnMapRoomClicked(Room room)
	{
		foreach (MapRoom map_room in rooms.GetChildren())
		{
			if (map_room.room.row == room.row)//同一层的地图房间不可用
			{
				map_room.importavailable = false;
			}
		}
	}
	
	public void RebuildMapFromLoadedRooms(List<Room> loadedRooms)
	{
		if (loadedRooms == null || loadedRooms.Count == 0)
		{
			GD.PrintErr("没有可用的地图房间数据！");
			return;
		}

		// 1. 找出最大行数和列数，以便创建二维 Array
		int maxRow = -1;
		int maxCol = -1;
		foreach (Room r in loadedRooms)
		{
			if (r.row > maxRow) maxRow = r.row;
			if (r.column > maxCol) maxCol = r.column;
		}

		// 2. 初始化二维 Array
		if (map_data == null)
			map_data = new Godot.Collections.Array<Godot.Collections.Array<Room>>();
		else
			map_data.Clear();
		for (int r = 0; r <= maxRow; r++)
		{
			Array<Room> rowArray = new Array<Room>();
			for (int c = 0; c <= maxCol; c++)
			{
				rowArray.Add(null); // 占位，稍后填充
			}
			map_data.Add(rowArray);
		}
		foreach (Room r in loadedRooms)
		{
			// GD.Print($"准备放置房间: row={r.row}, col={r.column}");
			// GD.Print($"map_data.Count={map_data.Count}, 当前行长度={map_data[r.row].Count}");
			map_data[r.row][r.column] = r;
		}

		// 3. 把每个 Room 放到对应位置
		foreach (Room r in loadedRooms)
		{
			map_data[r.row][r.column] = r;
		}

		// GD.Print("地图重建完成：行数=" + (maxRow + 1) + ", 列数=" + (maxCol + 1));
	}
}


