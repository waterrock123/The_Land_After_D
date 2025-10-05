using Godot;
using Godot.Collections;
using System;

public partial class EnemyHandler : Node2D
{
	public Array<Enemy> acting_enemies = new Array<Enemy>();




	public override void _Ready()
	{
		events.instance.EnemyDied += OnEnemyDied;
		events.instance.EnemyActionCompleted += OnEnemyActionCompleted;
		events.instance.PlayerHandDrawn += OnPlayerHandDrawm;
	}
	public void ResetEnemyActions()
	{
		Enemy enemy = null;
		foreach (Node child in GetChildren())
		{
			enemy = child as Enemy;
			enemy.current_action = null;
			enemy.UpdateAction();
		}
	}


	public void SetupEnemies(battle_stats BattleStats)
	{
		if (BattleStats == null)
		{
			return;
		}
		foreach (Enemy enemy in GetChildren())
		{
			enemy.QueueFree();
		}
		var all_new_enemies = BattleStats.enemies.Instantiate();
		foreach (Node2D new_enemy in all_new_enemies.GetChildren())
		{
			var new_enemy_child = new_enemy.Duplicate() as Enemy;
			AddChild(new_enemy_child);
			new_enemy_child.status_handler.StatusesApplied += (type) => { OnEnemyStatusesApplied(type, new_enemy_child); };
		}
		all_new_enemies.QueueFree();


	}


	public override void _ExitTree()
	{
		if (events.instance != null)
		{
			events.instance.EnemyActionCompleted -= OnEnemyActionCompleted;
			events.instance.PlayerHandDrawn -= OnPlayerHandDrawm;



			// 其他解绑...
		}
	}
	public void StartTurn()
	{
		if (GetChildCount() == 0)
		{
			return;
		}
		acting_enemies.Clear();
		foreach (Enemy enemy in GetChildren())
		{
			acting_enemies.Add(enemy);
		}
		StartNextEnemyTurn();
	}
	public void OnEnemyActionCompleted(Enemy enemy)
	{
		enemy.status_handler.ApplyStatusesByType(status.Type.END_OF_TURN);



	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void StartNextEnemyTurn()
	{

		if (acting_enemies.Count == 0)
		{
			events.instance.EmitSignal(events.SignalName.EnemyTurnEnded);
			return;
		}
		acting_enemies[0].status_handler.ApplyStatusesByType(status.Type.START_OF_TURN);
	}

	public void OnEnemyStatusesApplied(status.Type type, Enemy enemy)
	{
		switch (type)
		{
			case status.Type.START_OF_TURN:
				enemy.DoTurn();
				GD.Print("攻击了");
				break;
			case status.Type.END_OF_TURN:
				acting_enemies.Remove(enemy);
				StartNextEnemyTurn();
				break;
		}
	}
	public void OnEnemyDied(Enemy enemy)
	{
		var IsEnemyTurn = acting_enemies.Count > 0;
		acting_enemies.Remove(enemy);
		if (IsEnemyTurn)
		{
			StartNextEnemyTurn();
		}
	}

	public void OnPlayerHandDrawm()
	{
		foreach (Enemy enemy in GetChildren())
		{
			enemy.UpdateIntent();
		}
	}

}
