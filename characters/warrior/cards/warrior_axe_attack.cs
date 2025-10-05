using Godot;
using Godot.Collections;
using System;

public partial class warrior_axe_attack : Card
{
    public int base_damage = 60;
    public override void apply_effects(Array<Node> targets, ModifierHandler modifiers)
    {
        var DamageEffect = new damage_effect();
        DamageEffect.amount = modifiers.GetModifiedValue(base_damage, Modifier.Type.DMG_DEALT);
        DamageEffect.sound = sound;
        DamageEffect.execute(targets);

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
