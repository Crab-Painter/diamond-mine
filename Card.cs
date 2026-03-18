using Godot;
using System;

public partial class Card : Area2D
{
	/*
	expected scene structure:
	Area2D
	--CollisionShape2D
	--Sprite2D
	*/
	[Export] public string pathToSprite;
	public int value;
	public int suit;
	public bool isClosed;
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

	public void SetZIndexRecursive(int zId)
	{
		ZIndex = zId;
		if (HasNode("./Card"))
		{
			Card childCard = GetNode<Card>("./Card");
			childCard.SetZIndexRecursive(zId+1);
		}
	}

	public void FlipFaceUp()
	{
		Texture2D texture = (Texture2D)ResourceLoader.Load("res://cardAssets/"+value+"-"+suit+".png");
		GetSpriteNode().Texture = texture;
		CollisionLayer = GameRules.COLLISION_LAYER_DRAGGABLE;
		if (!IsDiamonds())
		{
			CollisionLayer += GameRules.COLLISION_LAYER_DROPPABLE;
		}
		isClosed = false;
	}

	public void FlipFaceDown()
	{
		Texture2D texture = (Texture2D)ResourceLoader.Load("res://cardAssets/CardBack.png");
		GetSpriteNode().Texture = texture;
		CollisionLayer = GameRules.COLLISION_LAYER_NON_DRAGGABLE;
		isClosed = true;
	}

	public bool IsDiamonds()
	{
		return suit == (int)GameRules.Suits.diamonds;
	}

	public bool HasPreviousCard()
	{
		return HasNode("../../Card");
	}

	public Card GetPreviousCard()
	{
		return GetNode<Card>("../../Card");
	}

	public bool HasNextCard()
	{
		return HasNode("./Card");
	}

	public Card GetNextCard()
	{
		return GetNode<Card>("./Card");
	}
}
