using Godot;
using System;

public partial class CardPileOpener : TextureButton
{

	[Export]
	public Label counter;
	public Cardpile card_pile;
	[Export]
	public Cardpile importcard_pile
	{
		get => card_pile;
		set => SetCardPile(value);
	}
	public void SetCardPile(Cardpile new_value)
	{
		GD.Print("SetCardPile called with: " + (new_value == null ? "NULL" : new_value.ToString()));
		card_pile = new_value; 
		card_pile.CardPileSizeChanged += OnCardPileSizeChanged;
		OnCardPileSizeChanged(card_pile.cards.Count);
		
	}
	public void OnCardPileSizeChanged(int cards_amount)
	{
		counter.Text = cards_amount.ToString();
	}
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
