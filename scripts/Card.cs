using Godot;

namespace Diamondmine.scripts;

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


    public override string ToString()
    {
		string result = Name + " " + value.ToString() + " of " + ((GameRules.Suits)suit).ToString() + ". ";
		result += "Collision layer is " + CollisionLayer.ToString() + ". ";
		result += "The card is " + (isClosed ? "closed" : "open");
        return base.ToString() + result;
    }


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
		GD.Print(Name + " " + value.ToString() + " of " + ((GameRules.Suits)suit).ToString() + ". Z index is " + zId.ToString());
		ZIndex = zId;
		if (HasNode("./Card"))
		{
			Card childCard = GetNode<Card>("./Card");
			childCard.SetZIndexRecursive(zId+1);
		}
	}

	public void FlipFaceUp()
	{
		if (!isClosed)
		{
			return;
		}

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
		if (isClosed)
		{
			return;
		}

		Texture2D texture = (Texture2D)ResourceLoader.Load("res://cardAssets/CardBack.png");
		GetSpriteNode().Texture = texture;
		CollisionLayer = 0;
		isClosed = true;
	}

	public bool IsDiamonds()
	{
		return suit == (int)GameRules.Suits.diamonds;
	}

	public bool HasPreviousCard()
	{
		Node parent = GetParent();
		bool correctName = parent.Name == "Card";
		bool correctClass = parent is Card;

		return correctClass && correctName;
	}

	public Card GetPreviousCard()
	{
		return (Card)GetParent();
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
