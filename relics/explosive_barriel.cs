using Godot;
using System;
//遗物模版
public partial class explosive_barriel : Relic
{
	[Export]
	public int damage = 3;


	public override void ActivateRelic(RelicUi owner)
	{
		var enemies = owner.GetTree().GetNodesInGroup("enemies");
		var damage_effect = new damage_effect();
		damage_effect.amount = damage;
		damage_effect.receiver_modifier_type = Modifier.Type.NO_MODIFIER;//使伤害不应用修饰符
		damage_effect.execute(enemies);

		owner.Flash();
	}






}

