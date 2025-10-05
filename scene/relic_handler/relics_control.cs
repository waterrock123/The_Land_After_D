using Godot;
using System;

public partial class relics_control : Control
{
	public float RELICS_PER_PAGE = 5;
	public float TWEEN_SCROLL_DURATION = 0.2f;


	[Export]
	public TextureButton left_button;
	[Export]
	public TextureButton right_button;

	public HBoxContainer relics;
	public float page_width;


	public int num_of_relics = 0;
	public int current_page = 1;
	public int max_page = 0;
	public Tween tween;


	public override void _Ready()
	{
		relics = GetNode<HBoxContainer>("%Relics");
		page_width = this.CustomMinimumSize.X;

		left_button.Pressed += OnLeftButtonPressed;
		right_button.Pressed += OnRightButtonPressed;
		foreach (RelicUi relic_ui in relics.GetChildren())
		{
			relic_ui.Free();
		}

		relics.ChildOrderChanged += OnRelicsChildOrderChanged;

		

	}

	public void Update()
	{
		if (!IsInstanceValid(left_button) || !IsInstanceValid(right_button))
		{
			return;
		}

		num_of_relics = relics.GetChildCount();
		max_page = (int)Math.Ceiling((num_of_relics / RELICS_PER_PAGE));
		left_button.Disabled = current_page <= 1;
		right_button.Disabled = current_page >= max_page;
	}

	public void TweenTo(float x_position)
	{
		if (tween != null)
		{
			tween.Kill();
		}
		tween = CreateTween().SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
		tween.TweenProperty(relics, "position:x", x_position, TWEEN_SCROLL_DURATION);

	}


	public void OnLeftButtonPressed()
	{
		if (current_page > 1)
		{
			current_page -= 1;
			Update();
			TweenTo(relics.Position.X + page_width+10);
		}

	}

	public void OnRightButtonPressed()
	{
		if (current_page < max_page)
		{
			current_page += 1;
			Update();
			TweenTo(relics.Position.X - page_width-10);
		}
	}

	public void OnRelicsChildOrderChanged()
	{
		Update();
	}


}
