using Godot;
using Godot.Collections;
using System;

public partial class SpreadUi : Control
{
    public static PackedScene CardSlotScene = GD.Load<PackedScene>("res://scene/spread/card_slot.tscn");
    [Export] public Spread spread;
    
    public GridContainer card_slot_holder;

    public override void _Ready()
    {
        card_slot_holder = GetNode<GridContainer>("%CardSlotHolder");
        foreach (CardSlot cardSlot in card_slot_holder.GetChildren())
        {
            cardSlot.QueueFree();
        }
        GenerateSpread();
    }
   public void GenerateSpread()
    {


        for (int y = 1; y < 10; y++)
        {
            var card_slot = CardSlotScene.Instantiate() as CardSlot;
            if (spread.IsLockIndex.Contains(y))
            {
                card_slot.Islocked = true;
            }
            if (spread.MainSlotIndex.Contains(y))
            {
                card_slot.IsMainSlot = true;
            }
            card_slot_holder.AddChild(card_slot);
        }
    }

}
