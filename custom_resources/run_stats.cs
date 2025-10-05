using Godot;
using System;
using System.Security.Cryptography.X509Certificates;
[GlobalClass]
public partial class run_stats : Resource
{
    [Signal]
    public delegate void GoldChangedEventHandler();
    [Signal]
    public delegate void SupplyChangedEventHandler();
    
    public const int STARTING_GOLD = 70;
    public const int BASE_CARD_REWARDS = 3;
    public const double BASE_COMMON_WEIGHT = 6.0;
    public const double BASE_UNCOMMON_WEIGHT = 3.7;
    public const double BASE_RARE_WEIGHT = 0.3;
    public const int BASE_SUPPLY = 17;
    [Export]
    public int supply=BASE_SUPPLY;
    [Export]
    public int importsupply
    {
        get => supply;
        set => SetSupply(value);
    }
    [Export]
    public int gold = STARTING_GOLD;
    [Export]
    public int importgold {
        get => gold;
        set => SetGold(value);
    }
    [Export]
    public int card_rewards = BASE_CARD_REWARDS;
    [Export(PropertyHint.Range, "0.0,10.0")]
	public double common_weight = BASE_COMMON_WEIGHT;
    [Export(PropertyHint.Range, "0.0,10.0")]
	public double uncommon_weight = BASE_UNCOMMON_WEIGHT;
     [Export(PropertyHint.Range, "0.0,10.0")]
	public double rare_weight = BASE_RARE_WEIGHT;
    
    public void SetGold(int new_amount)
    {
        gold = new_amount;
        EmitSignal(SignalName.GoldChanged);

    }
    public void SetSupply(int new_amount)
    {
        supply = new_amount;
        EmitSignal(SignalName.SupplyChanged);
    }
    public void ResetWeights()
    {
        common_weight = BASE_COMMON_WEIGHT;
        uncommon_weight = BASE_UNCOMMON_WEIGHT;
        rare_weight = BASE_RARE_WEIGHT;
    }


}
