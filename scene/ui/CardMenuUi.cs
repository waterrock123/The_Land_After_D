using Godot;
using System;
using System.Threading.Tasks;

public partial class CardMenuUi : CenterContainer
{
	[Signal]
	public delegate void TooltipRequestedEventHandler(Card card);
	public static StyleBox BASE_STYLEBOX = (StyleBox)ResourceLoader.Load("res://scene/card_ui/card_base_stylebox.tres");
	public static StyleBox HOVER_STYLEBOX = (StyleBox)ResourceLoader.Load("res://scene/card_ui/card_hover_stylebox.tres");
	public Card card;
	[Export]
	public Card importcard
	{
		get => card;
		set => SetCard(value);
	}
	public CardVisuals visuals;
	public override void _Ready()
	{
		visuals = GetNode<CardVisuals>("Visuals");
	}

	public void OnMouseEntered()
	{
		visuals.panel.Set("theme_override_styles/panel", HOVER_STYLEBOX);
	}
	public void OnMouseExited()
	{
		visuals.panel.Set("theme_override_styles/panel", BASE_STYLEBOX);
	}
	public void OnGuiInput(InputEvent @event)
	{
		if (@event.IsActionPressed("鼠标左键"))
		{
			EmitSignal(CardMenuUi.SignalName.TooltipRequested, card);
		}
	}
	public async Task SetCard(Card value)
	{
		if (!IsNodeReady())
		{
			await ToSignal(this, "ready");
		}
		card = value;
		visuals.importcard = card;
		
	}
	
}
