using Godot;
using Godot.Collections;
using System;
[GlobalClass]
public partial class Spread : Resource
{
    //主牌位索引
    [Export]
    public Array<int> MainSlotIndex = new Array<int>();
    //封锁索引
    [Export]
    public Array<int> IsLockIndex = new Array<int>();




}
