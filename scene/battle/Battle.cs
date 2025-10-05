using Godot;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

public partial class Battle : Node2D
{
	[Export]
	public battle_stats BattleStats;
	[Export]
	public CharaStats char_stats;
	[Export]
	public AudioStream music;

	[Export]
	public RelicHandler relics;

	public sound_player musicplayer;
	public battle_ui BattleUI;
	public player Player;
	public PlayerHandler player_handler;
	public EnemyHandler enemy_handler;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
		musicplayer = GetNode<sound_player>("/root/MusicPlayer");
		BattleUI = GetNode<battle_ui>("BattleUI");
		Player = GetNode<player>("Player");
		player_handler = GetNode<PlayerHandler>("PlayerHandler");
		enemy_handler = GetNode<EnemyHandler>("EnemyHandler");


		
		enemy_handler.ChildOrderChanged += OnEnemiesChildOrderChanged;
		events.instance.EnemyTurnEnded += OnEnemyTurnEnded;
		events.instance.PlayerTurnEnded += player_handler.EndTurn;
		events.instance.PlayerHandDiscarder += enemy_handler.StartTurn;
		events.instance.PlayerDied += OnPlayerDied;

		
		


	}
    public override void _Input(InputEvent @event)
    {
		if (@event.IsActionPressed("ui_accept"))
		{
			var card_ui = player_handler.Hand.GetChild(0) as CardUi;
			card_ui.import_card.cost -= 1;
			card_ui.import_card = card_ui.card;
		}
    }
	public void OnEnemiesChildOrderChanged()//检查胜利
	{
		if (enemy_handler.GetChildCount() == 0 && IsInstanceValid(relics))
		{
			relics.ActivateRelicsByType(Relic.Type.END_OF_COMBAT);



		}

	}
	public override void _ExitTree()
	{
		// 解绑，防止重载/切换时回调旧实例
		events.instance.EnemyTurnEnded -= OnEnemyTurnEnded;
		events.instance.PlayerTurnEnded -= player_handler.EndTurn;
		events.instance.PlayerHandDiscarder -= enemy_handler.StartTurn;
		events.instance.PlayerDied -= OnPlayerDied;
		relics.RelicsActivated -= OnRelicsActivated;
        Player.status_handler.StatusesApplied -= player_handler.OnStatusesApplied;
    }

	public void OnPlayerDied()
	{
		events.instance.EmitSignal(events.SignalName.BattleOverScreenRequested, "Game Over!", Variant.From(BattleOverPanel.Type.LOSE));
		SaveGame.DeleteData();
	}
	
	
	public void StartBattle()//战斗开始
	{
		GetTree().Paused = false;
		musicplayer.play(music, true);

		BattleUI.import_stats = char_stats;
		Player.Stats = char_stats;
		player_handler.relics = relics;
		enemy_handler.SetupEnemies(BattleStats);
		enemy_handler.ResetEnemyActions();

		relics.RelicsActivated += OnRelicsActivated;
		relics.ActivateRelicsByType(Relic.Type.START_OF_COMBAT);


	}

	public void OnRelicsActivated(Relic.Type type)
	{
		switch (type)
		{
			case Relic.Type.START_OF_COMBAT:
				player_handler.StartBattle(char_stats);
				BattleUI.initialize_card_pile_ui();
				break;
			case Relic.Type.END_OF_COMBAT:

				events.instance.EmitSignal(events.SignalName.BattleOverScreenRequested, "胜利!", Variant.From(BattleOverPanel.Type.WIN));
				
				
				break;

		}
	}


	public void OnEnemyTurnEnded()
	{
		player_handler.StartTurn();
		enemy_handler.ResetEnemyActions();
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
