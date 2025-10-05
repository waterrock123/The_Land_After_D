using Godot;
using System;

public partial class IntentUi : HBoxContainer
{
	
	public TextureRect icon;
	public Label label;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		icon = GetNode<TextureRect>("Icon");
		label = GetNode<Label>("Label");
	}
	public void UpdateIntent(intent Intent)
	{
		if (Intent == null)
		{
			Hide();
			return;
		}
		
		icon.Texture = (Texture2D)Intent.icon;
		icon.Visible = icon.Texture != null;
		label.Text = Intent.current_text.ToString();
		label.Visible = Intent.current_text.Length> 0;
		Show();
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	
}
