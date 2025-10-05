using Godot;
using System;

public partial class CharacterSelector : Control
{
	//预载入资源
	public static PackedScene RUN_SCENE = (PackedScene)ResourceLoader.Load("res://scene/run/run.tscn");
	public CharaStats ASSASSIN_STATS;
	public CharaStats WARRIOR_STATS;
	public CharaStats WIZARD_STATS;
	public Label title;
	public Label description;
	public TextureRect character_portrait;
	public CharaStats current_character;

	public CharaStats import_current_character
	{
		get => current_character;
		set => SetCurrentCharacter(value);
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ASSASSIN_STATS = (CharaStats)ResourceLoader.Load("res://characters/Assassin/assassin.tres");
		WARRIOR_STATS = (CharaStats)ResourceLoader.Load("res://characters/warrior/warrior.tres");
		WIZARD_STATS = (CharaStats)ResourceLoader.Load("res://characters/Wizard/wizard.tres");
		title = GetNode<Label>("%Title");
		description = GetNode<Label>("%Description");
		character_portrait = GetNode<TextureRect>("%CharacterPortrait");
		SetCurrentCharacter(WARRIOR_STATS);


	}

	public void SetCurrentCharacter(CharaStats new_character)
	{
		current_character = new_character;
		title.Text = current_character.character_name;
		description.Text = current_character.description;
		character_portrait.Texture = (Texture2D)current_character.portrait;
	}
	public void OnStartButtonPressed()
	{
		RunStartup.Instance.type = RunStartup.Type.NEW_RUN;
		RunStartup.Instance.picked_character = current_character;
		GetTree().ChangeSceneToPacked(RUN_SCENE);

	}
	public void OnWarriorButtonPressed()
	{
		import_current_character = WARRIOR_STATS;

	}
	public void OnWizardButtonPressed()
	{
		import_current_character = WIZARD_STATS;
	}
	public void OnAssassinButtonPressed()
	{
		import_current_character = ASSASSIN_STATS;
	}
}
