using Godot;
using System;
//遗物模版
public partial class healing_potion : Relic
{
	[Export]
	public int heal_amount = 6;

	public override void ActivateRelic(RelicUi owner)
	{
		var player = owner.GetTree().GetFirstNodeInGroup("player") as player;
		if (player != null)
		{
			player.stats.Heal(heal_amount);
			owner.Flash();
		}
	}

	




}

