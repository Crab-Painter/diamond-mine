using Godot;
using System;

public partial class Card : Sprite2D
{
	[Export] public string pathToSprite;
	public int value;
	public int suit;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public Sprite2D GetSpriteNode()
	{
		return (Sprite2D)GetNode(pathToSprite);
	}
}
