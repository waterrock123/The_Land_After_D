using Godot;
using Godot.Collections;
using System;



public partial class events : Node
{
	public static events instance{ get; set; }

	//卡牌相关事件
	[Signal]
	public delegate void CardAimStartedEventHandler(CardUi card_ui);
	[Signal]
	public delegate void CardAimEndedEventHandler(CardUi card_ui);
	[Signal]
	public delegate void CardPlayedEventHandler(Card card);
	[Signal]
	public delegate void CardDragStartedEventHandler(CardUi card_ui);
	[Signal]
	public delegate void CardDragEndedEventHandler(CardUi card_ui);
	
	[Signal]
	public delegate void CardTooltipRequestedEventHandler(Texture icon,string tooltip_text);//请求提示
	[Signal]
	public delegate void TooltipHideRequestedEventHandler();//隐藏提示


	//牌阵相关事件
	[Signal]
	public delegate void SpreadMotivateEventHandler();//牌阵激发事件
	[Signal]
	public delegate void SpreadMotivateEndedEventHandler();//激发完毕
	[Signal]
	public delegate void CardSlotRequestedEventHandler();//	正在拖动的卡牌请求进入牌阵;
	[Signal]
	public delegate void CardSlotOutEventHandler(CardUi cardUi);

	//玩家相关事件
	[Signal]
	public delegate void PlayerHandDrawnEventHandler();//玩家抽牌
	[Signal]
	public delegate void PlayerHandDiscarderEventHandler();//玩家弃牌
	[Signal]
	public delegate void PlayerTurnEndedEventHandler();//玩家回合结束
	[Signal]
	public delegate void PlayerDiedEventHandler();//玩家死亡信号
	[Signal]
	public delegate void PlayerHitEventHandler();//玩家受伤信号

	//敌人相关
	[Signal]
	public delegate void EnemyActionCompletedEventHandler(Enemy enemy);
	[Signal]
	public delegate void EnemyTurnEndedEventHandler();
	[Signal]
	public delegate void EnemyDiedEventHandler(Enemy enemy);


	//战斗相关事件
	[Signal]
	public delegate void BattleOverScreenRequestedEventHandler(string text, BattleOverPanel.Type type);
	[Signal]
	public delegate void BattleWonEventHandler();//胜利信号
	[Signal]
	public delegate void StatusTooltipRequestedEventHandler(Array<status> statuses);


	//地图相关事件
	[Signal]
	public delegate void MapExitedEventHandler(Room room);//退出地图

	//商店相关事件
	[Signal]
	public delegate void ShopEnterdEventHandler(Shop shop);
	[Signal]
	public delegate void ShopExitedEventHandler();
	[Signal]
	public delegate void ShopCardBoughtEventHandler(Card card,int gold_cost);//购买卡牌
	[Signal]
	public delegate void ShopRelicBoughtEventHandler(Relic relic, int gold_cost);//购买遗物


	//篝火相关事件
	[Signal]
	public delegate void CampfireExitedEventHandler();

	//战斗奖励相关事件
	[Signal]
	public delegate void BattleRewardExitedEventHandler();

	//宝箱房事件
	[Signal]
	public delegate void TreasureRoomExitedEventHandler(Relic found_relic);

	//遗物相关事件
	[Signal]
	public delegate void RelicTooltipRequestedEventHandler(Relic relic);

	//随机事件相关
	[Signal]
	public delegate void EventRoomExitedEventHandler();

	public override void _Ready()
	{
		instance = this;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}


}
