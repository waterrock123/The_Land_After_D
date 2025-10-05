using Godot;
using System;
using System.Threading.Tasks;

public partial class player : Node2D
{
	public CharaStats stats;
	public StatusHandler status_handler;
	public ModifierHandler modifier_handler;
	public Material WhiteSpriteMaterial = (ShaderMaterial)ResourceLoader.Load("res://art/white_sprite_material.tres");
	[Export]
	public CharaStats Stats
	{
		get => stats;
		set
		{
			SetCharacterStats(value);
		}
	}
	// Called when the node enters the scene tree for the first time.
	Sprite2D sprite_2d;
	Stats_ui stats_ui;
	public override void _Ready()
	{
		sprite_2d = GetNode<Sprite2D>("Sprite2D");
		stats_ui = GetNode<Stats_ui>("StatsUI");
		status_handler = GetNode<StatusHandler>("StatusHandler");
		modifier_handler = GetNode<ModifierHandler>("ModifierHandler");
		
		


	}
	private void SetCharacterStats(CharaStats value)
	{
		stats = (CharaStats)value;

		// 绑定信号
		stats.StatsChanged -= UpdateStats;
		stats.StatsChanged += UpdateStats;
		UpdatePlayer();
	}


	public async Task UpdatePlayer()
	{
		if (!(stats is CharaStats))
		{
			return;
		}
		if (!IsInsideTree())
		{
			await ToSignal(this, "ready");
		}
		sprite_2d.Texture = (Texture2D)stats.art;
		UpdateStats();
	}

	public override void _Process(double delta)
	{
	}
	public void UpdateStats()
	{

		stats_ui.update_stats(stats);
	}

	public void TakeDamage(int damage,Modifier.Type which_modifier)//受伤函数
	{
		if (stats.Health <= 0)
		{
			return;
		}
		sprite_2d.Material = WhiteSpriteMaterial;
		var modified_damage = modifier_handler.GetModifiedValue(damage, which_modifier);
		var tween = CreateTween();
		tween.TweenCallback(Callable.From(() => GetNode<shaker>("/root/Shaker").shake(this, 16f, 0.15f)));
		tween.TweenCallback(Callable.From(() => stats.TakeDamage(modified_damage)));
		tween.TweenInterval(0.17);
		tween.Finished += () =>
		{
			sprite_2d.Material = null;
			if (stats.Health <= 0)
			{
				events.instance.EmitSignal(events.SignalName.PlayerDied);
				QueueFree();
			}
		};

	}
	public override void _ExitTree()
	{
    // 假设stats是一个存储了信号源的变量
    	if (stats != null)
    	{
        	stats.StatsChanged -= UpdateStats; // 假设信号名为StatsChanged
    	}
   
	}
}
