using Godot;
using System;
using Godot.Collections;
[GlobalClass]
public partial class Room : Resource
{
    public enum Type
    {
        NOT_ASSIGNED,//未分配
        MONSTER,
        TREASURE,
        CAMPFIRE,
        SHOP,
        Event,
        BOSS
    }
    [Export]
    public Type type;
    [Export]
    public int row;
    [Export]
    public int column;
    [Export]
    public Vector2 position;
    [Export]
    public Array<Room> next_rooms=null;
    [Export]
    public bool selected = false;

    [Export]
    public PackedScene event_scene;//只为事件房使用


    //只为怪物房和boss房使用
    [Export]
    public battle_stats BattleStats;

    public override string ToString()
    {
        string typeName = Enum.GetName(typeof(Type), type);
        return $"{column}({typeName[0]})";
    }


}
