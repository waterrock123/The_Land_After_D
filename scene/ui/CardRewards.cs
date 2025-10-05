using Godot;
using System;
using Godot.Collections;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
public partial class CardRewards : ColorRect
{
	[Signal]
	public delegate void CardRewardSelectedEventHandler(Card card);
	public static PackedScene CARD_MENU_UI = (PackedScene)ResourceLoader.Load("res://scene/ui/card_menu_ui.tscn");


	public Array<Card> rewards;

	[Export]
	public Array<Card> importrewards
	{
		get => rewards;
		set => SetRewards(value);
	}
	public HBoxContainer cards;
	public Button skip_card_reward;
	public CardTooltipPopup card_tooltip_popup;
	public Button take_button;
	public Card selected_card;
	public override void _Ready()
	{
		cards = GetNode<HBoxContainer>("%Cards");
		skip_card_reward = GetNode<Button>("%SkipCardReward");
		card_tooltip_popup = GetNode<CardTooltipPopup>("CardTooltipPopup");
		take_button = GetNode<Button>("%TakeButton");
		ClearRewards();
		take_button.Pressed += () =>
		{
			EmitSignal(SignalName.CardRewardSelected, selected_card);
			
			QueueFree();//删除整个选择屏幕
		};

		skip_card_reward.Pressed += () =>
		{
			EmitSignal(SignalName.CardRewardSelected, null);
		
			QueueFree();
		};


	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_cancel"))
		{
			card_tooltip_popup.hide_tooltip();
		}
	}
	public void ClearRewards()
	{
		foreach (Node card in cards.GetChildren())
		{
			card.QueueFree();
		}
		card_tooltip_popup.hide_tooltip();
		selected_card = null;
	}

	public void Show_Tooltip(Card card)
	{
		selected_card = card;
		card_tooltip_popup.show_tooltip(card);
	}

	public async Task SetRewards(Array<Card> new_cards)
	{
		rewards = new_cards;
		if (!IsNodeReady())
		{
			await ToSignal(this, "ready");

		}
		ClearRewards();
		foreach (Card card in rewards)
		{
			var new_card = CARD_MENU_UI.Instantiate() as CardMenuUi;//创建一个展示卡牌的ui
			cards.AddChild(new_card);//将新卡牌ui添加到cards（可选择卡）中
			new_card.importcard = card;//设置其卡牌的各种属性（主要是卡面，描述，稀有度）
			new_card.TooltipRequested += Show_Tooltip;//连接信号，使被点击时显示提示场景
			
		}
	} 
	
	
}
