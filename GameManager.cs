using Godot;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public partial class GameManager : Node2D
{
	public enum GameStates
	{
		@default,
		dragCard
	}
	private Node2D draggedCardNode;
	private GameStates state = GameStates.@default;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
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
			var spaceState = GetWorld2D().DirectSpaceState;
			PhysicsPointQueryParameters2D queryParams = new();
			queryParams.SetPosition(GetGlobalMousePosition());
			queryParams.SetCollideWithAreas(true);
			queryParams.SetCollisionMask(1);
			int resultLimit = 16;
			// as far as i understand, matches sorted in Z order. Since we need only top one, limiting number of results will improve performance
			//TODO check this
    		var matches = spaceState.IntersectPoint(queryParams,resultLimit);
			if (matches.Count == 0)
			{
				return;
			}

			GD.Print("Get " + matches.Count + " collisions");
			//get the top card (highest absolute Z index)
			CollisionObject2D collider = (CollisionObject2D)(GodotObject)matches[0]["collider"];
			int maxZId = ((Node2D)collider.GetParent()).ZIndex;
			GD.Print("Starting node is" + collider.GetParent().Name + ". It's index is " + maxZId);
			foreach (Godot.Collections.Dictionary match in matches)
			{
				CollisionObject2D collisionNode = (CollisionObject2D)(GodotObject)match["collider"];
				GD.Print("Checking node is" + collisionNode.GetParent().Name + ". It's index is " + ((Node2D)collisionNode.GetParent()).ZIndex);
				if (((Node2D)collisionNode.GetParent()).ZIndex > maxZId)
				{
					GD.Print("new collider candidate");
					maxZId = ((Node2D)collisionNode.GetParent()).ZIndex;
					collider = collisionNode;
				}
				
			}

			GD.Print("Collider node is" + collider.GetParent().Name + ". It's index is " + maxZId);
			Node cardNode = collider.GetParent();
			draggedCardNode = (Node2D)cardNode;//to ensure that we can move a card through script its root node should be child of or Node2D
			//TODO when i start moving to cards i need to ensure that root node is node2d
			state = GameStates.dragCard;
			
			GD.Print(cardNode.Name);
		}

		if (@event.IsActionReleased("LMB"))
		{
			state = GameStates.@default;
			// GD.Print("released");
		}
	}
}
