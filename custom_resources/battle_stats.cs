 using Godot;
using System;

[GlobalClass]
public partial class battle_stats : Resource
{
    [Export(PropertyHint.Range, "0,2")]
    public int battle_tier;
    [Export(PropertyHint.Range, "0.0,10.0")]
    public float weight;
    [Export]
    public int gold_reward_min;
    [Export]
    public int gold_reward_max;
    [Export]
    public PackedScene enemies;

    public float accumulated_weight = 0.0f;

    public int RollGoldReward()
    {
        return rng.instance.RandiRange(gold_reward_min, gold_reward_max);
    }





}
