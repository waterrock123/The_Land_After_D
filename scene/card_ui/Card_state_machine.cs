using Godot;
using Godot.Collections;
using System;

public partial class Card_state_machine : Node
{
	[Export]
	card_state initial_state { get; set; }
	[Export]
	public card_state current_state;
	public Dictionary<card_state.State, card_state> states = new();


	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void init(CardUi card)
	{
		foreach (Node child in GetChildren())
		{
			if (child is card_state card_State)
			{
				states[card_State.state] = card_State;
				card_State.TransitionRequested += OnTransitionRequested;
				card_State.cardUi = card;
			}
		}

		if (initial_state != null)
		{
			initial_state.Enter();
			current_state = initial_state;

		}
	}
	public virtual void on_input(InputEvent @event)
	{
		current_state?.OnInput(@event);

	}
	public void on_gui_input(InputEvent @event)
	{
		current_state?.OnGuiInput(@event);
	}
	public void on_mouse_entered()
	{
		current_state?.OnMouseEntered();
	}
	public void on_mouse_exited()
	{
		current_state?.OnMouseExited();
	}
	private void OnTransitionRequested(card_state from, long Stateto)
	{
		card_state.State to = (card_state.State)(int)Stateto;
		if (from != current_state)
		{
			return;
		}
		if (!states.TryGetValue(to, out var newstate))
		{
			return;
		}
		current_state?.Exit();
		newstate.Enter();
		current_state = newstate;
		newstate.PostEntter();
	}
	
}
