using Godot;

namespace Diamondmine.scripts;
public partial class GameManager : Node2D
{
	[Export] public PackedScene CardScene {get;set;}
	[Export] public UI.UIManager UserInterface {get;set;}
	[Export] public int DragedCardZIndex {get;set;}
	[Export] public float CardStackingTransform {get;set;}
	[Export] public string DragActionName {get;set;}
	[Export] public string DebugProbeActionName {get;set;}
	[Export] public string UndoActionName {get;set;}
	[Export] public string RedoActionName {get;set;}
	[Export] public string NewGameActionName {get;set;}




	private DraggedCardData draggedCardData;
	private enum States
	{
		idle,
		cardDragged,
		newGame,
		win,
		lose
	}
	private States state = States.newGame;
	private uint _points = 0;
	public uint Points
	{
		get
		{
			return _points;
		}
		set
		{
			_points = value;	
			UserInterface.UpdatePoints(Points);
		}
	}
	private UndoRedo undoRedo = new();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		UserInterface.RedoButton.Pressed += Redo;
		UserInterface.UndoButton.Pressed += Undo;
		UserInterface.NewGameButton.Pressed += StartNewGame;
		StatisticsData.Load();	
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		switch(state)
		{
			//switch to state pattern? doesn't seems like super heavy and complicated yet
			case States.idle:
				return;
			case States.newGame:
				GameRules.GenerateDeck(this, CardScene);
				state = States.idle;
				break;
			case States.win:
				UserInterface.PlayerMessage.Text = "You win!";//todo make preatier
				break;
			case States.lose:
				//show "no possible moves" msg
				break;
			case States.cardDragged:
				Vector2 mousePosition = GetGlobalMousePosition();
				Card draggedCardNode = draggedCardData.CardNode;
				draggedCardNode.Position = new Vector2(
					float.Clamp(mousePosition.X, 0, GetViewportRect().Size.X),//looks like this might cause productivity drop?
					float.Clamp(mousePosition.Y, 0, GetViewportRect().Size.Y)
				) + draggedCardData.RelativeDragVector;
				break;
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
			if (state != States.cardDragged)
			{
				return;
			}
			state = States.idle;
			if (!TryDropHere())
			{
				Card draggedCardNode = draggedCardData.CardNode;
				draggedCardNode.Reparent(draggedCardData.CrardParentNode, false);
				draggedCardNode.Position = new Vector2(0,draggedCardData.CrardParentNode is Card ? CardStackingTransform : 0f);
				draggedCardNode.SetZIndexRecursive(draggedCardData.CardZIndexGlobal);
			}
		}

		if (@event.IsActionPressed(DebugProbeActionName))
		{
			GetDebugInfoAtCursor();
		}

		if (@event.IsActionPressed(UndoActionName))
		{
			Undo();
		}

		if (@event.IsActionPressed(RedoActionName))
		{
			Redo();
		}

		if (@event.IsActionPressed(NewGameActionName))
		{
			StartNewGame();
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
		bool wasParentClosed = cardNode.HasPreviousCard() && cardNode.GetPreviousCard().isClosed;
		draggedCardData = new(
			cardNode,
			cardNode.GetParent<Area2D>(),
			cardNode.GlobalPosition - GetGlobalMousePosition(),
			cardNode.ZIndex,
			wasParentClosed
		);
		state = States.cardDragged;
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
		}
		else if (matches.Count != 1)
		{
			string names = "";
			foreach (var match in matches)
			{
				names += ((Node2D)(GodotObject)match["collider"]).Name;
				names += ", ";
			}
			if (names == "Card, Card, ")
			{
				names = "";
				foreach (var match in matches)
				{
					names += ((Card)(GodotObject)match["collider"]).ToString();
					names += ", ";
				}
			}
			GD.Print("double drop bug. Nodes are " + names);
			return false;
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
		parent.CollisionLayer += GameRules.COLLISION_LAYER_DROPPABLE;
		if (parent is Card card && card.isClosed)
		{
			card.FlipFaceUp();
		}

		
		//card processing
		if (draggedCardNode.IsDiamonds())
		{
			draggedCardNode.CollisionLayer = GameRules.COLLISION_LAYER_DROPPABLE;
		}

		//game rules(win, lose, points ect) processing
		Points += GameRules.CalculatePointsChange(draggedCardNode);
		if (GameRules.IsWin(Points))
		{
			state = States.win;
		}
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
		
		////////////////////////////////////////
		if (parent is Card card && draggedCardDataLocal.WasParentClosed)
		{
			card.FlipFaceDown();
		}
		else
		{
			parent.CollisionLayer -= GameRules.COLLISION_LAYER_DROPPABLE;
		}
		//////////////////////////////////////////
		
		//card processing
		if (draggedCardNode.IsDiamonds())
		{
			draggedCardNode.CollisionLayer = GameRules.COLLISION_LAYER_DRAGGABLE;
		}

		//game rules(win, lose, points ect) processing
		Points -= GameRules.CalculatePointsChange(draggedCardNode);
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

	private void StartNewGame()
	{
		GameRules.ClearBoardFromCards(this);
		Points = 0;
		undoRedo = new();
		state = States.newGame;
	}


	private void GetDebugInfoAtCursor()
	{
		//check which areas2D associated with cards are in the point on mouse cursor
		var spaceState = GetWorld2D().DirectSpaceState;
		PhysicsPointQueryParameters2D queryParams = new();
		queryParams.SetPosition(GetGlobalMousePosition());
		queryParams.SetCollideWithAreas(true);
		// queryParams.SetCollisionMask(GameRules.COLLISION_LAYER_DRAGGABLE);
		Godot.Collections.Array<Godot.Collections.Dictionary> matches = spaceState.IntersectPoint(queryParams);
		if (matches.Count == 0)
		{
			GD.Print("No collision founded");
			return;
		}

		Card cardNode;
		//get the top card (highest absolute Z index)
		try
		{
			cardNode = (Card)(GodotObject)matches[0]["collider"];
		}
		catch
		{
			GD.Print("not a card");
			return;
		}
		
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

		GD.Print(cardNode.ToString());
	}
}
