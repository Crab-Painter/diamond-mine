using Godot;
using System;
using System.Data;
public partial class GameManager : Node2D
{
	[Export] public PackedScene CardScene {get;set;}
	[Export] public int DragedCardZIndex {get;set;}
	[Export] public float CardStackingTransform {get;set;}
	[Export] public PointsCounter PointsCounter {get;set;}
	[Export] public string DragActionName {get;set;}
	[Export] public Button UndoButton {get;set;}
	[Export] public Button RedoButton {get;set;}

	private DraggedCardData draggedCardData;
	private bool isCardDragged = false;
	private int points = 0;
	private UndoRedo undoRedo = new();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		RedoButton.Pressed += Redo;
		UndoButton.Pressed += Undo;
		GameRules.GenerateDeck(this, CardScene);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (isCardDragged)
		{
			Vector2 mousePosition = GetGlobalMousePosition();
			Card draggedCardNode = draggedCardData.CardNode;
			draggedCardNode.Position = new Vector2(
				float.Clamp(mousePosition.X, 0, GetViewportRect().Size.X),//looks like this might cause productivity drop?
				float.Clamp(mousePosition.Y, 0, GetViewportRect().Size.Y)
			) + draggedCardData.RelativeDragVector;
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed(DragActionName))
		{
			DetectTopCard();
		}                  

		if (@event.IsActionReleased(DragActionName))
		{
			if (!isCardDragged)
			{
				return;
			}
			isCardDragged = false;
			if (!TryDropHere())
			{
				Card draggedCardNode = draggedCardData.CardNode;
				draggedCardNode.Reparent(draggedCardData.CrardParentNode, false);
				draggedCardNode.Position = new Vector2(0,draggedCardData.CrardParentNode is Card ? CardStackingTransform : 0f);
				draggedCardNode.SetZIndexRecursive(draggedCardData.CardZIndexGlobal);
			}
		}
	}

	private void DetectTopCard()
	{
		//check which areas2D associated with cards are in the point on mouse cursor
		var spaceState = GetWorld2D().DirectSpaceState;
		PhysicsPointQueryParameters2D queryParams = new();
		queryParams.SetPosition(GetGlobalMousePosition());
		queryParams.SetCollideWithAreas(true);
		queryParams.SetCollisionMask(GameRules.COLLISION_LAYER_DRAGGABLE);
		Godot.Collections.Array<Godot.Collections.Dictionary> matches = spaceState.IntersectPoint(queryParams);
		if (matches.Count == 0)
		{
			return;
		}

		//get the top card (highest absolute Z index)
		Card cardNode = (Card)(GodotObject)matches[0]["collider"];
		int maxZId = cardNode.ZIndex;
		foreach (Godot.Collections.Dictionary match in matches)
		{
			Card collisionNode = (Card)(GodotObject)match["collider"];
			if (collisionNode.ZIndex > maxZId)
			{
				maxZId = collisionNode.ZIndex;
				cardNode = collisionNode;
			}
		}

		//change gamestate
		draggedCardData = new(
			cardNode,
			cardNode.GetParent<Area2D>(),
			cardNode.GlobalPosition - GetGlobalMousePosition(),
			cardNode.ZIndex
		);
		isCardDragged = true;
		cardNode.Reparent(this);//It's way easier to change (calculate changes as human) position of cards this way
		cardNode.SetZIndexRecursive(DragedCardZIndex);
	}

	private bool TryDropHere()
	{
		var spaceState = GetWorld2D().DirectSpaceState;
		PhysicsPointQueryParameters2D queryParams = new();
		queryParams.SetPosition(GetGlobalMousePosition());
		queryParams.SetCollideWithAreas(true);
		queryParams.SetCollisionMask(GameRules.COLLISION_LAYER_DROPPABLE);
		queryParams.SetExclude([draggedCardData.CardNode.GetRid()]);
		var matches = spaceState.IntersectPoint(queryParams);
		if (matches.Count == 0)
		{
			return false;
		} else if (matches.Count != 1)
		{
			string names = "";
			foreach (var match in matches)
			{

				names += ((Node2D)(GodotObject)match["collider"]).Name;
				names += ", ";
			}
			throw new DataException("wrong number of droppable items. Need to check collision layers management. Nodes are: " + names);
		}

		Area2D dropPoint = (Area2D)(GodotObject)matches[0]["collider"];
		if (!GameRules.CanDrop(draggedCardData.CardNode, dropPoint))
		{
			return false;
		}


		DraggedCardData draggedCardDataLocal = draggedCardData;
		undoRedo.CreateAction("drag card");
		undoRedo.AddDoMethod(Callable.From(() => DropHere(dropPoint, draggedCardDataLocal)));
		undoRedo.AddUndoMethod(Callable.From(() => ReverseDropHere(dropPoint, draggedCardDataLocal)));
		undoRedo.CommitAction();
		return true;
	}
	
	private void DropHere(Area2D dropPoint, DraggedCardData draggedCardDataLocal)
	{
		//Visuals and parenting
		Card draggedCardNode = draggedCardDataLocal.CardNode;
		draggedCardNode.Reparent(dropPoint, false);
		draggedCardNode.Position = new Vector2(0,GetPositionYAfterDrop(dropPoint));
		draggedCardNode.SetZIndexRecursive(dropPoint.ZIndex+1);

		//drop point processing
		dropPoint.CollisionLayer -= GameRules.COLLISION_LAYER_DROPPABLE;

		//previous place processing
		Area2D parent = draggedCardDataLocal.CrardParentNode;
		if (parent is Card card)
		{
			card.FlipFaceUp();
		}
		else
		{
			parent.CollisionLayer += GameRules.COLLISION_LAYER_DROPPABLE;
		}
		
		//card processing
		if (draggedCardNode.IsDiamonds())
		{
			draggedCardNode.CollisionLayer = GameRules.COLLISION_LAYER_NON_DRAGGABLE + GameRules.COLLISION_LAYER_DROPPABLE;
		}

		//game rules(win, lose, points ect) processing
		points += GameRules.CalculatePointsChange(draggedCardNode);
		PointsCounter.UpdatePoints(points);
	}

	private void ReverseDropHere(Area2D dropPoint, DraggedCardData draggedCardDataLocal)
	{
		//Visuals and parenting
		Card draggedCardNode = draggedCardDataLocal.CardNode;
		Area2D parent = draggedCardDataLocal.CrardParentNode;
		draggedCardNode.Reparent(parent, false);
		draggedCardNode.Position = new Vector2(0,parent is Card ? CardStackingTransform : 0f);
		draggedCardNode.SetZIndexRecursive(draggedCardDataLocal.CardZIndexGlobal);

		//drop point processing
		dropPoint.CollisionLayer += GameRules.COLLISION_LAYER_DROPPABLE;

		//previous place processing
		
		if (parent is Card card)
		{
			card.FlipFaceDown();
		}
		else
		{
			parent.CollisionLayer -= GameRules.COLLISION_LAYER_DROPPABLE;
		}
		
		//card processing
		if (draggedCardNode.IsDiamonds())
		{
			draggedCardNode.CollisionLayer = GameRules.COLLISION_LAYER_DRAGGABLE;
		}

		//game rules(win, lose, points ect) processing
		points -= GameRules.CalculatePointsChange(draggedCardNode);
		PointsCounter.UpdatePoints(points);
	}

	private float GetPositionYAfterDrop(Area2D dropPoint)
	{
        if ((dropPoint is Card dropCard) && !dropCard.IsDiamonds())
        {
            return CardStackingTransform;
        }
		return 0f;
    }

	public void Undo()
	{
		undoRedo.Undo();
	}

	public void Redo()
	{
		undoRedo.Redo();
	}
}
