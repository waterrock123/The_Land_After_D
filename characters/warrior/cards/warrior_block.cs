using Godot;
using Godot.Collections;
using System;

public partial class warrior_block : Card
{
    public int base_block = 5;
    public override void apply_effects(Array<Node> targets, ModifierHandler modifiers)
    {
        var BlockEffect = new block_effect();
        BlockEffect.amount = base_block;
        BlockEffect.sound = sound;
        BlockEffect.execute(targets);

    }
    public override string GetDefaultTooltip()
	{
		return string.Format(tooltip_text, base_block);
	}

	public override string GetUpdatedTooltip(ModifierHandler playermodifiers, ModifierHandler enemymodifiers)
	{
		return string.Format(tooltip_text, base_block);
        
    }

}
