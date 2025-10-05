using Godot;
using System;
using System.Threading.Tasks;
using Godot.Collections;

public partial class RewardButton : Button
{
	public bool iscard = false;//判断是否是卡牌奖励，用于区分自己
	public Texture reward_icon;
	public Array<Card> CachedRewards { get; set; } = null;
	[Export]
	public Texture importreward_icon
	{
		get => reward_icon;
		set => SetRewardIcon(value);
	}
	public String reward_text;
	[Export]
	public String importreward_text
	{
		get => reward_text;
		set => SetRewardText(value);
	}

	public TextureRect custom_icon;
	public Label custom_text;
	public override void _Ready()
	{
		custom_icon = GetNode<TextureRect>("%CustomIcon");
		custom_text = GetNode<Label>("%CustomText");


	}
	public async Task SetRewardIcon(Texture new_icon)
	{
		reward_icon = new_icon;

		if (!IsNodeReady())
		{
			await ToSignal(this, "ready");
		}
		custom_icon.Texture = (Texture2D)reward_icon;



	}

	public async Task SetRewardText(String new_text)
	{
		reward_text = new_text;

		if (!IsNodeReady())
		{
			await ToSignal(this, "ready");
		}
		custom_text.Text = reward_text.ToString();



	}

	public void Onpressed()
	{
		if (iscard)
		{
			return;
		}
		QueueFree();
		
	}
}
