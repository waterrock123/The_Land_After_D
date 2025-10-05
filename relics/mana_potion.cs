using Godot;
using System;
//遗物模版
public partial class mana_potion : Relic
{
	public int new_mana = 1;

	public override void ActivateRelic(RelicUi owner)
	{
		void Handler()
        {
            AddMana(owner);
            events.instance.PlayerHandDrawn -= Handler; // ✅ 移除自己，保证只执行一次
        }

        events.instance.PlayerHandDrawn += Handler;
		
		
		
	}

	public void AddMana(RelicUi owner)
	{
		owner.Flash();
		var player = owner.GetTree().GetFirstNodeInGroup("player") as player;
		if (player != null)
		{
			player.stats.MANA += new_mana;

		}

	}


}

