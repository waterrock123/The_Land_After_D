using Godot;
using System;
[GlobalClass]
public partial class effect : RefCounted
{
    public AudioStream sound;
    public virtual void execute(Godot.Collections.Array<Node> _targets)
    {

    }
    
}
