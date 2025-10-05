using Godot;
using Godot.Collections;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
public partial class RelicHandler : HBoxContainer
{
	[Signal]
	public delegate void RelicsActivatedEventHandler(Relic.Type type);

	public static float RELIC_APPLY_INTERVAL = 0.5f;
	public static PackedScene RELIC_UI = (PackedScene)GD.Load("res://scene/relic_handler/relic_ui.tscn");

	public relics_control relics_control;
	public HBoxContainer relics;


	public override void _Ready()
	{
		relics_control = GetNode<relics_control>("RelicsControl");
		relics = GetNode<HBoxContainer>("%Relics");
		relics.ChildExitingTree += OnRelicsChildExitingTree;//将遗物容器子节点退出树信号连接到回调方法

	


	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
    public override void _ExitTree()
    {
        relics.ChildExitingTree -= OnRelicsChildExitingTree;
    }



	public void ActivateRelicsByType(Relic.Type type)//急活type类的遗物效果，基于事件的遗物则由自己的事件决定
	{
		if (type == Relic.Type.EVENT_BASED)
		{
			return;
		}
		

		Array<RelicUi> relic_queue = new Array<RelicUi>();
		foreach (var relic_ui in GetAllRelicUiNodes())
		{
			if (relic_ui.relic.type == type)
				relic_queue.Add(relic_ui);
		}

		if (relic_queue.Count == 0)
		{
			EmitSignal(nameof(RelicsActivated), Variant.From(type));
			return;
		}

		var tween = CreateTween();
		foreach (RelicUi relic_ui in relic_queue)
		{
			tween.TweenCallback(Callable.From(() => relic_ui.relic.ActivateRelic(relic_ui)));
			tween.TweenInterval(RELIC_APPLY_INTERVAL);
		}
		tween.Finished += () => { EmitSignal(nameof(RelicsActivated), Variant.From(type)); };
		
	}
	

	public void AddRelics(Array<Relic> relics_array)
	{
		foreach (Relic relic in relics_array)
		{
			AddRelic(relic);
		}
	}

	public void AddRelic(Relic relic)
	{
		if (HasRelic(relic.id))
		{
			return;
		}
		var new_relic_ui = RELIC_UI.Instantiate() as RelicUi;
		relics.AddChild(new_relic_ui);
		new_relic_ui.importrelic = relic;
		new_relic_ui.relic.InitializeRelic(new_relic_ui);

	}

	public bool HasRelic(string id)
	{
		foreach (RelicUi relic_ui in relics.GetChildren())
		{
			if (relic_ui.relic.id == id && IsInstanceValid(relic_ui))//且是有效的遗物 =》为ui展示做的
			{
				return true;
			}
		}
		return false;
	}

	public Array<Relic> GetAllRelics()//获取遗物数组
	{
		var relic_ui_nodes = GetAllRelicUiNodes();
		Array<Relic> relics_array = new Array<Relic>();
		foreach (RelicUi relic_ui in relic_ui_nodes)
		{
			relics_array.Add(relic_ui.relic);
		}

		return relics_array;

	}


	public Array<RelicUi> GetAllRelicUiNodes()//获取遗物ui节点数组
	{
		var all_relics = new Array<RelicUi>();
		foreach (RelicUi relic_ui in relics.GetChildren())
		{
			all_relics.Add(relic_ui);
		}

		return all_relics;

	}

	public void OnRelicsChildExitingTree(Node relic_ui)
	{
		if (relic_ui == null)
		{
			return;
		}

		if ((relic_ui as RelicUi).relic != null)
		{
			(relic_ui as RelicUi).relic.DeactivateRelic((RelicUi)relic_ui);//使遗物效果无效
		}

		
	}


}
