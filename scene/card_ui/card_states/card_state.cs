using Godot;
using System;

public partial class card_state : Node
{
    public enum State
    {
        BASE,
        CLICKED,
        DRAGGING,
        AIMING,
        RELEASED,
        INSLOT
    }

	[Signal]
    public delegate void TransitionRequestedEventHandler(card_state from, long Stateto);

	 [Export]
    public State state { get; set; }

	public CardUi cardUi;
    public events middlevent;

    public override void _EnterTree()
    {
        // CardUI 在你的树里位于两级父节点
        cardUi ??= GetParent()?.GetParent<CardUi>();

        // Autoload 单例（名字是 “Events”，区分大小写）
        middlevent ??= GetNodeOrNull<events>("/root/Events");
    }
    public override void _Ready()
    {
        cardUi = GetNode<CardUi>("CardUI");
    }
	public virtual void Enter()
    {
        // 相当于 GDScript 的 func enter(): pass
    }

	public virtual void Exit()
    {
    }

    public virtual void OnInput(InputEvent @event)
    {
    }

    public virtual void OnGuiInput(InputEvent @event)
    {
    }

    public virtual void OnMouseEntered()
    {
    }

    public virtual void OnMouseExited()
    {
    }

    public virtual void PostEntter()
    {
        
    } 
	public override void _Process(double delta)
    {
    }
}
