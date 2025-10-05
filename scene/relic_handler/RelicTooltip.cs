using Godot;
using System;

public partial class RelicTooltip : Control
{
	public TextureRect relic_icon;
	public RichTextLabel relic_tooltip;
	public Button back_button;
	public override void _Ready()
	{
		relic_icon = GetNode<TextureRect>("%RelicIcon");
		relic_tooltip = GetNode<RichTextLabel>("%RelicTooltip");
		back_button = GetNode<Button>("%BackButton");
		back_button.Pressed += Hide;
		Hide();

	}
    public override void _Input(InputEvent @event)
    {
		if (@event.IsActionPressed("ui_cancel") && Visible)
		{
			Hide();
		}
    }

	public void ShowTooltip(Relic relic)
	{
		relic_icon.Texture = (Texture2D)relic.icon;
		relic_tooltip.Text = relic.GetTooltip();
		Show();

	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnGuiInput(InputEvent @event)
	{
		if (@event.IsActionPressed("鼠标左键"))
		{
			Hide();
		}
	}
}
