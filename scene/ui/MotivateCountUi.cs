using Godot;
using System;

public partial class MotivateCountUi : Panel
{
    public CharaStats char_stats;
    public Label motivatelabel;
    [Export]
    public CharaStats import_char_stats
    {
        get => char_stats;
        set
        {
            SetCharStats(value);
        }
    }
    public override void _Ready()
	{
		motivatelabel = GetNode<Label>("MotivateLabel");
		
		
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
    public void OnStatsChanged()
	{
		motivatelabel.Text = $"{char_stats.motivate_count}/{char_stats.motivate_max}";

	}

}
