using Godot;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

public partial class GlodBloodEvent : EventRoom
{
    public PackedScene CARD_PILE_KILL_VIEW = GD.Load<PackedScene>("res://scene/ui/card_pile_kill_view.tscn");
    public static PackedScene EVENT_BUTTON = (PackedScene)GD.Load("res://scene/eventrooms/event_room_button.tscn");
    public int gold = rng.instance.RandiRange(65, 91);
    public int HPtogold = rng.instance.RandiRange(7, 9);
    public int HPtokillcard = rng.instance.RandiRange(3, 5);
    public VBoxContainer OptionContainer;
    public Label des_label;
    public EventRoomButton gold_button;
    public EventRoomButton supply_button;
    public EventRoomButton back_button;

    public override void _Ready()
    {
        gold_button = GetNode<EventRoomButton>("%GoldButton");
        supply_button = GetNode<EventRoomButton>("%SupplyButton");
        back_button = GetNode<EventRoomButton>("%BackButton");
        OptionContainer = GetNode<VBoxContainer>("%OptionContainer");
        des_label = GetNode<Label>("%DesLabel");
        gold_button.Text = string.Format(gold_button.Text, HPtogold, gold);
        supply_button.Text = string.Format(supply_button.Text, HPtokillcard);
        gold_button.IsEndButton = false;
        supply_button.IsEndButton = false;
        
    }

    public override async Task SetUp()
    {
        if (!IsNodeReady())
        {
            await ToSignal(this, "ready");
        }
        gold_button.event_button_callback = new Callable(this, MethodName.BloodToGold);
        supply_button.event_button_callback = new Callable(this, MethodName.BloodToKillCard);
    }



    public void BloodToGold()
    {
        character_stats.TakeDamage(HPtogold);
        RunStats.importgold += gold;
        foreach (Button button in OptionContainer.GetChildren())
        {
            button.QueueFree();
        }
        var new_button = EVENT_BUTTON.Instantiate() as EventRoomButton;
        new_button.Text = "离开";
        OptionContainer.AddChild(new_button);
        des_label.Text = "对方小心地收起了血瓶，向你鞠躬道谢。";
    }

    public void BloodToKillCard()
    {
        character_stats.TakeDamage(HPtokillcard);
        var cardkill = CARD_PILE_KILL_VIEW.Instantiate() as CardPileKillView;
        cardkill.card_pile = character_stats.deck;
        AddChild(cardkill);
        foreach (Button button in OptionContainer.GetChildren())
        {
            button.QueueFree();
        }
        var new_button = EVENT_BUTTON.Instantiate() as EventRoomButton;
        new_button.Text = "离开";
        OptionContainer.AddChild(new_button);
        des_label.Text = "对方小心地收起了血瓶，向你鞠躬道谢。";

    }




}
