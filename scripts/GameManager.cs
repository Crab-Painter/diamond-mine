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

	public List<Node> MenuPath = [];

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		RemoveChild(MenuScene);
		EventManager.OpenSubMenuEvent += OpenSubMenu;
		EventManager.MenuReturnRequestEvent += GoBackOneLevel;
		EventManager.OpenMenuRequestEvent += OpenMenu;
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
