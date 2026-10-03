using Godot;

namespace Diamondmine.scripts.menu;


public partial class ExitButton : Button
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Pressed += CloseGame;
	}

	private void CloseGame()
	{
		GetTree().Quit();
	}
}
