using Godot;
using Godot.Collections;
using System;
//效果
//创建一个应用于目标的效果
public partial class effect_template :effect
{
	int member_var = 0;
    public override void execute(Array<Node> _targets)
    {
		GD.Print("这是一个效果，应用于:" + _targets);
    }

}
