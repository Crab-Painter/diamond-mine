using Godot;
using System;
using System.Collections.Generic;

namespace Diamondmine.scripts;

public partial class Menu : Control
{
	[Export] public Button GraphicsButtonScene {get;set;}
	[Export] public PackedScene GraphicsScene {get;set;}


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GraphicsButtonScene.Pressed += OpenGraphicSettings;
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	private void OpenGraphicSettings()
	{
		EventManager.BroadcastOpenSubMenuEvent(GraphicsScene.Instantiate());
	}
}
