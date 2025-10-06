using Godot;
using System;

public partial class Tooltip : PanelContainer
{
	[Export]
	double FadeSeconds = 0.2;
	public TextureRect tooltip_icon;
	public RichTextLabel tooltip_text_label;
	public Tween tween;
	public bool is_visible = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

		tooltip_icon = GetNode<TextureRect>("%TooltipIcon");
		tooltip_text_label = GetNode<RichTextLabel>("%TooltipText");
		events.instance.CardTooltipRequested += OnCardTooltipRequested;
		events.instance.TooltipHideRequested += HideTooltip;
		Modulate = Colors.Transparent;
		Hide();
	}
	public override void _ExitTree()
	{
		if (events.instance != null)
		{
			events.instance.CardTooltipRequested -= OnCardTooltipRequested;
			events.instance.TooltipHideRequested -= HideTooltip;
		}
		if (tween != null && tween.IsRunning())
        tween.Kill();
	}
	//中转函数
	private void OnCardTooltipRequested(Texture icon,string tooltip_text)
	{
		//ShowTooltip(icon, tooltip_text);
	}
	public void ShowTooltip(Texture icon, String text)
	{
		is_visible = true;
		if (tween != null)
		{
			tween.Kill();
		}
		tooltip_icon.Texture = (Texture2D)icon;
		tooltip_text_label.Text = text;
		tween = CreateTween().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
		tween.TweenCallback(Callable.From(Show));
		tween.TweenProperty(this, "modulate", Colors.White, FadeSeconds);

	}
	public void HideTooltip()
	{
		is_visible = false;
		if (tween != null)
		{
			tween.Kill();
		}
		GetTree().CreateTimer(FadeSeconds, false).Timeout += () => {
    if (IsInstanceValid(this))
        HideAnimation();
};
	}
	public void HideAnimation()
	{
		if (!is_visible)
		{
			tween = CreateTween().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
			tween.TweenProperty(this, "modulate", Colors.Transparent, FadeSeconds);
			tween.TweenCallback(Callable.From(Hide));
		}
		
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
