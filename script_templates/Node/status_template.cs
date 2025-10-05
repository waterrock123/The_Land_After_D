using Godot;
using System;

public partial class status_template : status
{
    public int member_var = 0;
	

	public  override void  initialize_status(Node target)
	{


	}

    public override void apply_status(Node target)
    {
        EmitSignal(SignalName.StatusApplied, this);


    }
}
