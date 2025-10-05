using Godot;
using Godot.Collections;
using System;
using System.Threading.Tasks;

public partial class MushElfEvent : EventRoom
{
    public EventRoomButton FoodButton;
    public EventRoomButton MushRelicButton;
    public VBoxContainer OptionContainer;
    public Label DesLabel;
    public static PackedScene CARD_REWARDS = (PackedScene)ResourceLoader.Load("res://scene/ui/card_rewards.tscn");
    public static PackedScene EVENT_BUTTON = (PackedScene)GD.Load("res://scene/eventrooms/event_room_button.tscn");
    public TextureRect description;
    [Export]
    public Relic relic;
    [Export]
    public Cardpile choosecards;

    public override async Task SetUp()
    {
        if (!IsNodeReady())
        {
            await ToSignal(this, "ready");
        }
        MushRelicButton.IsEndButton = false;
        FoodButton.IsEndButton = false;
        FoodButton.event_button_callback = new Callable(this, MethodName.FoodHeal);
        MushRelicButton.event_button_callback = new Callable(this, MethodName.MushElf);
    }


    public override void _Ready()
    {
        FoodButton = GetNode<EventRoomButton>("%FoodButton");
        MushRelicButton = GetNode<EventRoomButton>("%MushRelicButton");
        OptionContainer = GetNode<VBoxContainer>("%OptionContainer");
        description = GetNode<TextureRect>("%Description");
        DesLabel = GetNode<Label>("%DesLabel");
        
        
    }
    public void ONCardRewardTaken(Card card)
    {

        if (character_stats == null || card == null)
        {
            return;
        }

        character_stats.deck.add_card(card);


    }
    public void FoodHeal()
    {
        character_stats.Heal((int)Math.Ceiling(character_stats.MaxHealth * 0.2));
        var card_rewards = CARD_REWARDS.Instantiate() as CardRewards;
        var availablecards = choosecards.DuplicateCards();
        var cards = new Array<Card>();
        //待做，随机选卡
        for (int i = 0; i < RunStats.card_rewards; i++)//循环选卡次数
        {
            Card card = (Card)rng.ArrayPickRandomCard(availablecards);
            availablecards.Remove(card);
            cards.Add(card);


        }
        AddChild(card_rewards);
        card_rewards.CardRewardSelected += ONCardRewardTaken;

        card_rewards.importrewards = cards;
        card_rewards.Show();
        foreach (Button button in OptionContainer.GetChildren())
        {
            button.QueueFree();
        }

        var new_button = EVENT_BUTTON.Instantiate() as EventRoomButton;
        new_button.Text = "该启程了";
        OptionContainer.AddChild(new_button);
        DesLabel.Text = "你在寂静安宁的森林中美美地休息了一会儿";

    }

    public void MushElf()
    {
        relic_handler.AddRelic(relic);
        
        foreach (Button button in OptionContainer.GetChildren())
        {
            button.QueueFree();
        }

        var new_button = EVENT_BUTTON.Instantiate() as EventRoomButton;
        new_button.Text = "带上这只蘑菇离开";
        OptionContainer.AddChild(new_button);
        DesLabel.Text = "你在树林深处找到了一具尸体，尸体旁的笔记上写着一些关于蘑菇的研究记录。你收好了笔记，回头时便发现有一只会动的蘑菇跟着你。";
        description.Texture = GD.Load<Texture2D>("res://art/蘑菇精灵.png");

        
    }



}
