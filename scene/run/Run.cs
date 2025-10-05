using Godot;
using System;
using System.Dynamic;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

public partial class Run : Node
{	//常量场景载入
	public static  PackedScene BATTLE_SCENE=(PackedScene)ResourceLoader.Load("res://scene/battle/battle.tscn");
	public static  PackedScene BATTLE_REWARD_SCENE=(PackedScene)ResourceLoader.Load("res://scene/battle_reward/battle_reward.tscn");
	public static  PackedScene CAMPFIRE_SCENE=(PackedScene)ResourceLoader.Load("res://scene/campfire/campfire.tscn");
	
	public static  PackedScene SHOP_SCENE=(PackedScene)ResourceLoader.Load("res://scene/shop/shop.tscn");
	public static  PackedScene TREASURE_SCENE=(PackedScene)ResourceLoader.Load("res://scene/treasure/treasure.tscn");

	public static PackedScene WIN_SCREEN_SCENE = (PackedScene)GD.Load("res://scene/win_screen/win_screen.tscn");
	public static PackedScene MAIN_MENU_PATH = GD.Load<PackedScene>("res://scene/ui/main_menu.tscn");



	
	public Map map;

	//测试节点引用
	public Node current_view;

	public Button battle_button;
	public Button campfire_button;
	public Button map_button;
	public Button rewards_button;
	public Button shop_button;
	public Button treasure_button;

	//顶部栏相关
	public CardPileOpener deck_button;
	public CardPileView deck_view;
	public GoldUi gold_ui;
	public HealthUi health_ui;
	public RelicHandler relic_handler;
	public RelicTooltip relic_tooltip;


	public PauseMenu pause_menu;
	public run_stats stats;
	public CharaStats chararcter;

	public SaveGame save_data;


	public override void _Ready()
	{
		//节点加载
		pause_menu = GetNode<PauseMenu>("PauseMenu");
		current_view = GetNode("CurrentView");

		battle_button = (Button)GetNode("%BattleButton");
		campfire_button = (Button)GetNode("%CampfireButton");
		map_button = (Button)GetNode("%MapButton");
		rewards_button = (Button)GetNode("%RewardsButton");
		shop_button = (Button)GetNode("%ShopButton");
		treasure_button = (Button)GetNode("%TreasureButton");

		deck_button = (CardPileOpener)GetNode("%DeckButton");
		deck_view = (CardPileView)GetNode("%DeckView");
		gold_ui = (GoldUi)GetNode("%GoldUI");
		health_ui = (HealthUi)GetNode("%HealthUI");
		map = GetNode<Map>("Map");
		relic_handler = GetNode<RelicHandler>("%RelicHandler");
		relic_tooltip = GetNode<RelicTooltip>("%RelicTooltip");

		if (RunStartup.Instance == null)
		{
			return;
		}
		pause_menu.SaveAndQuit +=OnSaveAndQuitButtonPressed;
		switch (RunStartup.Instance.type)
		{
			case RunStartup.Type.NEW_RUN:
				chararcter = (CharaStats)RunStartup.Instance.picked_character.CreateInstance();
				StartRun();
				break;
			case RunStartup.Type.CONTINUED_RUN:
				LoadRun();
				break;
		}




	}
	public override void _ExitTree()
	{
		events.instance.BattleWon -= OnBattleWon;
		events.instance.BattleRewardExited -= ShowMap;
		events.instance.CampfireExited -= ShowMap;
		events.instance.MapExited -= OnMapExited;
		events.instance.ShopExited -= ShowMap;
		events.instance.TreasureRoomExited -= OnTreasureRoomExited;
		pause_menu.SaveAndQuit -= OnSaveAndQuitButtonPressed;
		chararcter.StatsChanged -= OnStatsChaged;
		deck_button.Pressed -= OnDeckButtonPressed;
		events.instance.RelicTooltipRequested -= relic_tooltip.ShowTooltip;
		events.instance.EventRoomExited -= ShowMap;
		
		
	
        
    }
	public void OnSaveAndQuitButtonPressed()
	{
		
		GetTree().ChangeSceneToFile("res://scene/ui/main_menu.tscn");
	}
	public void StartRun()//开始新的一局游戏的函数
	{
		stats = new run_stats();

		SetupEventConnections();
		SetupTopBar();
		map.GenerateNewMap();//生成一张新地图
		map.UnlockFloor(0);
		save_data = new SaveGame();
		SaveRun(true);

		
	}

	//	保存游戏
	public void SaveRun(bool was_on_map)
	{
		save_data.rng_seed = rng.instance.Seed;
		save_data.rng_state = rng.instance.State;

		save_data.Run_Stats = stats;
		save_data.char_stats = chararcter;
		save_data.current_deck = chararcter.deck;
		save_data.current_health = chararcter.Health;
		save_data.relics = relic_handler.GetAllRelics();
		
	if (map.last_room != null)
    {
        save_data.last_room_col = map.last_room.column;
        save_data.last_room_row = map.last_room.row;
    }
    else
    {
        
        save_data.last_room_col = -1;
        save_data.last_room_row = -1;
    }
		save_data.floors_climbed = map.floors_climbed;
		MapSaveManager.SaveMap(map.map_data.Duplicate());

		save_data.was_on_map = was_on_map;
		save_data.SaveData();
	}

	//加载
	public void LoadRun()
	{
		save_data = SaveGame.LoadData();
		if (save_data == null)
		{

			GD.Print("找不到保存的存档");
			return;
		}
		rng.SetFromSaveData(save_data.rng_seed, save_data.rng_state);
		stats = save_data.Run_Stats;
		chararcter = save_data.char_stats;
		chararcter.deck = save_data.current_deck;
		chararcter.Health = save_data.current_health;
		relic_handler.AddRelics(save_data.relics);

		SetupTopBar();
		SetupEventConnections();

		map.RebuildMapFromLoadedRooms(MapSaveManager.LoadMap());
		Room last_room;
		if (save_data.last_room_col == -1 && save_data.last_room_row == -1)
		{
			last_room = null;
		}
		else
		{
			last_room = map.map_data[save_data.last_room_row][save_data.last_room_col];
		}
		map.LoadMap(map.map_data, save_data.floors_climbed, last_room);
		if (last_room != null && !save_data.was_on_map)
		{
		 	OnMapExited(last_room);
		}
	}

	//进入战斗
	public void OnBattleRoomEntered(Room room)
	{
		Battle battle_scene = ChangeView(BATTLE_SCENE) as Battle;
		battle_scene.char_stats = chararcter;
		battle_scene.BattleStats = room.BattleStats;
		battle_scene.relics = relic_handler;
		battle_scene.StartBattle();

	}

	public void OnTreasureRoomEntered()
	{
		var treasure_scene = ChangeView(TREASURE_SCENE) as Treasure;
		treasure_scene.relic_handler = relic_handler;
		treasure_scene.char_stats = chararcter;
		treasure_scene.GenerateRelic();
	}

	public void OnTreasureRoomExited(Relic relic)
	{
		var reward_scene = ChangeView(BATTLE_REWARD_SCENE) as BattleReward;
		reward_scene.Run_Stats = stats;
		reward_scene.character_stats = chararcter;
		reward_scene.relic_handler = relic_handler;

		reward_scene.AddRelicReward(relic);
		reward_scene.AddGoldReward(rng.instance.RandiRange(65,120));


	}

    





	public void OnCampfireEntered()
	{
		var campfire = ChangeView(CAMPFIRE_SCENE) as Campfire;
		campfire.char_stats = chararcter;

	}


	public void OnShopEnterd()
	{
		var shop = ChangeView(SHOP_SCENE) as Shop;
		shop.char_stats = chararcter;
		shop.RunStats = stats;
		shop.relic_handler = relic_handler;
		events.instance.EmitSignal(events.SignalName.ShopEnterd, shop);
		shop.PopulateShop();
	}


	public Node ChangeView(PackedScene scene)//视图改变函数
	{
		
		if (current_view.GetChildCount() > 0)
		{
			current_view.GetChild(0).QueueFree();
		}
		GetTree().Paused = false;
		var new_view = scene.Instantiate();
		current_view.AddChild(new_view);
		map.HideMap();
		return new_view;
	}

	public void ShowMap()
	{
		if (current_view.GetChildCount() > 0)
		{
			current_view.GetChild(0).QueueFree();//删掉旧场景,在退出场景时
		}
		map.ShowMap();
		map.UnlockNextRooms();
		SaveRun(true);
	}

	public void SetupEventConnections()
	{
		events.instance.BattleWon += OnBattleWon;
		events.instance.BattleRewardExited += ShowMap;
		events.instance.CampfireExited += ShowMap;
		events.instance.MapExited += OnMapExited;
		events.instance.ShopExited += ShowMap;
		events.instance.TreasureRoomExited += OnTreasureRoomExited;
		events.instance.EventRoomExited += ShowMap;
		

		//测试按钮
		// battle_button.Pressed += () => ChangeView(BATTLE_SCENE);
		// campfire_button.Pressed += () => ChangeView(CAMPFIRE_SCENE);
		// map_button.Pressed += () => ShowMap();
		// rewards_button.Pressed += () => ChangeView(BATTLE_REWARD_SCENE);
		// shop_button.Pressed += () => ChangeView(SHOP_SCENE);
		// treasure_button.Pressed += () => ChangeView(TREASURE_SCENE);
	}
	public void OnDeckButtonPressed()
	{
		deck_view.show_current_view("Deck");
	}
	public void OnStatsChaged()
	{
		health_ui.UpdateStats(chararcter);
	}
	public void SetupTopBar()
	{
		chararcter.StatsChanged += OnStatsChaged;
		health_ui.UpdateStats(chararcter);
		gold_ui.importrun_stats = stats;

		relic_handler.AddRelic(chararcter.starting_relic);
		events.instance.RelicTooltipRequested += relic_tooltip.ShowTooltip;

		deck_button.importcard_pile = chararcter.deck;
		deck_view.card_pile = chararcter.deck;
		deck_button.Pressed += OnDeckButtonPressed;
	}


	public void ShowRegularBattleRewards()//常规战斗奖励
	{
		var reward_scene = ChangeView(BATTLE_REWARD_SCENE) as BattleReward;
		reward_scene.Run_Stats = stats;
		reward_scene.character_stats = chararcter;

		reward_scene.AddGoldReward(map.last_room.BattleStats.RollGoldReward());
		reward_scene.AddCardReward();
	}
	public void OnEventRoomEntered(Room room)
	{
		var event_room = ChangeView(room.event_scene) as EventRoom;
		event_room.character_stats = chararcter;
		event_room.RunStats = stats;
		event_room.relic_handler = relic_handler;
		event_room.SetUp();
	}

	public void OnBattleWon()
	{
		//判断是否为最后一层
		if (map.floors_climbed == MapGenerator.FLOORS)
		{
			var win_screen = ChangeView(WIN_SCREEN_SCENE) as WinScreen;
			win_screen.character = chararcter;
			SaveGame.DeleteData();
		}
		else
		{
			ShowRegularBattleRewards();
		}
	}
	




	public void OnMapExited(Room room)
	{
		SaveRun(false);
		switch (room.type)
		{
			case Room.Type.MONSTER:
				OnBattleRoomEntered(room);
				break;
			case Room.Type.TREASURE:
				OnTreasureRoomEntered();
				break;
			case Room.Type.CAMPFIRE:
				OnCampfireEntered();
				break;
			case Room.Type.SHOP:
				OnShopEnterd();
				break;
			case Room.Type.BOSS:
				OnBattleRoomEntered(room);
				break;
			case Room.Type.Event:
				OnEventRoomEntered(room);
				break;



		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
}
