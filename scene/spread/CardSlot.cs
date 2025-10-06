using Godot;
using System;

public partial class CardSlot : Control
{

    public static StyleBox BASE_STYLEBOX = (StyleBox)ResourceLoader.Load("res://scene/spread/slot_base_style_box_flat.tres");
    public static StyleBox NONE_STYLEBOX = (StyleBox)ResourceLoader.Load("res://scene/spread/slot_none_style_box_flat.tres");
    public static StyleBox APPEAR_STYLEBOX = (StyleBox)ResourceLoader.Load("res://scene/spread/slot_appear_style_box_flat.tres");
    public static StyleBox IMPORTANT_STYLEBOX=(StyleBox)ResourceLoader.Load("res://scene/spread/slot_important_style_box_flat.tres");
    public static PackedScene CARD_MENU_UI_SCENE = (PackedScene)ResourceLoader.Load("res://scene/ui/card_menu_ui.tscn");//加载card_menu_ui
    //是否为主位
    [Export]
    public bool IsMainSlot = false;
    [Export]
    public bool Islocked = false;//是否锁定
    [Export]
    public bool IsEmpty = true;

    public AspectRatioContainer card_container;
    public Panel panel;//牌位视觉表现
    


    public override void _Ready()
    {
        card_container = GetNode<AspectRatioContainer>("%CardContainer");
        panel = GetNode<Panel>("Panel");
        Update();

    }
    //更新cardSlot状态
    public void Update()
    {
        if (Islocked)
        {
            panel.Set("theme_override_styles/panel", NONE_STYLEBOX);
        }
        if (IsMainSlot)
        {
            panel.Set("theme_override_styles/panel", IMPORTANT_STYLEBOX);
        }
    }


    //往牌位内放置牌
    public void PlaceCard(Card card)
    {
        var place_cardui = CARD_MENU_UI_SCENE.Instantiate() as CardMenuUi;
        place_cardui.importcard = card;
        card_container.AddChild(place_cardui);
        IsEmpty = false;
    }
    
    
    
    // 获取当前卡槽的世界（全局）中心点（用于吸附目标）
    // 注意：Control 上使用 GetGlobalRect() 来获取全球坐标和大小
    public Vector2 GetCenterGlobal()
    {
        Rect2 g = GetGlobalRect();
        return g.Position + g.Size * 0.5f;
    }

    //撤回卡槽功能
    public void ClearSlot()
    {
        if (card_container.GetChildCount() > 0)
        {
            card_container.GetChild(0).QueueFree();
            
        }
        IsEmpty = true;
        Update();
    }





    // 拖放基础逻辑
    public bool CanDropData(Variant data)
    {
        return !Islocked && data.Obj is Card && IsEmpty;
    }

    public  void DropData(Variant data)
    {
        if (data.Obj is Card card)
            PlaceCard(card);
    }





}
