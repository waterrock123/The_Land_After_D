using Godot;
using System;
[GlobalClass]
public partial class EnemyAction : Node
{
	[Export]
	public Type type;
	[Export(PropertyHint.Range, "0.0,10.0")]
	public double chance_weight = 0.0;
	[Export]
	public AudioStream sound;
	[Export]
	public intent Intent;
	public double accumulated_weight;
	public Enemy enemy;
	public Node2D target;

	public enum Type
	{
		CONDITIONAL,//条件性行动
		CHANCE_BASED//基础行动，根据概率来
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		accumulated_weight = 0.0;
	}
	public virtual bool IsPerformable()//特定行为判断函数，对于实例敌人要覆盖此函数
	{
		return false;
	}
	public virtual void PerformAction()//行动方法，供子类重写
	{

	}

	public virtual void UpdateIntentText()
	{
		Intent.current_text = Intent.base_text;
	}

	
}
