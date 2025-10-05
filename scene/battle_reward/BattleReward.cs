using Godot;
using System;
using Godot.Collections;
using System.Linq;
public partial class BattleReward : Control
{
	
	public static PackedScene CARD_REWARDS = (PackedScene)ResourceLoader.Load("res://scene/ui/card_rewards.tscn");
	public static PackedScene REWARD_BUTTON = (PackedScene)ResourceLoader.Load("res://scene/ui/reward_button.tscn");
	public static Texture2D GOLD_ICON = (Texture2D)ResourceLoader.Load("res://art/gold.png");
	public string GOLD_TEXT = "{0} 金币";
	public static Texture2D CARD_ICON = (Texture2D)ResourceLoader.Load("res://art/rarity.png");
	public string CARD_TEXT = "添加一张新卡牌";
	[Export]
	public run_stats Run_Stats;
	[Export]
	public CharaStats character_stats;
	[Export]
	public RelicHandler relic_handler;
	public float card_reward_total_weight = 0.0f;
	public Dictionary<Card.Rarity, float> card_rarity_weights = new Dictionary<Card.Rarity, float>
    {
        { Card.Rarity.COMMON, 0.0f },
        { Card.Rarity.UNCOMMON, 0.0f },
        { Card.Rarity.RARE, 0.0f }
    };
	public VBoxContainer rewards;
	public override void _Ready()
	{
		rewards = GetNode<VBoxContainer>("%Rewards");
		foreach (Node node in rewards.GetChildren())
		{
			node.QueueFree();
		}

		//测试
		

	}

	public void OnButtonPressed()
	{
		events.instance.EmitSignal(events.SignalName.BattleRewardExited);
	}

	public void AddGoldReward(int amount)
	{
		var gold_reward = REWARD_BUTTON.Instantiate() as RewardButton;
		gold_reward.importreward_icon = GOLD_ICON;
		gold_reward.importreward_text = string.Format(GOLD_TEXT, amount);
		gold_reward.Pressed += () => { OnGoldRewardTaken(amount); };
		rewards.CallDeferred("add_child", gold_reward);

	}

	public void AddRelicReward(Relic relic)
	{
		if (relic == null)
		{
			return;
		}
		var relic_reward = REWARD_BUTTON.Instantiate() as RewardButton;
		relic_reward.importreward_icon = relic.icon;
		relic_reward.importreward_text = relic.relic_name;
		relic_reward.Pressed += () => OnRelicRewardTaken(relic);
		rewards.CallDeferred("add_child", relic_reward);


	}
	



	public void AddCardReward()
	{
		//添加卡牌奖励
		var card_reward = REWARD_BUTTON.Instantiate() as RewardButton;
		card_reward.importreward_icon = CARD_ICON;
		card_reward.importreward_text = CARD_TEXT;
		card_reward.iscard = true;
		card_reward.Pressed += () => ShowCardRewards(card_reward);
		rewards.CallDeferred("add_child", card_reward);
	}

	public void ShowCardRewards(RewardButton button)//展示卡牌奖励。3选1那种
	{
		if (Run_Stats == null || character_stats == null)
		{
			return;
		}
		if ( button.CachedRewards == null)
		{
		button.CachedRewards = new Array<Card>();//为单个按钮缓存卡组
		
		Array<Card> available_cards = character_stats.draftable_cards.DuplicateCards();//可用卡组,角色可选卡组的副本
		//启动循环
		for (int i = 0; i < Run_Stats.card_rewards; i++)//循环选卡次数
		{
			SetupCardChance();
			float roll = rng.instance.RandfRange(0.0f, card_reward_total_weight);

			foreach (Card.Rarity rarity in card_rarity_weights.Keys)
			{
				//依据权重挑选卡牌
				if (card_rarity_weights[rarity] > roll)
				{
					
					ModifyWeights(rarity);
					var picked_card = GetRandomAvailableCard(available_cards, rarity);
					button.CachedRewards.Add(picked_card);;//将匹配稀有度的卡添加到数组中 **注意用Add
					available_cards.Remove(picked_card);//并将其从可用卡牌中删除，防止重新出现卡牌
					break;


				}
			}

		}}
		var card_rewards = CARD_REWARDS.Instantiate() as CardRewards;
    	AddChild(card_rewards);
    	card_rewards.CardRewardSelected += (Card card) => ONCardRewardTaken(card, button);

    	card_rewards.importrewards = button.CachedRewards;
    	card_rewards.Show();
	}
	//设置卡牌概率
	public void SetupCardChance()
	{
		//设置随机加权分布
		card_reward_total_weight = (float)(Run_Stats.common_weight + Run_Stats.uncommon_weight + Run_Stats.rare_weight);
		card_rarity_weights[Card.Rarity.COMMON] = (float)Run_Stats.common_weight;
		card_rarity_weights[Card.Rarity.UNCOMMON] = (float)(Run_Stats.common_weight + Run_Stats.uncommon_weight);
		card_rarity_weights[Card.Rarity.RARE] = card_reward_total_weight;



	}
	public void ModifyWeights(Card.Rarity rarity_rolled)
	{
		if (rarity_rolled == Card.Rarity.RARE)
		{
			Run_Stats.rare_weight = run_stats.BASE_RARE_WEIGHT;
		}
		else
		{
			Run_Stats.rare_weight = Math.Clamp(Run_Stats.rare_weight + 0.3, run_stats.BASE_RARE_WEIGHT, 5.0);
		}
	}


	public Card GetRandomAvailableCard(Array<Card> available_cards, Card.Rarity with_rarity)
	{
		//稀有度过滤
		Array<Card> all_possible_cards=new Array<Card>();
		foreach (Card card in available_cards) {
			if (card.rarity == with_rarity)
			{
				all_possible_cards.Add(card);
			}
		}
		//随机选牌
		if (all_possible_cards.Count() > 0)
		{

			return (Card)rng.ArrayPickRandomCard(all_possible_cards);

		}
		return null;
	}

	public void ONCardRewardTaken(Card card, RewardButton button)
	{

		if (character_stats == null || card == null)
		{
			return;
		}
		
		character_stats.deck.add_card(card);
		
		button.QueueFree();
	}

	public void OnGoldRewardTaken(int amount)
	{
		if (Run_Stats == null)
		{
			return;
		}
		Run_Stats.importgold += amount;

	}
	public void OnRelicRewardTaken(Relic relic)
	{
		if (relic == null || relic_handler == null)
		{
			return;
		}
		relic_handler.AddRelic(relic);
	}

}
