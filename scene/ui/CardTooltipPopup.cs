using Godot;
using System;

public partial class CardTooltipPopup : Control
{

	public static PackedScene CARD_MENU_UI_SCENE = (PackedScene)ResourceLoader.Load("res://scene/ui/card_menu_ui.tscn");
	
	public CenterContainer tooltip_card;
	public RichTextLabel card_description;
	[Export]
	public Color background_color = new Color("000000b0");
	public ColorRect background;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		background = GetNode<ColorRect>("Background");
		tooltip_card = GetNode<CenterContainer>("%TooltipCard");
		card_description = GetNode<RichTextLabel>("%CardDescription");
		foreach (CardMenuUi card in tooltip_card.GetChildren())
		{
			card.QueueFree();
		}
		background.Color = background_color;
		//hide_tooltip();
		//var timer = GetTree().CreateTimer(3.0f);
		//timer.Timeout += () =>
		//{
		//show_tooltip((Card)ResourceLoader.Load("res://characters/warrior/cards/warrior_block.tres"));
		//};

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void OnGuiInput(InputEvent @event)
	{
		if (@event.IsActionPressed("鼠标左键"))
		{
			hide_tooltip();
		}

	}
	public void show_tooltip(Card card)
	{
		var new_card = CARD_MENU_UI_SCENE.Instantiate() as CardMenuUi;
		tooltip_card.AddChild(new_card);
		new_card.importcard = card;
		new_card.TooltipRequested += (_)=>hide_tooltip();
		card_description.Text = card.GetDefaultTooltip();
		Show();
	}
	public void hide_tooltip()
	{
		if (!Visible)
		{
			return;
		}
		foreach (CardMenuUi card in tooltip_card.GetChildren())
		{
			card.QueueFree();
		}
		Hide();
	}
}
