using Diamondmine.scripts.menu;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Diamondmine.scripts;
public partial class GameManager : Node
{
	[Export] public Game GameScene {get;set;}
	[Export] public Menu MenuScene {get;set;}
	[Export] public string MenuActionName {get;set;}

	private List<Node> MenuPath = [];

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		RemoveChild(MenuScene);
		EventManager.OpenSubMenuEvent += OpenSubMenu;
		EventManager.MenuReturnRequestEvent += GoBackOneLevel;
		EventManager.OpenMenuRequestEvent += OpenMenu;

		InitGraphics();
	}

	private void InitGraphics()
	{
		Logger.GetLogger().Log(Logger.LogTypes.gameInit, "InitGraphics");
		SettingsData.Load();
		
		var mode = SettingsData.IsFullscreen ? Window.ModeEnum.Fullscreen : Window.ModeEnum.Windowed;
		GetTree().GetRoot().GetWindow().SetMode(mode);
		Logger.GetLogger().Log(Logger.LogTypes.gameInit, "screen mode: " + mode.ToString());

		string[] dimensions = SettingsData.Resolution.Split("x");
        int width = dimensions[0].ToInt();
        int hight = dimensions[1].ToInt();
		GetTree().GetRoot().GetWindow().SetSize(new Vector2I(width,hight));
		Logger.GetLogger().Log(Logger.LogTypes.gameInit, "screen. width: " + width.ToString() + ", hight: " + hight.ToString());
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed(MenuActionName))
		{
			ToggleMenu();
		}                  
	}

	private void ToggleMenu()
	{
		if (MenuPath.Count == 0)
		{
			OpenMenu();
		}
		else
		{
			GoBackOneLevel();
		}
	}

	private void OpenMenu()
	{
		RemoveChild(GameScene);
		AddChild(MenuScene);
		MenuPath.Add(MenuScene);
	}
	private void GoBackOneLevel()
	{
		if (MenuPath.Count == 1)
		{
			CloseMenu();
			return;
		}

		Node currentScene = MenuPath.Last();
		MenuPath.RemoveAt(MenuPath.Count - 1);
		Node newScene = MenuPath.Last();
		RemoveChild(currentScene);
		AddChild(newScene);
	}
	private void CloseMenu()
	{
		MenuPath.Clear();
		RemoveChild(MenuScene);
		AddChild(GameScene);
	}
	

	private void OpenSubMenu(Node scene)
	{
		Node currentScene = MenuPath.Last();
		RemoveChild(currentScene);
		MenuPath.Add(scene);
		AddChild(scene);
	}
	
}
