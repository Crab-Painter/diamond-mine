using Diamondmine.scripts;
using Godot;
using System;

namespace Diamondmine.scripts.menu;

public partial class ResolutionButton : OptionButton
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ItemSelected += SendIvent;
	}

	public void SendIvent(long index)
	{
		GD.Print("selected"+Text);
		string[] dimensions = Text.Split("x");
        int width = dimensions[0].ToInt();
        int hight = dimensions[1].ToInt();
		GetTree().GetRoot().GetWindow().SetSize(new Vector2I(width,hight));
		GetTree().GetRoot().GetWindow().SetMode(Window.ModeEnum.Fullscreen);
        // EventManager.BroadcastNewResolutionSetEvent();
	}
}
