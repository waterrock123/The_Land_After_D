using Godot;
using System;
using System.Security.Cryptography.X509Certificates;

[GlobalClass]
public partial class Relic : Resource
{
    public enum Type
    {
        START_OF_TURN,
        START_OF_COMBAT,
        END_OF_TURN,
        END_OF_COMBAT,
        EVENT_BASED,
    }

    public enum CharacterType//角色分类
    {
        ALL,
        ASSASSIN,
        WARRIOR,
        WIZARD
    }

    [Export]
    public string relic_name;
    [Export]
    public string id;
    [Export]
    public Type type;
    [Export]
    public CharacterType character_type;
    [Export]
    public bool starter_relic = false;
    [Export]
    public Texture icon;
    [Export(PropertyHint.MultilineText)]
    public string tooltip;


    public virtual void InitializeRelic(RelicUi owner)
    {

    }

    public virtual void ActivateRelic(RelicUi owner)
    {

    }

    //这个方法是用于停用遗物的,确保遗物的效果无效
    public virtual void DeactivateRelic(RelicUi owner)
    {

    }

    public virtual string GetTooltip()
    {
        return tooltip;
    }



    public bool CanApperAsReward(CharaStats character)
    {
        if (starter_relic)
        {
            return false;
        }
        if (character_type == CharacterType.ALL)
        {
            return true;
        }
        string relicCharName = Enum.GetName(typeof(CharacterType), character_type)?.ToLower();
        var char_name = character.character_name.ToLower();
        return relicCharName == char_name;
    }





}
