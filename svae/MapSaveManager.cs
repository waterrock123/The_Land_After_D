using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections; // Godot Array 和 Dictionary

using Newtonsoft.Json;

/// <summary>
/// 数据类：只保存 Room 的必要信息，用于 JSON 序列化
/// </summary>
[Serializable]
public class RoomData
{
    public int row;
    public int column;
    public string type; // Room.Type 枚举的名字
    public float posX;
    public float posY;
    public bool selected;

    // 保存 next_rooms 的索引，而不是直接保存 Room 对象
    public List<int> nextRoomIndices = new List<int>();

    // 如果是怪物房/Boss房，保存战斗信息
    public BattleStatsData battleStats; // 保存战斗信息 
    //如果是事件房,保存事件信息
    public string event_scene_path;
}

/// <summary>
/// 保存 battle_stats 的必要字段（示例）
/// </summary>
[Serializable]
public class BattleStatsData
{
    public int battle_tier;
    public float weight;
    public int gold_reward_min;
    public int gold_reward_max;
    public string enemiesPath; // PackedScene 的路径，JSON 保存字符串
}

/// <summary>
/// 保存整个地图的数据类
/// </summary>
[Serializable]
public class MapSaveData
{
    public List<RoomData> rooms = new List<RoomData>();
}

/// <summary>
/// Map 的保存/加载管理类
/// </summary>
public static class MapSaveManager
{
    private static readonly string SAVE_PATH = "user://map_save.json";

    /// <summary>
    /// 保存地图
    /// </summary>
    /// <param name="mapData">Godot Array<Array<Room>>，原始地图数据</param>
    public static void SaveMap(Array<Array<Room>> mapData)
    {
        // 创建一个 MapSaveData 对象，用于存储所有 Room 的数据
        MapSaveData saveData = new MapSaveData();
        Godot.Collections.Dictionary<Room, int> roomToIndex = new Godot.Collections.Dictionary<Room, int>(); // 用于记录 Room 对应的索引

        // 遍历 map_data，把 Room 转成 RoomData 并记录索引
        int index = 0;
        foreach (Array<Room> row in mapData)
        {
            foreach (Room room in row)
            {
                RoomData rd = new RoomData();
                rd.row = room.row;
                rd.column = room.column;
                rd.type = room.type.ToString();
                rd.selected = room.selected;
                rd.posX = room.position.X;
                rd.posY = room.position.Y;
                rd.event_scene_path = room.event_scene != null ? room.event_scene.ResourcePath : "";

                // 保存 battleStats
                if (room.BattleStats != null)
                {
                    rd.battleStats = new BattleStatsData()
                    {
                        battle_tier = room.BattleStats.battle_tier,
                        weight = room.BattleStats.weight,
                        gold_reward_min = room.BattleStats.gold_reward_min,
                        gold_reward_max = room.BattleStats.gold_reward_max,
                        enemiesPath = room.BattleStats.enemies != null ? room.BattleStats.enemies.ResourcePath : ""
                    };
                }

                saveData.rooms.Add(rd);
                roomToIndex[room] = index;
                index++;
            }
        }

        // 第二遍处理 next_rooms，把对象引用转成索引
        index = 0;
        foreach (Array<Room> row in mapData)
        {
            foreach (Room room in row)
            {
                if (room.next_rooms != null)
                {
                    foreach (Room nextRoom in room.next_rooms)
                    {
                        if (roomToIndex.ContainsKey(nextRoom))
                            saveData.rooms[index].nextRoomIndices.Add(roomToIndex[nextRoom]);
                    }
                }
                index++;
            }
        }

        // 序列化为 JSON
        string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
        using var file = FileAccess.Open(SAVE_PATH, FileAccess.ModeFlags.Write);
        file.StoreString(json);
        
        
        GD.Print("地图已保存到: " + SAVE_PATH);
    }

    /// <summary>
    /// 加载地图
    /// </summary>
    /// <returns>返回一个 List<Room>，包含所有 Room 对象，可重新生成 map_data</returns>
    public static List<Room> LoadMap()
    {
        if (!FileAccess.FileExists(SAVE_PATH))
        {
            GD.PrintErr("地图存档不存在！");
            return new List<Room>();
        }
        using var file = FileAccess.Open(SAVE_PATH, FileAccess.ModeFlags.Read);
        string json = file.GetAsText();
        MapSaveData saveData = JsonConvert.DeserializeObject<MapSaveData>(json);
        file.Close();

        if (saveData == null || saveData.rooms.Count == 0)
        {
            GD.PrintErr("地图数据为空！");
            return new List<Room>();
        }

        // 第一步：生成所有 Room 对象
        List<Room> roomObjects = new List<Room>();
        foreach (RoomData rd in saveData.rooms)
        {
            Room r = new Room();
            r.row = rd.row;
            r.column = rd.column;
            r.type = Enum.Parse<Room.Type>(rd.type);
            r.selected = rd.selected;
            r.position=new Vector2(rd.posX, rd.posY);
            r.event_scene = !string.IsNullOrEmpty(rd.event_scene_path) ? GD.Load<PackedScene>(rd.event_scene_path) : null;
            if (rd.battleStats != null)
            {
                r.BattleStats = new battle_stats()
                {
                    battle_tier = rd.battleStats.battle_tier,
                    weight = rd.battleStats.weight,
                    gold_reward_min = rd.battleStats.gold_reward_min,
                    gold_reward_max = rd.battleStats.gold_reward_max,
                    enemies = !string.IsNullOrEmpty(rd.battleStats.enemiesPath) ? GD.Load<PackedScene>(rd.battleStats.enemiesPath) : null
                };
            }

            roomObjects.Add(r);
        }

        // 第二步：恢复 next_rooms
        for (int i = 0; i < saveData.rooms.Count; i++)
        {
            RoomData rd = saveData.rooms[i];
            Room r = roomObjects[i];
            r.next_rooms = new Array<Room>();
            foreach (int nextIndex in rd.nextRoomIndices)
            {
                if (nextIndex >= 0 && nextIndex < roomObjects.Count)
                    r.next_rooms.Add(roomObjects[nextIndex]);
            }
        }

        GD.Print("地图已加载，共 " + roomObjects.Count + " 个房间");
        return roomObjects;
    }
}
