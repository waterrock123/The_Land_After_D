using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
[GlobalClass]
public partial class Cardpile : Resource
{
    [Signal]
    public delegate void CardPileSizeChangedEventHandler(int cards_amount);
    [Export]
    public Godot.Collections.Array<Card> cards { get; set; } = new Godot.Collections.Array<Card>();

    //牌堆
    public bool empty()
    {
        return cards.Count == 0;
    }

    public Card draw_card()//抽牌
    {
        // 取出第一张牌
        Card card = cards[0];
        cards.RemoveAt(0);

        // 发射信号，传当前牌堆数量
        EmitSignal(SignalName.CardPileSizeChanged, cards.Count);

        return card;
    }
    public void add_card(Card card)
    {
        cards.Add(card);
        EmitSignal(SignalName.CardPileSizeChanged, cards.Count);
    }
    public void remove_card(Card card)
    {
        cards.Remove(card);
        EmitSignal(SignalName.CardPileSizeChanged, cards.Count);
    }
    public void shuffle()
    {
        rng.ArrayShuffle(cards);
    }
    public void clear()
    {
        cards.Clear();
        EmitSignal(SignalName.CardPileSizeChanged, cards.Count);
    }

    //因为godot引擎在深拷贝方面的bug所以在此手动拷贝一下

    public Array<Card> DuplicateCards()
    {
        var new_array = new Array<Card>();
        foreach (Card card in cards)
        {
            new_array.Add((Card)card.Duplicate());
        }
        return new_array;
    }

    public Cardpile CustomDuplicate()
    {
        var new_card_pile = new Cardpile();
        new_card_pile.cards = DuplicateCards();

        return new_card_pile;
    }


    public String _to_string()
    {
        Godot.Collections.Array<string> cardStrings = new Godot.Collections.Array<string>();

        for (int i = 0; i < cards.Count; i++)
        {
            cardStrings.Add($"{i + 1}: {cards[i].id}");
        }

        return string.Join("\n", cardStrings);
    }
}
