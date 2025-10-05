using Godot;
using Godot.Collections;
using System;

public partial class warrior_big_slam : Card
{
	//名称 卡牌逻辑
	//描述：卡牌打出后触发什么
	// Called when the node enters the scene tree for the first time.
	public static status EXPOSED_STATUS = (status)GD.Load("res://statuses/exposed.tres");
	public int base_damage = 4;
	public int exposed_duration = 2;


	[Export]
	public AudioStream optional_sound;//可选音效
	public override void apply_effects(Array<Node> _target, ModifierHandler modifiers)
	{
		var damage_effect = new damage_effect();
		damage_effect.amount = modifiers.GetModifiedValue(base_damage, Modifier.Type.DMG_DEALT);
		damage_effect.sound = sound;
		damage_effect.execute(_target);
		var status_effect = new StatusEffect();
		var exposed = EXPOSED_STATUS.Duplicate() as status;
		exposed.importduration = exposed_duration;
		status_effect.Status = exposed;
		status_effect.execute(_target);



	}
	public override string GetDefaultTooltip()
	{
		return string.Format(tooltip_text, base_damage);
	}

	public override string GetUpdatedTooltip(ModifierHandler playermodifiers, ModifierHandler enemymodifiers)
	{
		var modified_dmg = playermodifiers.GetModifiedValue(base_damage, Modifier.Type.DMG_DEALT);
		if (enemymodifiers != null)
		{
			modified_dmg = enemymodifiers.GetModifiedValue(modified_dmg, Modifier.Type.DMG_TAKEN);
		}

		return string.Format(tooltip_text, modified_dmg);

        
    }
	

	

}

