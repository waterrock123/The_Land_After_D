using Godot;
using System;

public partial class battle_ui : CanvasLayer
{
	public CharaStats char_stats;
	[Export]
	public CharaStats import_stats
	{
		get => char_stats;
		set => SetCharStats(value);
	}
	public hand Hand;
	public ManaUi manaUi;
	public Button end_turn_button;
	public Button motivate_button;
	public CardPileOpener draw_pile_button;
	public CardPileOpener discard_pile_button;
	public CardPileView draw_pile_view;
	public CardPileView discard_pile_view;
	public MotivateCountUi motivate_ui;
	public SpreadUi spread;



	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		draw_pile_button = GetNode<CardPileOpener>("%DrawPileButton");
		discard_pile_button = GetNode<CardPileOpener>("%DiscardPileButton");
		draw_pile_view = GetNode<CardPileView>("%DrawPileView");
		discard_pile_view = GetNode<CardPileView>("%DiscardPileView");
		Hand = GetNode<hand>("Hand");
		spread = GetNode<SpreadUi>("Spread");
		manaUi = GetNode<ManaUi>("ManaUI");
		motivate_ui = GetNode<MotivateCountUi>("MotivateUI");
		end_turn_button = GetNode<Button>("%EndTurnButton");
		end_turn_button.Disabled = true;
		motivate_button = GetNode<Button>("MotivateButton");
		motivate_button.Disabled = true;


		events.instance.PlayerHandDrawn += OnPlayerHandDrawn;//连接抽牌信号，抽牌完时执行这个函数
		events.instance.SpreadMotivateEnded += OnMotivateEnded;//连接牌阵激发完信号，牌阵激发完后执行再次激活激发按钮
		motivate_button.Pressed += OnMotivateButtonPressed;
		end_turn_button.Pressed += OnEndTurnButtonPressed;//为按钮连接函数
		draw_pile_button.Pressed += () => draw_pile_view.show_current_view("Draw Pile", true);
		discard_pile_button.Pressed += () => discard_pile_view.show_current_view("Discard Pile");
	}
	public override void _ExitTree()
	{
		if (events.instance != null)
		{

			events.instance.PlayerHandDrawn -= OnPlayerHandDrawn;
			end_turn_button.Pressed -= OnEndTurnButtonPressed;
			motivate_button.Pressed -= OnMotivateButtonPressed;
			events.instance.SpreadMotivateEnded -= OnMotivateEnded;


			
        // 其他解绑...
		}
	}
	public void initialize_card_pile_ui()
	{
		draw_pile_button.importcard_pile = char_stats.draw_pile;
		draw_pile_view.card_pile = char_stats.draw_pile;
		discard_pile_button.importcard_pile = char_stats.discard;
		discard_pile_view.card_pile = char_stats.discard;
	}
	public void OnPlayerHandDrawn()
	{
		end_turn_button.Disabled = false;//设置回合结束按钮可用
		motivate_button.Disabled = false;
	}
	public void OnEndTurnButtonPressed()
	{
		end_turn_button.Disabled = true;//设置回合结束按钮不可用,代表按了后的按钮不能再按直到下一回合开始抽牌
		motivate_button.Disabled = true;

		events.instance.EmitSignal(events.SignalName.PlayerTurnEnded);//传递回合结束信号
	}

	public void OnMotivateButtonPressed()
	{
		motivate_button.Disabled = true;
		events.instance.EmitSignal(events.SignalName.SpreadMotivate);
	}
	public void OnMotivateEnded()
	{
		if (char_stats.motivate_count > 0)
		{
			motivate_button.Disabled = false;
		}
		
	}

	public void SetCharStats(CharaStats value)
	{
		char_stats = value;
		manaUi.import_char_stats = char_stats;
		Hand.char_stats = char_stats;
		spread.character = char_stats;
		motivate_ui.import_char_stats = char_stats;
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
