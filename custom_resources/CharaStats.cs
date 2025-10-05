using Godot;
using System;
[GlobalClass]
public partial class CharaStats : Stats
{
    [Godot.ExportGroup("Visuals")]
    [Export]
    public string character_name;
    [Export(PropertyHint.MultilineText)]
    public string description;
    [Export]
    public Texture portrait;
    [Godot.ExportGroup("Gameplay Data")]
    [Export]//初始牌堆
    public Cardpile starting_deck;
    [Export]
    public Cardpile draftable_cards;//可选卡牌堆
    [Export]//每回合抽牌数
    public int cards_per_turn;
    [Export]//最大法力值
    public int max_mana;
    [Export]
    public Relic starting_relic;//初始遗物

    private int mana;
    public int MANA
    {
        get => mana;
        set
        {
            mana = Mathf.Clamp(value, -5, 9999);
            EmitSignal(SignalName.StatsChanged);
        }
    }
    [Export]
    public Cardpile deck;//当前牌组，主菜单里的
    public Cardpile discard;//弃牌堆
    public Cardpile draw_pile;//抽牌堆
    public void ResetMana()
    {
        this.MANA = max_mana;
    }
    public override void TakeDamage(int damage)
    {
        var initial_health = Health;
        base.TakeDamage(damage);
        if (initial_health > Health)
        {
            events.instance.EmitSignal(events.SignalName.PlayerHit);
        }
    }

    public bool CanPlayCard(Card card)
    {
        return mana >= card.cost;
    }
    public override Resource CreateInstance()
    {
        CharaStats instance = (CharaStats)Duplicate();
        instance.Health = MaxHealth;
        instance.Block = 0;
        instance.ResetMana();
        instance.deck =(Cardpile)instance.starting_deck.Duplicate();
        instance.draw_pile = new Cardpile();
        instance.discard = new Cardpile();


        return instance;
    } 
}
