using Godot;
using System;
using System.Threading.Tasks;

public partial class ShopCard : VBoxContainer
{
	public CardMenuUi current_card_ui;
	public static PackedScene CARD_MENU_UI = (PackedScene)GD.Load("res://scene/ui/card_menu_ui.tscn");
	public Card card;
	[Export]
	public Card importcard
	{
		get => card;
		set => SetCard(value);
	}
	public CenterContainer card_container;
	public HBoxContainer price;
	public Label price_label;
	public Button buy_button;
	public int gold_cost;




	public override void _Ready()
	{
		card_container = GetNode<CenterContainer>("%CardContainer");
		price = GetNode<HBoxContainer>("%Price");
		price_label = GetNode<Label>("%PriceLabel");
		buy_button = GetNode<Button>("%BuyButton");
		gold_cost = rng.instance.RandiRange(100, 300);
		



	}

	public void Update(run_stats RunStats)
	{
		if (!IsInstanceValid(card_container) 
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

	public async Task SetCard(Card new_card)
	{
		if (!IsNodeReady())
		{
			await ToSignal(this, "ready");
		}

		card = new_card;
		foreach (CardMenuUi card_menu_ui in card_container.GetChildren())
		{
			card_menu_ui.QueueFree();
		}

		var new_card_menu_ui = CARD_MENU_UI.Instantiate() as CardMenuUi;
		card_container.AddChild(new_card_menu_ui);
		new_card_menu_ui.importcard = card;
		current_card_ui = new_card_menu_ui;

	}

	public override void _Process(double delta)
	{
	}

	public void OnBuyButtonPressed()
	{
		events.instance.EmitSignal(events.SignalName.ShopCardBought, card, gold_cost);
		card_container.QueueFree();
		price.QueueFree();
		buy_button.QueueFree();
		
	}
}
