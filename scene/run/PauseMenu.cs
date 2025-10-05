using Godot;
using System;

public partial class PauseMenu : CanvasLayer
{
	[Signal]
	public delegate void SaveAndQuitEventHandler();

	public Button BackToGameButton;
	public Button SaveAndQuitButton;
	public override void _Ready()
	{
		BackToGameButton = GetNode<Button>("%BackToGameButton");
		SaveAndQuitButton = GetNode<Button>("%SaveAndQuitButton");
		BackToGameButton.Pressed += Unpause;
		SaveAndQuitButton.Pressed += OnSaveAndQuitButtonPressed;
	}
    public override void _ExitTree()
    {
        BackToGameButton.Pressed -= Unpause;
		SaveAndQuitButton.Pressed -= OnSaveAndQuitButtonPressed;
    }


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("pause"))
		{
			if (Visible)
			{
				Unpause();
			}
			else
			{
				Pause();
			}
			GetViewport().SetInputAsHandled();
		}
	}

	public void Pause()
	{
		Show();
		GetTree().Paused = true;
	}

	public void Unpause()
	{
		Hide();
		GetTree().Paused = false;
	}

	public void OnSaveAndQuitButtonPressed()
	{
		GetTree().Paused = false;
		EmitSignal(SignalName.SaveAndQuit);
	}


}
