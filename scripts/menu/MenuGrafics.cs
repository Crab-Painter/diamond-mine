using Godot;
using System;

namespace Diamondmine.scripts.menu;


public partial class MenuGrafics : Control
{
	[Export] public OptionButton ResolutionSelect {get;set;}
	[Export] public CheckBox Fullscreen {get;set;}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ResolutionSelect.ItemSelected += ChangeResolution;
		Fullscreen.Pressed += Toggle;
		InitValues();
	}

	private void InitValues()
	{
		ResolutionSelect.Text = SettingsData.Resolution;
		Fullscreen.ButtonPressed = SettingsData.IsFullscreen;
	}

	public void Toggle()
	{
		GD.Print("pressed");
		SettingsData.IsFullscreen = Fullscreen.ButtonPressed;
		var mode = Fullscreen.ButtonPressed ? Window.ModeEnum.Fullscreen : Window.ModeEnum.Windowed;
		GetTree().GetRoot().GetWindow().SetMode(mode);
	}

	public void ChangeResolution(long index)
	{
		SettingsData.Resolution = ResolutionSelect.Text;
		string[] dimensions = ResolutionSelect.Text.Split("x");
        int width = dimensions[0].ToInt();
        int hight = dimensions[1].ToInt();
		GetTree().GetRoot().GetWindow().SetSize(new Vector2I(width,hight));
	}
}
