using Godot;
using Godot.Collections;
using System;

public partial class SpreadUi : Control
{
    public static PackedScene CardSlotScene = GD.Load<PackedScene>("res://scene/spread/card_slot.tscn");
    [Export] public Spread spread;
    [Export]
    public  float SNAP_DISTANCE = 40f;

    [Export]
    public hand hand;
    public Array<CardSlot> slots = new Array<CardSlot>();
    public GridContainer card_slot_holder;
    public AnimationPlayer animate_player;
    private bool isExpanded = false;
    private bool isMouseNear = false;
    private CardUi draggedCard = null;//目前被拖动的卡牌
    CardSlot nearestSlot = null;
    // private Node originalParent = null;//卡牌的原父节点，一般是hand
    public CharaStats character;

    public double MotivateCardInterval = 0.2;

    public override void _Ready()
    {
        card_slot_holder = GetNode<GridContainer>("%CardSlotHolder");
        animate_player = GetNode<AnimationPlayer>("AnimationPlayer");
        foreach (CardSlot cardSlot in card_slot_holder.GetChildren())
        {
            cardSlot.QueueFree();
        }
        GenerateSpread();
        events.instance.SpreadMotivate += Motivate;
        events.instance.CardDragStarted += GetDraggingCard;
        events.instance.CardDragEnded += RemoveDraggingCard;
        events.instance.CardSlotRequested += OnCardSlotRequested;
        events.instance.CardSlotOut += OnCardSlotOut;

    }
    public override void _ExitTree()
    {
        events.instance.SpreadMotivate -= Motivate;
        events.instance.CardDragStarted -= GetDraggingCard;
        events.instance.CardDragEnded -= RemoveDraggingCard;
        events.instance.CardSlotRequested -= OnCardSlotRequested;
        events.instance.CardSlotOut -= OnCardSlotOut;
    }


    private void ToggleExpand()
    {
        if (isExpanded)
            Collapse();
        else
            Expand();
    }

    private void Expand()
    {
        if (animate_player.HasAnimation("expand"))
            animate_player.Play("expand");
        isExpanded = true;
    }

    private void Collapse()
    {
        if (animate_player.HasAnimation("shrink"))
            animate_player.Play("shrink");
        isExpanded = false;
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
            slots.Add(card_slot);
        }
    }
    public void OnCardSlotRequested()
    {
        if (nearestSlot == null)
        {
            GD.PrintErr("[SpreadUi] OnCardSlotRequested: nearestSlot 为空！");
            return;
        }
        if (draggedCard == null)
        {
            GD.PrintErr("[SpreadUi] OnCardSlotRequested: nearestSlot 为空！");
            return;
        }
        character.MANA -= draggedCard.card.cost;
        draggedCard.Reparent(nearestSlot.card_container);
        draggedCard.slot_index = nearestSlot.GetIndex();
        nearestSlot.IsEmpty = false;
        draggedCard.CanDrop = false;


    }
    public void OnCardSlotOut(CardUi cardUi)
    {
        if (cardUi == null)
        {
            GD.PrintErr("[SpreadUi] OnCardSlotRequested: cardui 为空！");
            return;
        }
        if (hand == null)
        {
            GD.Print("hand为空！");
            return;
        }


        character.MANA += cardUi.card.cost;
        var slot = card_slot_holder.GetChild(cardUi.slot_index) as CardSlot;
        cardUi.CanDrop = true;
        slot.IsEmpty = true;
    }
    //激发牌阵卡牌
    public void Motivate()
    {
        events.instance.CardSlotRequested -= OnCardSlotRequested;
        Tween tween = CreateTween();
        bool hasTweens = false;
        foreach (CardSlot slot in slots)
        {
            if (slot.Islocked||slot.IsEmpty)
            {
                continue;
            }
            hasTweens = true;
            tween.TweenCallback(Callable.From(() => slot.CardMotivate()));
            tween.TweenInterval(MotivateCardInterval);
        }
        if (!hasTweens)
        {
        // 没有可激发的卡槽，直接结束信号
        events.instance.EmitSignal(events.SignalName.SpreadMotivateEnded);
        tween.Kill();
        character.Motivate -= 1;
        return;
        }
        tween.Finished += () =>
        {
            character.Motivate -= 1;
            events.instance.CardSlotRequested += OnCardSlotRequested;
            events.instance.EmitSignal(events.SignalName.SpreadMotivateEnded);
		};
    }

    public override void _Process(double delta)
    {
        // 自动放大检测：例如鼠标靠近牌阵区域
        Vector2 mousePos = GetViewport().GetMousePosition();
        bool nowNear = GetGlobalRect().Grow(100).HasPoint(mousePos);

        if (nowNear && !isExpanded)
        {
            Expand();
            isMouseNear = true;
        }
        else if (!nowNear && isMouseNear && isExpanded)
        {
            Collapse();
            isMouseNear = false;
        }

        if (draggedCard != null)
        {
            ProcessDragging();
        }

    }
    //获取正在拖动的卡牌
    public void GetDraggingCard(CardUi cardUi)
    {
        draggedCard = cardUi;
    }
    public void RemoveDraggingCard(CardUi cardUi)
    {
        draggedCard = null;
        nearestSlot = null;
    }
    /// <summary>
    /// 主动吸附逻辑：检测是否有正在被拖动的卡牌，若靠近某个空的slot则自动吸附
    /// </summary>
    private void ProcessDragging()
    {
        var cardUi = draggedCard;
        if (cardUi == null) return;

        // 2. 找出最近的卡槽
        
        float nearestDistance = float.MaxValue;

        foreach (CardSlot slot in card_slot_holder.GetChildren())
        {
            if (slot.Islocked || !slot.IsEmpty) continue;

            float distance = cardUi.GlobalPosition.DistanceTo(slot.GlobalPosition);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestSlot = slot;
            }
        }

        // 3. 若距离小于吸附距离，则吸附
        if (nearestSlot != null && nearestDistance < SNAP_DISTANCE)
        {
            // 计算吸附目标位置，使卡居中
            Vector2 targetPos = nearestSlot.GlobalPosition;
            cardUi.CanDrop = true;


            // 使用线性插值让吸附更平滑
            cardUi.GlobalPosition = cardUi.GlobalPosition.Lerp(targetPos, 1f);
        }
        else
        {
            cardUi.CanDrop = false;
            nearestSlot = null;
        }
    

    }
    
    
    


}
