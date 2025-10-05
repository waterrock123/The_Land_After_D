using Godot;
using System;
[GlobalClass]
public partial class intent : Resource
{
    [Export]
    public String base_text;
    [Export]
    public Texture icon;

    public string current_text;
}
