using Godot;
using Godot.Collections;
using System;

public partial class warrior_true_strength : Card
{
	//名称 卡牌逻辑
	//描述：卡牌打出后触发什么
	public static status TRUE_STRENGTH_FORM_STATUS = (status)GD.Load("res://statuses/true_strength_form.tres");
	[Export]
	public AudioStream optional_sound;//可选音效
	public override void apply_effects(Array<Node> _target,ModifierHandler modifiers)
	{
		var status_effect = new StatusEffect();
		var true_strength = TRUE_STRENGTH_FORM_STATUS.Duplicate();
		status_effect.Status = (status)true_strength;
		status_effect.execute(_target);
		
	}
	

	

}

