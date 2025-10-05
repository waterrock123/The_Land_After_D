using Godot;
using System;
[GlobalClass]
public partial class Stats : Resource
{
    [Signal]
    public delegate void StatsChangedEventHandler();
    public int maxhealth=10;
    [Export]
    public int MaxHealth {
        get => maxhealth;
        set => SetMaxHealth(value);
    }
    [Export]
    public Texture art;

    private int _health;
    private int _block;
    public int Health
    {
        get => _health;
        set
        {
            _health = Mathf.Clamp(value, 0, MaxHealth);
            EmitSignal(SignalName.StatsChanged);
        }
    }
    public void SetMaxHealth(int value) {
        var diff = value - maxhealth;
        maxhealth = value;

        if (diff > 0)
        {
            Health += diff;
        }
        else if (Health > maxhealth)
        {
            Health = maxhealth;
        }
        EmitSignal(SignalName.StatsChanged);
    }
    public int Block
    {
        get => _block;
        set
        {
            _block = Math.Clamp(value, 0, 999);
            EmitSignal(SignalName.StatsChanged);
        }
    }
    public virtual void  TakeDamage(int damage)
    {
        if (damage <= 0)
        {
            return;
        }
        var initial_damage = damage;
        damage = Math.Clamp(damage - Block, 0, damage);
        this.Block = Math.Clamp(Block - initial_damage, 0, Block);
        this.Health -= damage;
    }
    public void TakeBlock(int block)
    {
        if (block <= 0)
        {
            return;
        }
        var initial_block = block;
        this.Block = Math.Clamp(Block + initial_block, 0, 999);

    }
    public void Heal(int amount)
    {
        this.Health += amount;
    }

    public virtual Resource CreateInstance()
    {
        Stats instance = (Stats)Duplicate();
        instance.Health = instance.MaxHealth;
        instance.Block = 0;
        return instance;
    }
}
