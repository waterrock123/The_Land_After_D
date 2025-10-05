using Godot;
using Godot.Collections;
using System;
//效果
//创建一个应用于目标的效果
public partial class StatusEffect :effect
{
	public status Status;
	public override void execute(Array<Node> _targets)
	{
		foreach (var target in _targets)
		{
			if (target == null)
			{
				continue;
			}
			if (target is Enemy enemy)
			{
				enemy.status_handler.AddStatus(Status);
			}
			else if (target is player player)
			{
				player.status_handler.AddStatus(Status);
			}
		}
	}

}

