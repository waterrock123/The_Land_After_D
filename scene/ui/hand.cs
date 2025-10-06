using Godot;
using System;

public partial class hand : HBoxContainer
{
	[Export]
	public player Player;
	[Export]
	public CharaStats char_stats;
	public PackedScene cardUi;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		cardUi = (PackedScene)ResourceLoader.Load("res://scene/card_ui/card_ui.tscn");
		//测试代码
		// var card = GD.Load<Card>("res://characters/warrior/cards/warrior_axe_attack.tres");
		// AddCard(card);
		
		
	}
	public void AddCard(Card card)
	{
		CardUi NewCardUI = (CardUi)cardUi.Instantiate();//创建一个卡的ui的实例
		AddChild(NewCardUI);//将其作为子节点加入
		NewCardUI.ReparentRequested += OnCardUiReparentRequested;//链接信号
		NewCardUI.import_card = card;//设置卡片属性
		NewCardUI.parent = this;//设置卡片的父节点
		NewCardUI.import_char_stats = char_stats;//连接人物状态，实现更新人物状态
		NewCardUI.player_modifiers = Player.modifier_handler;//传递玩家的修饰符管理

	}
	public override void _ExitTree()
{
    foreach (Node child in GetChildren())
    {
        if (IsInstanceValid(child))
            child.QueueFree();
    }
}
	public void DiscardCard(CardUi card)
	{
		card.QueueFree();
	}
	public void DisableHand()
	{
		foreach (CardUi card in GetChildren())
		{
			card.Disabled = true;
		}
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void OnCardUiReparentRequested(CardUi child)
	{
		
		child.Reparent(this);
		var NewIndex = Math.Clamp(child.OriginalIndex, 0, GetChildCount());
		CallDeferred("move_child", child, NewIndex);
		child.SetDeferred("disabled", false);
	}
}
