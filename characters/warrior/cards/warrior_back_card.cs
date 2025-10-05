using Godot;
using Godot.Collections;
using System;

public partial class warrior_back_card : Card
{
	//名称 卡牌逻辑
	//描述：卡牌打出后触发什么
	// Called when the node enters the scene tree for the first time.
	[Export]
	public AudioStream optional_sound;//可选音效
	public override void apply_effects(Array<Node> _target,ModifierHandler modifiers)
	{
		GD.Print("卡牌已打出");
		GD.Print("目标是" + target);
	}
	

	

}

