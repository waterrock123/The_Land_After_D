using Godot;
using System;
using System.Security.Cryptography.X509Certificates;


public partial class card_inslot_state : card_state
{
    private bool canBeClicked = false;
    public override void _Ready()
    {

    }
    public override  async void Enter()
    {
        canBeClicked = false;
        await ToSignal(GetTree().CreateTimer(0.15), "timeout");
        canBeClicked = true;
    }

    public override void Exit()
    {
        
    }
    public void Pass()
    {
        EmitSignal(SignalName.TransitionRequested, this, (long)card_state.State.RELEASED);
    }
    public override void OnGuiInput(InputEvent @event)
    {
        if (!canBeClicked) return;
        
        var cancel =@event.IsActionPressed("鼠标右键");
        if (cancel)
        {
            GD.Print("有点击传到");
            GD.Print("进入base状态");
            events.instance.EmitSignal(events.SignalName.CardSlotOut, cardUi);
            EmitSignal(SignalName.TransitionRequested, this, (long)(int)card_state.State.BASE);
        }

    }




}
