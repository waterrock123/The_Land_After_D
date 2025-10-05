using Godot;
using System;
[GlobalClass]
public partial class status : Resource
{
    [Signal]
    public delegate void StatusAppliedEventHandler(status Status);

    [Signal]
    public delegate void StatusChangedEventHandler();

    public enum Type
    {
        START_OF_TURN,
        END_OF_TURN,
        EVENT_BASED,
    }

    public enum StackType
    {
        NONE,
        INTENSITY,
        DURATION,
    }

    [Godot.ExportGroup("Status Data")]
    [Export]
    public string id;
    [Export]
    public Type type;
    [Export]
    public StackType stack_type;
    [Export]
    public bool can_expire;//能堆叠
    public int duration;
    [Export]
    public int importduration
    {
        get => duration;
        set => SetDuration(value);
    }
    public int stacks;
    [Export]
    public int importstacks
    {
        get => stacks;
        set => SetStacks(value);
    }

    [Godot.ExportGroup("Status Visuals")]
    [Export]
    public Texture2D icon;
    [Export(PropertyHint.MultilineText)]
    public string tooltip;



    //初始化状态
    public  virtual void  initialize_status(Node target)
    {


    }

    public virtual void apply_status(Node target)
    {
        EmitSignal(SignalName.StatusApplied, this);


    }

    public virtual string get_tooltip()
    {
        return tooltip;
    }


    public void SetDuration(int new_duration)
    {
        duration = new_duration;
        EmitSignal(SignalName.StatusChanged);
    }

    public void SetStacks(int new_stacks)
    {
        stacks = new_stacks;
        EmitSignal(SignalName.StatusChanged);

    }






}
