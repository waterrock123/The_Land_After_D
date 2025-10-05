using Godot;
using System;
using System.Security.Cryptography.X509Certificates;
using Godot.Collections;
using System.Linq;


public partial class MapGenerator : Node
{
    public static int X_DIST = 150;//两个不同房间之间水平间隙的像素数
    public static int Y_DIST = 200;//水平间隙像素
    public static int PLACEMENT_RANDOMNESS = 40;//放置性常量，随机移动房间的位置一点
    public static int FLOORS = 15;//层数
    public static int MAP_WIDTH = 7;//宽度
    public static int PATHS = 6;//希望为整个地图生成的路径的最大数量
    public static double MONSTER_ROOM_WEIGHT = 12.0;
    public static double SHOP_ROOM_WEIGHT = 2.5;
    public static double CAMPFIRE_ROOM_WEIGHT = 4.0;
    public static double EVENT_ROOM_WEIGHT = 5.0;
    

    [Export]
    public battle_stats_pool BattleStatsPool;
    [Export]
    public EventRoomPool event_room_pool;

    public Dictionary<Room.Type, double> random_room_type_weights = new Dictionary<Room.Type, double>
    {
        { Room.Type.MONSTER, 0.0 },
        { Room.Type.CAMPFIRE, 0.0 },
        { Room.Type.SHOP, 0.0 },
        { Room.Type.Event,0.0}
    };
    public double random_room_type_total_weight = 0;
    public Godot.Collections.Array<Godot.Collections.Array<Room>> map_data = new Godot.Collections.Array<Godot.Collections.Array<Room>>();

    //主要函数
    public Godot.Collections.Array<Godot.Collections.Array<Room>> GenerateMap()
    {
        //初始化地图（网格）
        map_data = GenerateInitialGrid();

        var starting_points = GetRandomStartingPoints();//起始点获取
        foreach (int j in starting_points)
        {
            var current_j = j;
            for (int i = 0; i < FLOORS - 1; i++)
            {
                current_j = SetupConnection(i, current_j);//迭代,设置下一个房间的索引
            }
        }

        BattleStatsPool.Setup();
        SetupBossRoom();
        SetupRandomRoomWeights();
        SetupRoomTypes();




        //测试
        


        return map_data;
    }


    public Godot.Collections.Array<Godot.Collections.Array<Room>> GenerateInitialGrid()
    {
        var result = new Godot.Collections.Array<Godot.Collections.Array<Room>>();
        for (int i = 0; i < FLOORS; i++)
        {
            Array<Room> adjacent_rooms = new Array<Room>();


            for (int j = 0; j < MAP_WIDTH; j++)
            {
                var current_room = new Room();
                var offset = new Vector2(GD.Randf(), GD.Randf()) * PLACEMENT_RANDOMNESS;
                current_room.position = new Vector2(j * X_DIST, i * -Y_DIST) + offset;
                current_room.row = i;
                current_room.column = j;
                current_room.next_rooms = new Array<Room>();

                if (i == FLOORS - 1)
                {
                    current_room.position.Y = (i + 1) * -Y_DIST;
                }
                adjacent_rooms.Add(current_room);

            }
            result.Add(adjacent_rooms);
        }
        return result;
    }
    public override void _Ready()
    {
       
    }
    public Array<int> GetRandomStartingPoints()
    {
        var y_coordinates = new Array<int>();//储存最终y坐标的整数数组
        var unique_points = 0;
        while (unique_points < 2)
        {
            unique_points = 0;
            y_coordinates = [];

            for (int i = 0; i < PATHS; i++)
            {
                var starting_point = rng.instance.RandiRange(0, MAP_WIDTH - 1);
                if (!y_coordinates.Contains(starting_point))
                {
                    unique_points += 1;
                }
                y_coordinates.Add(starting_point);

            }
        }
        return y_coordinates;

    }


    public int SetupConnection(int i, int j)
    {
        Room next_room = null;//下一个房间的候选
        var current_room = map_data[i][j] as Room;//获取当前房间的地图数据数组

        while (next_room == null || WouldCrossExistingPath(i, j, next_room))
        {
            var random_j = Math.Clamp(rng.instance.RandiRange(j - 1, j + 1), 0, MAP_WIDTH - 1);
            next_room = map_data[i + 1][random_j];
        }
        current_room.next_rooms.Add(next_room);
        return next_room.column;//返回下一个房间的索引
    }

    //检查是否有交叉路线
    public bool WouldCrossExistingPath(int i, int j, Room room)
    {
        Room left_neighbour = null;
        Room right_neighbour = null;
        //判断节点处于地图边缘的情况
        if (j > 0)
        {
            left_neighbour = map_data[i][j - 1];

        }
        if (j < MAP_WIDTH - 1)
        {
            right_neighbour = map_data[i][j + 1];
        }
        //右边邻居向左走就不能交叉向右走
        if (right_neighbour != null && room.column > j)
        {
            foreach (Room next_room in right_neighbour.next_rooms)
            {
                if (next_room.column < room.column)//检查右邻居节点是否有比此节点将去的节点索引更左的下一层房间节点
                {
                    return true;
                }
            }
        }
        //左边邻居向右走，就不能交叉向左走
        if (left_neighbour != null && room.column < j)
        {
            foreach (Room next_room in left_neighbour.next_rooms)
            {
                if (next_room.column > room.column)
                {
                    return true;
                }
            }
        }

        return false;//没有交叉违规

    }

    public void SetupBossRoom()//设置boss房
    {
        var middle = (int)Math.Floor(MAP_WIDTH * 0.5);
        var boss_room = map_data[FLOORS - 1][middle] as Room;
        for (int j = 0; j < MAP_WIDTH; j++)
        {
            var current_room = map_data[FLOORS - 2][j] as Room;
            if (current_room.next_rooms.Count > 0)
            {
                current_room.next_rooms = new Array<Room>();
                current_room.next_rooms.Add(boss_room);
            }
        }
        boss_room.type = Room.Type.BOSS;
        boss_room.BattleStats = BattleStatsPool.GetRandomBattleForTier(2);


    }

    //  设置房间权重
    public void SetupRandomRoomWeights()
    {
        random_room_type_weights[Room.Type.MONSTER] = MONSTER_ROOM_WEIGHT;
        random_room_type_weights[Room.Type.CAMPFIRE] = MONSTER_ROOM_WEIGHT + CAMPFIRE_ROOM_WEIGHT;
        random_room_type_weights[Room.Type.SHOP] = MONSTER_ROOM_WEIGHT + CAMPFIRE_ROOM_WEIGHT + SHOP_ROOM_WEIGHT;
        random_room_type_weights[Room.Type.Event] = random_room_type_weights[Room.Type.SHOP] + EVENT_ROOM_WEIGHT;
        random_room_type_total_weight = random_room_type_weights[Room.Type.Event];

    }

    public void SetupRoomTypes()
    {
        //第一层的节点都是战斗
        foreach (Room room in map_data[0])
        {
            if (room.next_rooms.Count > 0)
            {
                room.type = Room.Type.MONSTER;
                room.BattleStats = BattleStatsPool.GetRandomBattleForTier(0);
            }
        }

        //第9层总是宝箱房
        foreach (Room room in map_data[8])
        {
            if (room.next_rooms.Count > 0)
            {
                room.type = Room.Type.TREASURE;
            }
        }

        //boss前节点总是篝火
        foreach (Room room in map_data[13])
        {
            if (room.next_rooms.Count > 0)
            {
                room.type = Room.Type.CAMPFIRE;
            }
        }

        // 其余房间
        foreach (var current_floor in map_data)
        {
            foreach (Room room in current_floor)
            {
                foreach (Room next_room in room.next_rooms)
                {
                    if (next_room.type == Room.Type.NOT_ASSIGNED)
                    {
                        SetRoomRandomly(next_room);


                    }
                }
            }
        }
    }

    public void SetRoomRandomly(Room room_to_set)
    {
        bool campfire_below_4 = true;//4楼以下不能有营火 true表示默认违反此规则
        bool consecutive_campfire = true;//不能有连续的篝火
        bool consecutive_shop = true;//不能有连续的商店
        bool campfire_on_13 = true;//不能在最后一段有营火

        Room.Type type_candidate;
        do
        {
            type_candidate = GetRandomRoomTypeByWeight();

            var is_campfire = type_candidate == Room.Type.CAMPFIRE;
            var has_campfire_parent = RoomHasParentOfType(room_to_set, Room.Type.CAMPFIRE);
            var is_shop = type_candidate == Room.Type.SHOP;
            var has_shop_parent = RoomHasParentOfType(room_to_set, Room.Type.SHOP);

            campfire_below_4 = is_campfire && room_to_set.row < 3;
            consecutive_campfire = is_campfire && has_campfire_parent;
            consecutive_shop = is_shop && has_shop_parent;
            campfire_on_13 = is_campfire && room_to_set.row == 12;
        }
        while (campfire_below_4 | consecutive_campfire | consecutive_shop | campfire_on_13);
        room_to_set.type = type_candidate;
        if (type_candidate == Room.Type.MONSTER)
        {
            var tier_for_monster_rooms = 0;
            if (room_to_set.row > 2)
            {
                tier_for_monster_rooms = 1;

            }
            room_to_set.BattleStats = BattleStatsPool.GetRandomBattleForTier(tier_for_monster_rooms);
        }
        if (type_candidate == Room.Type.Event)
        {
            room_to_set.event_scene = event_room_pool.GetRandom();
        }

    }


    public bool RoomHasParentOfType(Room room, Room.Type type)
    {

        Array<Room> parents = [];
        if (room.column > 0 && room.row > 0)
        {
            //左父亲节点
            var parent_candidate = map_data[room.row - 1][room.column - 1] as Room;
            if (parent_candidate.next_rooms.Contains(room))
            {
                parents.Add(parent_candidate);
            }

        }

        //下面的中间的父房间
        if (room.row > 0)
        {
            var parent_candidate = map_data[room.row - 1][room.column] as Room;
            if (parent_candidate.next_rooms.Contains(room))
            {
                parents.Add(parent_candidate);
            }

        }
        //右边的父房间
        if (room.column < MAP_WIDTH - 1 && room.row > 0)
        {
            var parent_candidate = map_data[room.row - 1][room.column + 1] as Room;
            if (parent_candidate.next_rooms.Contains(room))
            {
                parents.Add(parent_candidate);
            }
        }

        foreach (Room parent in parents)
        {
            if (parent.type == type)
            {
                return true;
            }
        }
        return false;



    }


    public Room.Type GetRandomRoomTypeByWeight()
    {
        var roll = rng.instance.RandfRange(0.0f, (float)random_room_type_total_weight);

        foreach (Room.Type type in random_room_type_weights.Keys)
        {
            if (random_room_type_weights[type] > roll)
            {
                return type;

            }
        }
        return Room.Type.MONSTER;



    }

}
