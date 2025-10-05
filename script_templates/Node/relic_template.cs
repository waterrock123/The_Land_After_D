using Godot;
using System;
//遗物模版
public partial class relic_template : Relic
{
    public int member_var = 0;

    public override void InitializeRelic(RelicUi owner)
    {
        GD.Print("当我们获得一个新的遗物时触发");
    }

    public override void ActivateRelic(RelicUi owner)
    {
        GD.Print("激活遗物的方法，根据遗物类型在特定时间发生");
    }

    public override void DeactivateRelic(RelicUi owner)
    {
        GD.Print("当一个遗物被删除时发生");
        GD.Print("基于事件发生的遗物可以在此处断开与事件总线的链接");
    }

    //可选的独特提示,如果有动态UI需求的话
    public override string GetTooltip()
    {
        return tooltip;
    }






}
