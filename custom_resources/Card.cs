using Godot;
using System;
using Godot.Collections;
using System.Linq.Expressions;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
[GlobalClass]
public partial class Card : Resource
{

    [ExportGroup("Card Visuals")]//卡牌视觉效果
    [Export]
    public Texture icon { get; set; }
    [Export(PropertyHint.MultilineText)]
    public string tooltip_text { get; set; }
    [Export(PropertyHint.MultilineText)]
    public string description_text { get; set; }
    [Export]
    public AudioStream sound { get; set; }
    [Export]
    public string name;

    public enum Type
    {
        Attack,
        Skill,
        Power
        //如果是我的游戏的话我可能会暂定有攻击牌，技能牌，附魔牌
    }
    public enum Rarity
    {
        COMMON,
        UNCOMMON,
        RARE
    }
    public static readonly Dictionary<Card.Rarity, Color> RarityColors =
        new Dictionary<Card.Rarity, Color>
        {
            { Card.Rarity.COMMON, Colors.Gray },
            { Card.Rarity.UNCOMMON, Colors.CornflowerBlue },
            { Card.Rarity.RARE, Colors.Gold },
        };
    public enum Target
    {
        Self,
        Single_Enemy,
        All_Enemies,
        Everyone
    }
    [ExportGroup("Card Attributes")]
    [Export]
    public String id { get; set; }
    [Export]
    public Type type { get; set; }
    [Export]
    public Target target { get; set; }
    [Export]
    public int cost;
    [Export]
    public bool exhausts;
    [Export]
    public Rarity rarity;//稀有度
    public bool IsSingleTargeted()
    {
        return target == Target.Single_Enemy;
    }
    public Array<Node> GetTargets(Array<Node> targets)
    {
        if (target == null)
        {
            return [];
        }
        var tree = targets[0].GetTree();
        switch (target)
        {
            case Target.Self:
                return tree.GetNodesInGroup("player");
            case Target.All_Enemies:
                return tree.GetNodesInGroup("enemies");
            case Target.Everyone:
                return tree.GetNodesInGroup("enemies") + tree.GetNodesInGroup("player");
            default:
                return [];
        }
    }
    public void play(Array<Node> targets, CharaStats charstats, ModifierHandler modifiers)
    {
        events.instance.EmitSignal(events.SignalName.CardPlayed, this);
        // charstats.MANA -= cost;

        if (IsSingleTargeted())
        {
            apply_effects(targets, modifiers);
        }
        else
        {
            apply_effects(GetTargets(targets), modifiers);
        }
    }
    public virtual void apply_effects(Array<Node> _target, ModifierHandler modifiers)
    {

    }

    public virtual string GetDefaultTooltip()
    {
        return tooltip_text;
    }
    public virtual string GetDescription()
    {
        return description_text;
    }

    public virtual string GetUpdatedTooltip(ModifierHandler playermodifiers, ModifierHandler enemymodifiers)
    {
        return tooltip_text;
    }
}
