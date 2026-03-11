using Godot;
using System;
public partial class GameManager : Node2D
{
	[Export] public PackedScene CardScene {get;set;}
	public enum GameStates
	{
		@default,
		dragCard
	}
	private Node2D draggedCardNode;
	private Node2D draggedCrardParent;
	private GameStates state = GameStates.@default;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GenerateDeck();
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (state == GameStates.dragCard)
		{
			Vector2 mousePosition = GetGlobalMousePosition();
			draggedCardNode.Position = new Vector2(
				float.Clamp(mousePosition.X, 0, GetViewportRect().Size.X),//looks like this might cause productivity drop?
				float.Clamp(mousePosition.Y, 0, GetViewportRect().Size.Y)
			);
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("LMB"))
		{
			DetectTopCard();
		}                  

		if (@event.IsActionReleased("LMB"))
		{
			state = GameStates.@default;
			draggedCardNode.Reparent(draggedCrardParent, false);
			draggedCardNode.Position = new Vector2(0,22);
			// GD.Print("released");
		}
	}

	private void DetectTopCard()
	{
		//check which areas2D associated with cards are in the point on mouse cursor
		var spaceState = GetWorld2D().DirectSpaceState;
		PhysicsPointQueryParameters2D queryParams = new();
		queryParams.SetPosition(GetGlobalMousePosition());
		queryParams.SetCollideWithAreas(true);
		queryParams.SetCollisionMask(1);//todo collision mask constants
		var matches = spaceState.IntersectPoint(queryParams);
		if (matches.Count == 0)
		{
			return;
		}

		//get the top card (highest absolute Z index)
		Node2D cardNode = (Node2D)(GodotObject)matches[0]["collider"];
		int maxZId = cardNode.ZIndex;
		foreach (Godot.Collections.Dictionary match in matches)
		{
			Node2D collisionNode = (Node2D)(GodotObject)match["collider"];
			if (collisionNode.ZIndex > maxZId)
			{
				maxZId = collisionNode.ZIndex;
				cardNode = collisionNode;
			}
		}

		draggedCardNode = cardNode;
		state = GameStates.dragCard;
		draggedCrardParent = cardNode.GetParent<Node2D>();
		cardNode.Reparent(this);//It's way easier to change (calculate changes as human) position of cards this way
	}

	private void GenerateDeck()
	{
		int[] deckIds = new int[52];
		for (int i=0;i<52;i++)
		{
			deckIds[i] = i;
		}
		(new Random()).Shuffle(deckIds);

		int foundationId = 1;
		int zId = 1;
		foreach (int i in deckIds)
		{
			int value = i%13+1;
			int suit = i/13+1;
			
			Texture2D texture = (Texture2D)ResourceLoader.Load("res://cardAssets/"+value+"-"+suit+".png");
			Card card = (Card)CardScene.Instantiate();
			card.value = value;
			card.suit = suit;
			card.Name = "Card";
			card.GetSpriteNode().Texture = texture;
			card.ZIndex = zId;

			Foundation foundation = GetNode<Foundation>("Foundation"+foundationId);
			foundation.furtestCard.AddChild(card);
			foundation.furtestCard = card;
			card.Position = new Vector2(0,22);

			foundationId++;
			if (foundationId > 13)
			{
				foundationId -= 13;
				zId++;
			}
		}
				
	}
}
