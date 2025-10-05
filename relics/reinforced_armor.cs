using Godot;
using System;
//遗物模版
public partial class reinforced_armor : Relic
{
	[Export]
	public int block_bonus = 3;



	public override void ActivateRelic(RelicUi owner)
	{
		var player = owner.GetTree().GetNodesInGroup("player");
		var block_effect = new block_effect();
		block_effect.amount = block_bonus;
		block_effect.execute(player);

		owner.Flash();
		
	}







}

