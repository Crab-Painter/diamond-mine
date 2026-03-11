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
	private Vector2 relativeDragPosition;
	private int cardZId;
	private GameStates state = GameStates.@default;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GameRules.GenerateDeck(this, CardScene);
		
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
			) + relativeDragPosition;
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
			draggedCardNode.Position = new Vector2(0,draggedCrardParent is Card ? 22f : 0f);
			draggedCardNode.ZIndex = cardZId;
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

		state = GameStates.dragCard;
		draggedCardNode = cardNode;
		draggedCrardParent = cardNode.GetParent<Node2D>();
		cardNode.Reparent(this);//It's way easier to change (calculate changes as human) position of cards this way
		relativeDragPosition = cardNode.GlobalPosition - GetGlobalMousePosition();
		cardZId = cardNode.ZIndex;
		cardNode.ZIndex = 100;
	}

}
