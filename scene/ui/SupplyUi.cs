using Godot;
using System;

public partial class SupplyUi : HBoxContainer
{
    public run_stats RunStats;
    public Label supply;
    [Export]
    public run_stats importrun_stats
    {
        get => RunStats;
        set => SetRunStats(value);
    }
    public override void _Ready()
    {
        supply = GetNode<Label>("Label");
        supply.Text = '0'.ToString();
    }



    public void SetRunStats(run_stats new_value)
    {
        RunStats = new_value;
        RunStats.SupplyChanged -= UpdateSupply;
        RunStats.SupplyChanged += UpdateSupply;
        UpdateSupply();
    }
    public void UpdateSupply()
    {
        supply.Text = RunStats.supply.ToString();
    }

    
    

}
