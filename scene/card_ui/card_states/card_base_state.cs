using Godot;
using System;
using System.Threading.Tasks;

public partial class card_base_state : card_state
{
	public bool mouse_over_card;
	public override void _Ready()
	{

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override async void Enter()
	{
		if (!cardUi.IsNodeReady())
		{
			await ToSignal(cardUi, "ready");
		}
		// if (cardUi.GetParent()?.Name == "CardContainer")
    	// {
        // GD.Print($"[card_base_state] Skip Reparent for card {cardUi.Name} because parent is {cardUi.GetParent()?.Name}");
        // return;
    	// }
		if (cardUi.tween != null && cardUi.tween.IsRunning())
		{
			cardUi.tween.Kill();
		}
		// 发出请求，把 card_ui 重新挂载到新的父节点
		cardUi.EmitSignal(CardUi.SignalName.ReparentRequested, cardUi);
		//GD.Print("测试一下有没有panel" + cardUi.panel);
		// 修改 UI 外观
		cardUi.cardvisuals.panel.AddThemeStyleboxOverride("panel", cardUi.BASE_STYLEBOX);
		cardUi.PivotOffset = Vector2.Zero;      // 复位 pivot
		events.instance.EmitSignal(events.SignalName.TooltipHideRequested);
	}

	public override void OnGuiInput(InputEvent @event)
	{	if ((!cardUi._playable) || cardUi.Disabled)
		{
			return;
		}
		if (@event.IsActionPressed("鼠标左键")&&mouse_over_card)
		{
			// 让 pivot 随鼠标点击位置偏移
			cardUi.PivotOffset = cardUi.GetGlobalMousePosition() - cardUi.GlobalPosition;

			// 发出状态转换请求：当前状态 -> CLICKED
			EmitSignal(SignalName.TransitionRequested, this, (long)card_state.State.CLICKED);
		}
	}

	public override void OnMouseEntered()
	{
		mouse_over_card = true;
		if ((!cardUi._playable) || cardUi.Disabled)
		{
			return;
		}
		cardUi.cardvisuals.panel.AddThemeStyleboxOverride("panel", cardUi.HOVER_STYLEBOX);
		cardUi.RequestTooltip();
		
	}
	public override void OnMouseExited()
	{
		mouse_over_card = false;
		if ((!cardUi._playable) || cardUi.Disabled)
		{
			return;
		}
		cardUi.cardvisuals.panel.AddThemeStyleboxOverride("panel", cardUi.BASE_STYLEBOX);
		events.instance.EmitSignal(events.SignalName.TooltipHideRequested);
    }

}
