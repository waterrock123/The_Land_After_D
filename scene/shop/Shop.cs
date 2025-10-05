using Godot;
using Godot.Collections;
using System;

public partial class Shop : Control
{
	public static PackedScene SHOP_CARD = GD.Load<PackedScene>("res://scene/shop/shop_card.tscn");
	public static PackedScene SHOP_RELIC = GD.Load<PackedScene>("res://scene/shop/shop_relic.tscn");

	[Export]
	public Array<Relic> shop_relics;//所有能在商店中出现的遗物
	[Export]
	public CharaStats char_stats;
	[Export]
	public run_stats RunStats;
	[Export]
	public RelicHandler relic_handler;

	public AnimationPlayer shop_keeper_animation;
	public Timer blink_timer;

	public ModifierHandler modifier_handler;

	public HBoxContainer cards;
	public HBoxContainer relics;
	public CardTooltipPopup card_tooltip_popup;

	public override void _Ready()
	{
		modifier_handler = GetNode<ModifierHandler>("ModifierHandler");
		shop_keeper_animation = GetNode<AnimationPlayer>("%ShopkeeperAnimation");
		blink_timer = GetNode<Timer>("%BlinkTimer");
		cards = GetNode<HBoxContainer>("%Cards");
		relics = GetNode<HBoxContainer>("%Relics");
		card_tooltip_popup = GetNode<CardTooltipPopup>("%CardTooltipPopup");
		foreach (ShopCard shop_card in cards.GetChildren())
		{
			shop_card.QueueFree();
		}
		foreach (ShopRelic shop_relic in relics.GetChildren())
		{
			shop_relic.QueueFree();
		}

		events.instance.ShopCardBought += OnShopCardBought;
		events.instance.ShopRelicBought += OnShopRelicBought;

		BlinkTimerSetup();//设置随机闪烁定时器
		blink_timer.Timeout += () => { OnBlinkTimerTimeout(); };

	}

	public void BlinkTimerSetup()
	{
		blink_timer.WaitTime = GD.RandRange(1.0, 5.0);
		blink_timer.Start();
	}

	public void OnBlinkTimerTimeout()
	{
		shop_keeper_animation.Play("blink");
		BlinkTimerSetup();
	}

    public override void _ExitTree()
	{

		events.instance.ShopCardBought -= OnShopCardBought;
		events.instance.ShopRelicBought -= OnShopRelicBought;
	}


	public void PopulateShop()
	{
		GenerateShopCards();
		GenerateShopRelics();
	}

	//初始化商店卡牌
	public void GenerateShopCards()
	{
		var shop_card_array = new Array<Card>();
		var availablie_cards = char_stats.draftable_cards.DuplicateCards();
		rng.ArrayShuffle(availablie_cards);
		shop_card_array = availablie_cards.Slice(0, 3);//切片取3张
		foreach (Card card in shop_card_array)
		{
			var new_shop_card = SHOP_CARD.Instantiate() as ShopCard;
			cards.AddChild(new_shop_card);
			new_shop_card.importcard = card;
			new_shop_card.current_card_ui.TooltipRequested += card_tooltip_popup.show_tooltip;
			new_shop_card.gold_cost = GetUpdateShopCost(new_shop_card.gold_cost);
			new_shop_card.Update(RunStats);
		}
	}
	//初始化商店遗物
	public void GenerateShopRelics()
	{
		var shop_relics_array = new Array<Relic>();
		var availablie_relics = new Array<Relic>();
		foreach (Relic relic in shop_relics)
		{
			var can_appear = relic.CanApperAsReward(char_stats);
			var already_had_it = relic_handler.HasRelic(relic.id);
			if (can_appear && !already_had_it)
			{
				availablie_relics.Add(relic);
			}
		}
		rng.ArrayShuffleRelic(availablie_relics);
		shop_relics_array = availablie_relics.Slice(0, 3);

		foreach (Relic relic in shop_relics_array)
		{
			var new_shop_relic = SHOP_RELIC.Instantiate() as ShopRelic;
			relics.AddChild(new_shop_relic);
			new_shop_relic.importrelic = relic;
			new_shop_relic.gold_cost = GetUpdateShopCost(new_shop_relic.gold_cost);
			new_shop_relic.Update(RunStats);
		}

	}

	public int GetUpdateShopCost(int original_cost)
	{
		return modifier_handler.GetModifiedValue(original_cost, Modifier.Type.SHOP_COST);
	}



	public void UpdateItems()
	{
		foreach (ShopCard shop_card in cards.GetChildren())
		{
			
			shop_card.Update(RunStats);
		}

		foreach (ShopRelic shop_relic in relics.GetChildren())
		{
			
			shop_relic.Update(RunStats);
		}
	}

	public void UpdateItemsCosts()
	{
		foreach (ShopCard shopcard in cards.GetChildren())
		{
			shopcard.gold_cost = GetUpdateShopCost(shopcard.gold_cost);
			shopcard.Update(RunStats);
		}
		foreach (ShopRelic shoprelic in relics.GetChildren())
		{
			shoprelic.gold_cost = GetUpdateShopCost(shoprelic.gold_cost);
			shoprelic.Update(RunStats);
		}

	}

	public void OnShopCardBought(Card card, int gold_cost)
	{
		char_stats.deck.add_card(card);
		RunStats.importgold -= gold_cost;
		UpdateItems();
	}

	public void OnShopRelicBought(Relic relic, int gold_cost)
	{
		relic_handler.AddRelic(relic);
		RunStats.importgold -= gold_cost;
		if (relic is coupons)
		{
			var coupons_relic = relic as coupons;
			coupons_relic.AddShopModifier(this);
			UpdateItemsCosts();
		}
		else
		{
			UpdateItems();
		}
		
	}




    public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_cancel") && card_tooltip_popup.Visible)
		{
			card_tooltip_popup.hide_tooltip();
		}
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void OnBackButtonPressed()
	{
		events.instance.EmitSignal(events.SignalName.ShopExited);
	}
}
