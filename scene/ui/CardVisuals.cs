using Godot;
using System;
using System.Threading.Tasks;

public partial class CardVisuals : Control
{
	public Card card;
	[Export]
	public Card importcard {
		get => card;
		set => SetCard(value);
	}
	public Panel panel;
	public Label cost;
	public TextureRect icon;
	public TextureRect rarity;
	public override void _Ready()
	{
		panel = GetNode<Panel>("Panel");
		cost = GetNode<Label>("Cost");
		icon = GetNode<TextureRect>("Icon");
		rarity = GetNode<TextureRect>("Rarity");
	}
	public async Task SetCard(Card value)
	{
		if (!IsNodeReady())
		{
			await ToSignal(this, "ready");
		}
		card = value;
		icon.Texture = (Texture2D)card.icon;
		cost.Text = card.cost.ToString();
		rarity.Modulate = Card.RarityColors[card.rarity];
	}
	
	

}
