using Godot;
using System;

public partial class RunStartup : Node
{

    public enum Type
    {
        NEW_RUN,
        CONTINUED_RUN
    }
    [Export]
    public Type type;
    [Export]
    public CharaStats picked_character;
    public static RunStartup Instance { get; private set; }

    public override void _Ready()
    {
        if (Instance != null && Instance != this)
        {
            QueueFree(); // 保证只有一个
            return;
        }
        Instance = this;
    }
}

