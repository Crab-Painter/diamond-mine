using Diamondmine.scripts.menu;
using Godot;
using System;

public partial class FullscreenButton : CheckBox
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Pressed += Toggle;
	}

	public void Toggle()
	{
		SettingsData.IsFullscreen = ToggleMode;
		var mode = ToggleMode ? Window.ModeEnum.Fullscreen : Window.ModeEnum.Windowed;
		GetTree().GetRoot().GetWindow().SetMode(mode);
	}
}
