using Godot;
using System;

//玩家回合顺序：
//1.START_OF_TURN Relics 回合开始，应用此类遗物效果
//2.START_OF_TURN STATUSES 回合开始 应用此类状态（buff，debuff）效果
//3. Draw Hand 抽牌到手牌
//4.End Turn 结束回合
//5.END_OF_TURN Relics 回合结束 ，应用回合结束遗物
//6.END_OF_TURN Statuses 回合结束 应用回合结束状态效果
//7.Discard Hand 弃掉手牌的牌

public partial class PlayerHandler : Node
{
	double HandDrawInterval = 0.25;//抽卡间隔
	double HandDiscardInterval = 0.25;//弃牌间隔
	[Export]
	public NodePath HandPath;
	[Export]
	public RelicHandler relics;

	[Export]
	public player Player;
	public hand Hand;
	public CharaStats character;
	private Tween drawTween;
	public override void _Ready()
	{
		events.instance.CardPlayed += OnCardPlayed;
		if (HandPath != null && !HandPath.IsEmpty)
		{
			Hand = GetNode<hand>(HandPath);
		}
		else
		{
			GD.PrintErr("PlayerHandler: HandPath 没有设置！");
		}

	}
	public override void _ExitTree()
	{
		// 解绑，防止重载/切换时回调旧实例
		relics.RelicsActivated -= OnRelicsActivated;
        Player.status_handler.StatusesApplied -= OnStatusesApplied;

		events.instance.CardPlayed -= OnCardPlayed;
		
        

	}
	public void StartBattle(CharaStats char_stats)
	{

		character = char_stats;//连接角色状态
							   //设置战斗所需事项
		character.draw_pile = (Cardpile)character.deck.CustomDuplicate();//设置抽牌堆,为当前牌组的一个副本
		character.draw_pile.shuffle();//打乱抽牌组
		character.discard = new Cardpile();//设置弃牌堆，为空
		relics.RelicsActivated += OnRelicsActivated;
		Player.status_handler.StatusesApplied -= OnStatusesApplied;
		Player.status_handler.StatusesApplied += OnStatusesApplied;
		StartTurn();
	}

	public void OnCardPlayed(Card card)
	{
		if (card.exhausts || card.type == Card.Type.Power)
		{
			return;
		}
		character.discard.add_card(card);
	}
	public void StartTurn()
	{
		character.Block = 0;
		character.ResetMana();
		relics.ActivateRelicsByType(Relic.Type.START_OF_TURN);

	}
	//抽卡（单张）
	public void EndTurn()
	{

		Hand.DisableHand();//使手牌不可用
		relics.ActivateRelicsByType(Relic.Type.END_OF_TURN);


	}
	public void DrawCard()
	{

		ReshuffleDeckFromDiscard();//洗牌函数，如果抽牌堆里没有牌就将弃牌堆中牌洗回抽牌堆

		Hand.AddCard(character.draw_pile.draw_card());
		ReshuffleDeckFromDiscard();
	}
	public void DiscardCards()
	{
		if (Hand.GetChildCount() == 0)
		{
			events.instance.EmitSignal(events.SignalName.PlayerHandDiscarder);
			return;
		}
		Tween tween = CreateTween();
		foreach (CardUi cardUi in Hand.GetChildren())
		{
			// 相当于 character.discard.add_card(cardUi.card)
			tween.TweenCallback(Callable.From(() => character.discard.add_card(cardUi.card)));

			// 相当于 hand.discard_card(cardUi)
			tween.TweenCallback(Callable.From(() => Hand.DiscardCard(cardUi)));

			// 加一个间隔
			tween.TweenInterval(HandDiscardInterval);
		}
		tween.Finished += () =>
		{
			events.instance.EmitSignal(events.SignalName.PlayerHandDiscarder);
		};
	}
	public void ReshuffleDeckFromDiscard()
	{
		if (!character.draw_pile.empty())
		{
			return;
		}
		while (!character.discard.empty())
		{
			character.draw_pile.add_card(character.discard.draw_card());
		}
		character.draw_pile.shuffle();
	}
	//一次性抽多张卡
	public void DrawCards(int amount)
	{
		// 如果上一次抽卡动画还在跑，先停掉
		if (drawTween != null && drawTween.IsRunning())
		{
			drawTween.Kill();
		}

		drawTween = CreateTween();

		for (int i = 0; i < amount; i++)
		{
			drawTween.TweenCallback(Callable.From(DrawCard));
			drawTween.TweenInterval(HandDrawInterval);
		}

		drawTween.Finished += () =>
		{
			events.instance.EmitSignal(events.SignalName.PlayerHandDrawn);
		};

	}
	public void OnStatusesApplied(status.Type type)
	{
		switch (type)
		{
			case status.Type.START_OF_TURN:
				DrawCards(character.cards_per_turn);//抽角色的回合抽卡数
				break;
			case status.Type.END_OF_TURN:
				DiscardCards();
				break;
		}
	}

	public void OnRelicsActivated(Relic.Type type)
	{
		
		switch (type)
		{
			case Relic.Type.START_OF_TURN:
				Player.status_handler.ApplyStatusesByType(status.Type.START_OF_TURN);
				break;
			case Relic.Type.END_OF_TURN:
				Player.status_handler.ApplyStatusesByType(status.Type.END_OF_TURN);
				break;
		}
	}


}
