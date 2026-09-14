using Diamondmine.scripts.menu;
using Godot;

namespace Diamondmine.scripts;

public partial class Card : Area2D, IHighlightable
{
	[Export] public Sprite2D Image;
	[Export] public Sprite2D Highlighter;

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

	public void SetCardImage(Texture2D texture)
	{
		Image.Texture = texture;
	}

	public void SetZIndexRecursive(int zId)
	{
		var msg = Name + " " + value.ToString() + " of " + ((GameRules.Suits)suit).ToString() + ". Z index is " + zId.ToString();
		Logger.GetLogger().Log(Logger.LogTypes.debug,msg);
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

		Texture2D texture = (Texture2D)ResourceLoader.Load(SettingsData.CardAssetsDir+value+"-"+suit+".png");
		SetCardImage(texture);
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

		Texture2D texture = (Texture2D)ResourceLoader.Load(SettingsData.CardAssetsDir+"CardBack.png");
		SetCardImage(texture);
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

	public void HighlightOn()
	{
		Highlighter.Visible = true;
	}
	public void HighlightOff()
	{
		Highlighter.Visible = false;
	}
}
