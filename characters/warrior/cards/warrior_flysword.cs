using Godot;
using Godot.Collections;
using System;

public partial class warrior_flysword : Card
{
	public int base_damage = 4;
	//名称 卡牌逻辑
	//描述：卡牌打出后触发什么
	// Called when the node enters the scene tree for the first time.
	[Export]
	public AudioStream optional_sound;//可选音效
	public override void apply_effects(Array<Node> _target, ModifierHandler modifiers)
	{
		var DamageEffect = new damage_effect();
		DamageEffect.amount = modifiers.GetModifiedValue(base_damage, Modifier.Type.DMG_DEALT);
		DamageEffect.sound = sound;
		DamageEffect.execute(_target);
		DamageEffect.execute(_target);
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

