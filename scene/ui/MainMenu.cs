using Godot;
using System;

public partial class MainMenu : Control
{
	public Button continue_button;
	public static readonly PackedScene  CHAR_SELECTOR_SCENE=(PackedScene)ResourceLoader.Load("res://scene/ui/character_selector.tscn");
	public static PackedScene RUN_SCENE = GD.Load<PackedScene>("res://scene/run/run.tscn");
	public override void _Ready()
	{
		continue_button = GetNode<Button>("%Continue");
		GetTree().Paused = false;
		if (SaveGame.LoadData() == null)
		{
			continue_button.Disabled=true;
		}
		

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void OnContinuePressed()
	{
		RunStartup.Instance.type = RunStartup.Type.CONTINUED_RUN;

		GetTree().ChangeSceneToPacked(RUN_SCENE);
		SaveGame.LoadData();

	}
	public void OnNewRunPressed()
	{
		RunStartup.Instance.type = RunStartup.Type.NEW_RUN;
		GetTree().ChangeSceneToPacked(CHAR_SELECTOR_SCENE);

	}
	public void OnExitPressed()
	{
		GetTree().Quit();
	}
}
