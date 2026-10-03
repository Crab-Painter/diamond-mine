using Godot;

namespace Diamondmine.scripts.menu;

public partial class ReturnButton : Button
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Pressed += SendEvent;
	}

	private void SendEvent()
	{
		EventManager.BroadcastMenuReturnRequestEvent();
	}
}
