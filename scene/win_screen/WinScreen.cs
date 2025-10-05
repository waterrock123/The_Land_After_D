using Godot;
using System;

public partial class WinScreen : Control
{
	public static PackedScene MAIN_MENU_PATH = GD.Load<PackedScene>("res://scene/ui/main_menu.tscn");
	public static string MESSAGE = "{0} 是胜利者！";
	public CharaStats character;
	[Export]
	public CharaStats importcharacter
	{
		get => character;
		set => SetCharacter(value);
	}
	public Label message;
	public TextureRect character_portrait;

	public override void _Ready()
	{
		message = GetNode<Label>("%Message");
		character_portrait = GetNode<TextureRect>("%CharacterPortrait");


	}

	public void SetCharacter(CharaStats new_character)
	{
		character = new_character;
		message.Text = string.Format(MESSAGE, character.character_name.ToString());
		character_portrait.Texture = (Texture2D)character.portrait;
	}


	public override void _Process(double delta)
	{
	}

	public void OnMainMenuButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://scene/ui/main_menu.tscn");
		
	}


}
