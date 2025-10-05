using Godot;
using Godot.Collections;
using System;
[GlobalClass]
public partial class EventRoomPool : Resource
{
    [Export]
    public Array<PackedScene> event_rooms;

    public PackedScene GetRandom()
    {
        return event_rooms.PickRandom();
    }


}
