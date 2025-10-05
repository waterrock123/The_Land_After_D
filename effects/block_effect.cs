using Godot;
using System;

public partial class block_effect : effect
{
    [Export]
    public int amount = 0;
    public override void execute(Godot.Collections.Array<Node> _targets)
    {
        foreach (var target in _targets)
        {
            if (target == null)
                continue;
            if (target is Enemy enemy)
            {
                enemy.stats.TakeBlock(amount);
                sfxsound_player.instance.play(sound);
            }
            else if (target is player player)
            {
                player.stats.TakeBlock(amount);
                sfxsound_player.instance.play(sound);
            }
        }
    }
}
