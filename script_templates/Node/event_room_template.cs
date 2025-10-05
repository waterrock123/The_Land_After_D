using Godot;
using System;
using System.Threading.Tasks;
public partial class event_room_template : EventRoom
{
    public EventRoomButton example_button;

    public override void _Ready()
    {
        example_button = GetNode<EventRoomButton>("%ExampleButton");

    }

    public override async Task SetUp()
    {
        
        if (!IsNodeReady())
        {
            await ToSignal(this, "ready");
        }
    }


}
