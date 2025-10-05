using Godot;
using System;

public partial class damage_effect : effect
{
    [Export]
    public int amount = 0;
    public  Modifier.Type receiver_modifier_type = Modifier.Type.DMG_TAKEN;
     public override void execute(Godot.Collections.Array<Node> _targets)
    {
        foreach (var target in _targets)
        {
            if (target == null)
                continue;
            if (target is Enemy enemy)
            {
                enemy.TakeDamage(amount,receiver_modifier_type);

                sfxsound_player.instance.play(sound);
            }
            else if (target is player player)
            {
                player.TakeDamage(amount,receiver_modifier_type);
                sfxsound_player.instance.play(sound);
            }
        }
    }

}
