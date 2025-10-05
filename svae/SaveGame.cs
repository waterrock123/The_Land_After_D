using Godot;
using Godot.Collections;
using System;
public partial class SaveGame : Resource 
{
    const string SAVE_PATH = "user://savegame.tres";
    [Export]
    public ulong rng_seed;
    [Export]
    public ulong rng_state;
    [Export] 
    public run_stats Run_Stats;
    [Export] public CharaStats char_stats;
    [Export] public Cardpile current_deck;
    [Export] public int current_health;
    [Export] public Array<Relic> relics;
        //以上没有问题
        //[Export] public Godot.Collections.Array<Godot.Collections.Array<Room>> map_data;
    [Export]
    public int last_room_row;
    [Export]
    public int last_room_col;
    [Export] public int floors_climbed;
    [Export] public bool was_on_map;
    public void SaveData()
    {
            var err = ResourceSaver.Save(this, SAVE_PATH);
    }
    public static SaveGame LoadData()
    {
        if (FileAccess.FileExists(SAVE_PATH))
        {
            return ResourceLoader.Load(SAVE_PATH, "SaveGame", ResourceLoader.CacheMode.Ignore) as SaveGame;
        }
        return null;
    }



    public static void DeleteData()
    {
        if (FileAccess.FileExists(SAVE_PATH))
        {
            DirAccess.RemoveAbsolute(SAVE_PATH);
        }
    } 
}