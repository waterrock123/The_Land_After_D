using Godot;
using System;

public partial class CardPileKillView : Control
{
    public static PackedScene CARD_MENU_UI_SCENE = (PackedScene)ResourceLoader.Load("res://scene/ui/card_menu_ui.tscn");
    [Export]
    public Cardpile card_pile;
    public Label title;
    public GridContainer cards;
    public CardTooltipPopup card_tooltip_popup;
    public override void _Ready()
    {
        title = GetNode<Label>("%Title");
        cards = GetNode<GridContainer>("%Cards");
        card_tooltip_popup = GetNode<CardTooltipPopup>("%CardTooltipPopup");
        foreach (Node card in cards.GetChildren())
        {
            card.QueueFree();
        }
        card_tooltip_popup.hide_tooltip();
        show_current_view("选择一张卡牌移除",true);
        
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {

    }
    public void OnKillButtonPressed()
    {
        CardMenuUi card_menu_ui = (CardMenuUi)card_tooltip_popup.tooltip_card.GetChild(0);
        var card = card_menu_ui.card;
        card_pile.remove_card(card);
        QueueFree();
    }
    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_cancel"))
        {
            if (card_tooltip_popup.Visible)
            {
                card_tooltip_popup.hide_tooltip();
            }
            else
            {
                Hide();
            }
        }
    }
    public void show_current_view(string new_title, bool randomzied = false)
    {
        foreach (Node card in cards.GetChildren())
        {
            card.QueueFree();
        }
        card_tooltip_popup.hide_tooltip();
        title.Text = new_title;
        CallDeferred(nameof(_update_view), randomzied);

    }
    public void _update_view(bool randomzied)
    {
        if (card_pile == null)
        {
            return;
        }
        var all_cards = card_pile.cards.Duplicate();
        if (randomzied)
        {
            rng.ArrayShuffle(all_cards);
        }
        foreach (Card card in all_cards)
        {
            var new_card = CARD_MENU_UI_SCENE.Instantiate() as CardMenuUi;
            cards.AddChild(new_card);
            new_card.importcard = card;
            new_card.TooltipRequested += (card) => card_tooltip_popup.show_tooltip(card);

        }
        Show();
    }
    
}
