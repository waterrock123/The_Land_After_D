using Godot;
using System;
using System.Threading.Tasks;

public partial class ShopRelic : VBoxContainer
{
	public static PackedScene RELIC_UI = (PackedScene)GD.Load("res://scene/relic_handler/relic_ui.tscn");
	public Relic relic;
	[Export]
	public Relic importrelic
	{
		get => relic;
		set => SetRelic(value);
	}

	public CenterContainer relic_container;
	public HBoxContainer price;
	public Label price_label;
	public Button buy_button;
	public int gold_cost;






	public override void _Ready()
	{
		relic_container = GetNode<CenterContainer>("%RelicContainer");
		price = GetNode<HBoxContainer>("%Price");
		price_label = GetNode<Label>("%PriceLabel");
		buy_button = GetNode<Button>("%BuyButton");
		gold_cost = rng.instance.RandiRange(100, 300);
	}

	public void Update(run_stats RunStats)
	{
		if (!IsInstanceValid(relic_container) 
        || !IsInstanceValid(price) 
        || !IsInstanceValid(price_label) 
        || !IsInstanceValid(buy_button))
    {
        return;
    }

		price_label.Text = gold_cost.ToString();

		if (RunStats.gold >= gold_cost)
		{
			price_label.RemoveThemeColorOverride("font_color");
			buy_button.Disabled = false;
		}
		else
		{
			price_label.AddThemeColorOverride("font_color", Colors.Red);
			buy_button.Disabled = true;
		}
	}


	public async Task SetRelic(Relic new_relic)
	{
		if (!IsNodeReady())
		{
			await ToSignal(this, "ready");
		}
		relic = new_relic;

		foreach (RelicUi relic_ui in relic_container.GetChildren())
		{
			relic_ui.QueueFree();
		}

		var new_relic_ui = RELIC_UI.Instantiate() as RelicUi;
		relic_container.AddChild(new_relic_ui);
		new_relic_ui.importrelic = relic;

	}

	public override void _Process(double delta)
	{
	}

	public void OnBuyButtonPressed()
	{
		events.instance.EmitSignal(events.SignalName.ShopRelicBought, relic, gold_cost);
		relic_container.QueueFree();
		price.QueueFree();
		buy_button.QueueFree();
		
	}
}
