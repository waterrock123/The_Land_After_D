using Godot;
using System;
using System.Threading.Tasks;
[GlobalClass]
public partial class EventRoom : Control
{
    [Export]
    public CharaStats character_stats;
    [Export]
    public run_stats RunStats;
    [Export]
    public RelicHandler relic_handler;

    public virtual async Task SetUp()
    {
        if (!IsNodeReady())
        {
            await ToSignal(this, "ready");
        }
        
    }
}
