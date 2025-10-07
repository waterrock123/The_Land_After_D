 using Godot;
using Godot.NativeInterop;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class CardUi : Control
{
	[Signal]
	public delegate void ReparentRequestedEventHandler(CardUi which_card_ui);
	public Card_state_machine card_state_machine;
	public Card card;
	[Export]
	public Card import_card
	{
		get => card;
		set
		{
			_set_card(value);
		}
	}
	public CharaStats char_stats;
	[Export]
	public CharaStats import_char_stats
	{
		get => char_stats;
		set
		{
			SetCharStats(value);
		}
	}
	// 子节点
	public CardVisuals cardvisuals;
	public StyleBox BASE_STYLEBOX = (StyleBox)ResourceLoader.Load("res://scene/card_ui/card_base_stylebox.tres");
	public StyleBox DRAG_STYLEBOX = (StyleBox)ResourceLoader.Load("res://scene/card_ui/card_dragging_stylebox.tres");
	public StyleBox HOVER_STYLEBOX = (StyleBox)ResourceLoader.Load("res://scene/card_ui/card_hover_stylebox.tres");
	//以上是ui相关
	//下面是拖动区域
	public Area2D droppointdetec;
	public bool CanDrop=false;//是否可放入牌阵
	public int slot_index=-1;//追踪在牌阵中的位置
	public Godot.Collections.Array<Node> targets;
	public Control parent;
	public Tween tween;
	public int OriginalIndex=0;

	[Export]
	public ModifierHandler player_modifiers;//玩家数值调整器

	public bool Disabled { get; set; } = false;
	public bool _playable = true;
	public bool Playable
	{
    	get => _playable;
    	set => SetPlayable(value);
	}
	public override   void _Ready()
	{
		// 获取子节点
		cardvisuals = GetNode<CardVisuals>("CardVisuals");
		events.instance.CardAimStarted += OnCardDragOrAimingStarted;
    	events.instance.CardDragStarted += OnCardDragOrAimingStarted;
    	events.instance.CardDragEnded += OnCardDragOrAimEnded;
    	events.instance.CardAimEnded += OnCardDragOrAimEnded;

        _set_card(card);
		card_state_machine = GetNode<Card_state_machine>("CardStateMachine");
		droppointdetec = GetNode<Area2D>("DropPointDetector");
		card_state_machine.init(this);
		targets = new Godot.Collections.Array<Node>();

	}
	public override void _ExitTree()
    {
        // 解绑，防止重载/切换时回调旧实例
        
        events.instance.CardAimStarted -= OnCardDragOrAimingStarted;
    	events.instance.CardDragStarted -= OnCardDragOrAimingStarted;
    	events.instance.CardDragEnded -= OnCardDragOrAimEnded;
    	events.instance.CardAimEnded -= OnCardDragOrAimEnded;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var enemy_modifiers = GetActiveEnemyModifiers();
		var updated_tooltip = card.GetUpdatedTooltip(player_modifiers, enemy_modifiers);
		//测试
		cardvisuals.tooltip.Text = updated_tooltip;
	}
	
	public ModifierHandler GetActiveEnemyModifiers()
	{
		if (targets.Count == 0 || targets.Count > 1 || !(targets[0] is Enemy))
		{
			return null;
		}

		return (targets[0] as Enemy).modifier_handler;
	}
	public void RequestTooltip()
	{
		var enemy_modifiers = GetActiveEnemyModifiers();
		var updated_tooltip = card.GetUpdatedTooltip(player_modifiers, enemy_modifiers);
		//测试
		cardvisuals.tooltip.Text = updated_tooltip;

		events.instance.EmitSignal(events.SignalName.CardTooltipRequested, card.icon,updated_tooltip);
	}

	public override void _Input(InputEvent @event)
	{
		card_state_machine.on_input(@event);

	}
	public override void _GuiInput(InputEvent @event)
	{
		card_state_machine.on_gui_input(@event);
	}
	public void _OnMouseEntered()
	{
		card_state_machine.on_mouse_entered();
	}
	public void _OnMouseExited()
	{
		card_state_machine.on_mouse_exited();
	}
	public void _AreaEntered(Area2D area)
	{
		if (!targets.Contains(area))
		{
			targets.Add(area);
		}
	}
	public void _AreaExited(Area2D area)
	{
		targets.Remove(area);
	}
	public async Task _set_card(Card value)
	{
		if (!IsNodeReady())
		{
			await ToSignal(this, "ready");
		}

		card = value;
		cardvisuals.importcard = card;
	}
	public void SetPlayable(bool value)
	{
		_playable = value;
		if (!IsInstanceValid(cardvisuals.cost) || !IsInstanceValid(cardvisuals.icon))
        return;
		if (!_playable)
		{
			cardvisuals.cost.AddThemeColorOverride("font_color", Colors.Red);
			cardvisuals.icon.Modulate = new Color(1, 1, 1, 0.5f);
		}
		else
		{
			cardvisuals.cost.RemoveThemeColorOverride("font_color");
			cardvisuals.icon.Modulate = new Color(1, 1, 1, 1);
		}
		
	}
	//这里有可能有错
	public void SetCharStats(CharaStats value)
	{
		char_stats = value;
		char_stats.StatsChanged += OnCharStatsChanged;
	}
	public void OnCardDragOrAimingStarted(CardUi used_card)
	{
		if (used_card == this)
		{
			return;
		}
		Disabled = true;
	}
	public void OnCardDragOrAimEnded(CardUi _card)
	{
		Disabled = false;
		this.Playable = char_stats.CanPlayCard(card);
	}
	public void OnCharStatsChanged()
	{
		this.Playable = char_stats.CanPlayCard(card);
	}
	public void play()
	{
		if (card == null)
		{
			return;
		}
		card.play(targets, char_stats,player_modifiers);
		if (char_stats != null)
        char_stats.StatsChanged -= OnCharStatsChanged;
		QueueFree();
	}

	public void animate_to_position(Vector2 new_position, float duration)
	{
		tween = CreateTween().SetTrans(Tween.TransitionType.Circ).SetEase(Tween.EaseType.Out);
		tween.TweenProperty(this, "global_position", new_position, duration);
	}
}
