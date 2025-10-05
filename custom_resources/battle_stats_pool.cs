using Godot;
using System;
using Godot.Collections;
using System.Linq;

[GlobalClass]
public partial class battle_stats_pool : Resource
{
    [Export]
    public Array<battle_stats> pool;


    public Array<float> total_weights_by_tier = [0.0f, 0.0f, 0.0f];

    public Array<battle_stats> GetAllBattlesForTier(int tier)
    {
        var result = new Godot.Collections.Array<battle_stats>();
        foreach (var battle in pool.Where(battle => battle.battle_tier == tier))
        {
            result.Add(battle);
        }
        return result;
    }
    public void SetupWeightForTier(int tier)
    {
        var battles = GetAllBattlesForTier(tier);
        total_weights_by_tier[tier] = 0.0f;
        foreach (battle_stats battle in battles)
        {
            total_weights_by_tier[tier] += battle.weight;
            battle.accumulated_weight = total_weights_by_tier[tier];
        }
    }

    public battle_stats GetRandomBattleForTier(int tier)
    {
        var roll = rng.instance.RandfRange(0.0f, total_weights_by_tier[tier]);
        var battles = GetAllBattlesForTier(tier);

        foreach (battle_stats battle in battles)
        {
            if (battle.accumulated_weight > roll)
            {
                return battle;
            }
        }

        return null;

    }

    public void Setup()
    {
        for (int i = 0; i < 3; i++)
        {
            SetupWeightForTier(i);
        }
    }

}
