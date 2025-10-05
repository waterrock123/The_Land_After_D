using Godot;
using System;

public partial class BattleOverPanel : Panel
{
	public Label label;
	public Button continue_button;
	public Button main_menu_button;

	public static string MAIN_MENU = "res://scene/ui/main_menu.tscn";

	public Type type;
	public enum Type
	{
		WIN,
		LOSE
	}
	public override void _Ready()
	{
		label = GetNode<Label>("%Label");
		continue_button = GetNode<Button>("%ContinueButton");
		main_menu_button = GetNode<Button>("%MainMenuButton");
		continue_button.Pressed +=OnContinueButtonPressed;
		main_menu_button.Pressed +=OnMainMenuButtonPressed;
		events.instance.BattleOverScreenRequested -= show_screen;
		events.instance.BattleOverScreenRequested += show_screen;
	}
	public override void _ExitTree()
	{


		events.instance.BattleOverScreenRequested -= show_screen;
		main_menu_button.Pressed -= OnMainMenuButtonPressed;
		continue_button.Pressed -=OnContinueButtonPressed;
		
		
		
        
	}
	public void OnContinueButtonPressed()
	{
		events.instance.EmitSignal(events.SignalName.BattleWon);
	}
	public void OnMainMenuButtonPressed()
	{
		GetTree().ChangeSceneToFile(MAIN_MENU);
	}
	public void show_screen(string text, Type type)
	{
		label.Text = text;
		continue_button.Visible = type == Type.WIN;
		main_menu_button.Visible = type == Type.LOSE;

		Show();
		GetTree().Paused = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
