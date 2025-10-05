using Godot;
using System;
using System.Threading.Tasks;

public partial class ManaUi : Panel
{
	public CharaStats char_stats;
	[Export]
	public CharaStats import_char_stats
	{
		get => char_stats;
		set
		{
			SetCharStats(value);
		}
	}
	public Label manalabel;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		manalabel = GetNode<Label>("ManaLabel");
		
		//var timer = GetTree().CreateTimer(2.0f);
		//timer.Timeout += () =>
		//{
		//char_stats.MANA = 0;
		//};
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public async void SetCharStats(CharaStats value)
	{
		char_stats = value;
		
		char_stats.StatsChanged += OnStatsChanged;

		// 等待节点 Ready（等价于 await ready）
		if (!IsNodeReady())
		{
			await ToSignal(this, "ready");
		}
		OnStatsChanged();
	}
	public override void _ExitTree()
{
    // 确保断开所有信号连接
    if (char_stats != null)
    {
        char_stats.StatsChanged -= OnStatsChanged;
    }
    base._ExitTree();
}
	public void OnStatsChanged()
	{
		manalabel.Text = $"{char_stats.MANA}/{char_stats.max_mana}";

	}


}
