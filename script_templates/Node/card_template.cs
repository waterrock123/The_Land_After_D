using Godot;
using Godot.Collections;
using System;

public partial class card_template : Card
{
	//名称 卡牌逻辑
	//描述：卡牌打出后触发什么
	// Called when the node enters the scene tree for the first time.
	[Export]
	public AudioStream optional_sound;//可选音效
	public override void apply_effects(Array<Node> _target, ModifierHandler modifiers)
	{
		GD.Print("卡牌已打出");
		GD.Print("目标是" + target);
	}
	
	
	public override string GetDefaultTooltip()
    {
        return tooltip_text;
    }

    public override string GetUpdatedTooltip(ModifierHandler playermodifiers, ModifierHandler enemymodifiers)
    {


        return tooltip_text;
    }

	

}
