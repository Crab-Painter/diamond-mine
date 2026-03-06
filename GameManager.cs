using Godot;
using System;
using System.Collections.Generic;

public partial class GameManager : Node2D
{
	public enum GameStates
	{
		@default,
		dragCard
	}
	private Node draggedCardNode;
	private Node state;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("LMB"))
		{
			// PointTracing();
			var spaceState = GetWorld2D().DirectSpaceState;
			PhysicsPointQueryParameters2D queryParams = new();
			queryParams.SetPosition(GetGlobalMousePosition());
			queryParams.SetCollideWithAreas(true);
			queryParams.SetCollisionMask(1);
			int resultLimit = 1;
			// as far as i understand, matches sorted in Z order. Since we need only top one, limiting number of results will improve performance
    		var matches = spaceState.IntersectPoint(queryParams,resultLimit);
			var result = matches.Count > 0 ? matches[0] : null;
			CollisionObject2D collider = (CollisionObject2D)(GodotObject)result["collider"];
			Node cardNode = collider.GetParent();
			draggedCardNode = cardNode;
			state = 
			
			GD.Print(cardNode);
		}

		if (@event.IsActionReleased("LMB"))
		{
			// GD.Print("released");
		}
	}
}
