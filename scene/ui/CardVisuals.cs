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
	public Label name;
	public Label cost;
	public TextureRect icon;
	public TextureRect rarity;
	public RichTextLabel tooltip;

	public override void _Ready()
	{
		panel = GetNode<Panel>("Panel");
		cost = GetNode<Label>("Cost");
		icon = GetNode<TextureRect>("%Icon");
		rarity = GetNode<TextureRect>("Rarity");
		name = GetNode<Label>("Name");
		tooltip = GetNode<RichTextLabel>("%Tooltip");
		

	}
	public async Task SetCard(Card value)
	{
		if (!IsInsideTree()) await ToSignal(this, "tree_entered");
    	if (!IsNodeReady()) await ToSignal(this, "ready");
    	card = value;

    	icon.Texture = (Texture2D)card.icon;
    	cost.Text = card.cost.ToString();
    	rarity.Modulate = Card.RarityColors[card.rarity];
    	name.Text = card.name;

   	 	await ToSignal(GetTree(), "process_frame");
    	tooltip.Visible = false;
    	tooltip.Text = card.GetDefaultTooltip();
    	tooltip.Visible = true;
    	tooltip.QueueRedraw();
		
		
	}
	
	

}
