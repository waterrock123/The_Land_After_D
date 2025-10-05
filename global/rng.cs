using Godot;
using Godot.Collections;
using System;

public partial class rng : Node
{
    public static RandomNumberGenerator instance;


    public override void _Ready()
    {
        Initialize();

    }

    public void Initialize()
    {
        instance = new RandomNumberGenerator();
        instance.Randomize();
    }


    public static void SetFromSaveData(ulong which_seed, ulong state)
    {
        instance = new RandomNumberGenerator();
        instance.Seed = which_seed;
        instance.State = state;

    }

    public static Variant ArrayPickRandomCard(Godot.Collections.Array<Card> array)
    {
        return array[(int)(instance.Randi() % array.Count)];
    }

    public static Variant ArrayPickRandomRelic(Godot.Collections.Array<Relic> array)
    {
        return array[(int)(instance.Randi() % array.Count)];
    }
    public static Variant ArrayPickRandomEvent(Godot.Collections.Array<PackedScene> array)
    {
        return array[(int)(instance.Randi() % array.Count)];
    }

    public static void ArrayShuffle(Godot.Collections.Array<Card> array)
    {
        if (array.Count < 2)
        {
            return;
        }
        for (int i = array.Count - 1; i > 0; i--)
        {
            var j = (int)(instance.Randi() % (i + 1));
            var tmp = array[j];
            array[j] = array[i];
            array[i] = tmp;
        }
    }
    public static void ArrayShuffleRelic(Godot.Collections.Array<Relic> array)
    {
        if (array.Count < 2)
        {
            return;
        }
        for (int i = array.Count - 1; i == 0; i--)
        {
            var j = (int)instance.Randi() % (i + 1);
            var tmp = array[j];
            array[j] = array[i];
            array[i] = tmp;
        }
    }


}
