using Godot;
using System;

public partial class EventRoomButton : Button
{
    public Callable event_button_callback;
    public bool IsEndButton=true;
    public override void _Ready()
    {
        this.Pressed += OnPressed;
    }

    public void OnPressed()
    {
        if (event_button_callback.Target != null)
        {
            event_button_callback.Call();
        }
        if (IsEndButton)
        {
            
            events.instance.EmitSignal(events.SignalName.EventRoomExited);
        }

    }

}
