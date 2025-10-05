using Godot;
using System;
using System.Diagnostics;

public partial class sound_player : Node
{
	// Called when the node enters the scene tree for the first time.
	public static sound_player instance{ get; set; }
	public override void _Ready()
	{
		instance = this;
	}
	public void play(AudioStream audio, bool single = false)
	{
		if (audio == null)
		{
			return;
		}
		if (single)
		{
			stop();
		}
		foreach (var child in GetChildren())
		{
			if (child is AudioStreamPlayer player)
			{
				if (!player.Playing)
				{
					player.Stream = audio;
					player.Play();
				break;
				}	
			}
			
			
		}
		
	}
	public void stop()
	{
		foreach (var child in GetChildren())
		{
			if (child is AudioStreamPlayer player)
			{
				player.Stop();
			}
			
			
		}
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
