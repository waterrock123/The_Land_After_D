using Godot;
using System;
using System.Threading.Tasks;
public partial class Enemy : Area2D
{
	// Called when the node enters the scene tree for the first time.
	const int ArrowOffet = 50;
	public IntentUi intent_ui;
	public StatusHandler status_handler;//状态管理
	public ModifierHandler modifier_handler;//数值修改
	public Material WhiteSpriteMaterial = (ShaderMaterial)ResourceLoader.Load("res://art/white_sprite_material.tres");
	
	public EnemyStats stats;
	[Export]
	public EnemyStats EnemyStats
	{
		get => stats;
		set
		{
			SetEnemyStats(value);
		}
	}
	Sprite2D sprite_2d;
	Sprite2D arrow;
	Stats_ui stats_ui;
	public EnemyActionPicker enemy_action_picker;
	public EnemyAction current_action;
	public EnemyAction importcurrent_action
	{
		get => current_action;
		set
		{
			SetCurrentAction(value);
		}
	}
	public void SetCurrentAction(EnemyAction value)
	{
		current_action = value;
		UpdateIntent();
	}
	public override void _Ready()
	{
		sprite_2d = GetNode<Sprite2D>("Sprite2D");
		arrow = GetNode<Sprite2D>("Arrow");
		arrow.Visible = false;
		stats_ui = GetNode<Stats_ui>("StatsUI");
		intent_ui = GetNode<IntentUi>("IntentUI");
		status_handler = GetNode<StatusHandler>("StatusHandler");
		modifier_handler = GetNode<ModifierHandler>("ModifierHandler");


	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void SetEnemyStats(EnemyStats value)
	{
		stats = (EnemyStats)value.CreateInstance();
		stats.StatsChanged -= UpdateStats;
		stats.StatsChanged += UpdateStats;
		stats.StatsChanged += UpdateAction;
		UpdateEnemy();



	}
	public void SetupAi()
	{
		if (enemy_action_picker != null)
		{
			enemy_action_picker.QueueFree();
		}
		EnemyActionPicker new_action_picker = (EnemyActionPicker)stats.ai.Instantiate();
		AddChild(new_action_picker);
		enemy_action_picker = new_action_picker;
		enemy_action_picker.importenemy = this;
	}

	public void UpdateAction()
	{
		if (enemy_action_picker == null)
		{
			return;
		}
		if (current_action == null)
		{
			importcurrent_action = enemy_action_picker.GetAction();
			return;
		}
		var new_conditional_acton = enemy_action_picker.GetFirstConditionalACtion();
		if (new_conditional_acton != null && current_action != new_conditional_acton)
		{
			importcurrent_action = new_conditional_acton;
		}
	}
	public void UpdateStats()
	{
		stats_ui.update_stats(stats);
	}
	public async Task UpdateEnemy()
	{
		if (!(stats is Stats))
		{
			return;
		}
		if (!IsInsideTree())
		{
			await ToSignal(this, "ready");
		}
		sprite_2d.Texture = (Texture2D)stats.art;
		arrow.Position = Vector2.Up * (sprite_2d.GetRect().Size.X / 2 + ArrowOffet);
		SetupAi();

		UpdateStats();
	}
	public void UpdateIntent()
	{
		if (current_action != null)
		{
			current_action.UpdateIntentText();//更新意图文本
			intent_ui.UpdateIntent(current_action.Intent);//更新意图
		}
	}



	//敌人回合管理
	public void DoTurn()
	{
		stats.Block = 0;
		if (current_action == null)
		{
			return;
		}
		current_action.PerformAction();
	}
	public void TakeDamage(int damage,Modifier.Type which_modifier)//受伤函数
	{
		if (stats.Health <= 0)
		{
			return;
		}


		sprite_2d.Material = WhiteSpriteMaterial;
		//经过修饰的伤害
		var modified_damage = modifier_handler.GetModifiedValue(damage, which_modifier);

		var tween = CreateTween();
		tween.TweenCallback(Callable.From(() => GetNode<shaker>("/root/Shaker").shake(this, 16f, 0.15f)));
		tween.TweenCallback(Callable.From(() => stats.TakeDamage(modified_damage)));
		tween.TweenInterval(0.15);
		tween.Finished += () =>
		{
			sprite_2d.Material = null;
			if (stats.Health <= 0)
			{

				QueueFree();
				events.instance.EmitSignal(events.SignalName.EnemyDied,this);
			}
		};
	}
	public void OnAreaEnterd(Area2D area)
	{
		arrow.Show();
	}
	public void OnAreaExited(Area2D area)
	{
		arrow.Hide();
	}
}
