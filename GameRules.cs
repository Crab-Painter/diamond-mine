using Godot;
using System;
using System.Collections.Generic;

public static class GameRules
{
	public const uint COLLISION_LAYER_DRAGGABLE = 1;
	public const uint COLLISION_LAYER_NON_DRAGGABLE = 2;
	public const uint COLLISION_LAYER_DROPPABLE = 4;

	public enum Suits
	{
		diamonds = 1,
		hearts = 2,
		clubs = 3,
		spades = 4
	}

	private static Dictionary<int,bool> collectedFullSuits = new()
	{
		{(int)Suits.hearts, false},
		{(int)Suits.clubs, false},
		{(int)Suits.spades, false}
	};

    public static void GenerateDeck(GameManager rootNode, PackedScene cardScene)
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
			
			string textureName = zId == 4 ? "res://cardAssets/"+value+"-"+suit+".png" : "res://cardAssets/CardBack.png";
			Texture2D texture = (Texture2D)ResourceLoader.Load(textureName);
			Card card = (Card)cardScene.Instantiate();
			card.value = value;
			card.suit = suit;
			card.Name = "Card";
			card.GetSpriteNode().Texture = texture;
			card.ZIndex = zId;
			if (zId == 4)
			{
				card.CollisionLayer = COLLISION_LAYER_DRAGGABLE;
				if (suit != (int)Suits.diamonds)
				{
					card.CollisionLayer += COLLISION_LAYER_DROPPABLE;                
				}
				card.isClosed = false;
			}
			else
			{
				card.isClosed = true;
				card.CollisionLayer = COLLISION_LAYER_NON_DRAGGABLE;
			}
			

			Foundation foundation = rootNode.GetNode<Foundation>("Foundation"+foundationId);
			foundation.furtestCard.AddChild(card);
			card.Position = new Vector2(0,foundation.furtestCard is Card ? rootNode.CardStackingTransform : 0f);
			foundation.furtestCard = card;

			foundationId++;
			if (foundationId > 13)
			{
				foundationId -= 13;
				zId++;
			}
		}		
	}

	public static bool CanDrop(Card draggedCard, Area2D nodeToDropOn)
	{
		if (nodeToDropOn is Card cardToDropOn)
		{
			int valueDelta = cardToDropOn.value - draggedCard.value;
			if (draggedCard.IsDiamonds())
			{
				return valueDelta == -1 || valueDelta == 12;
			}
			else
			{
				return valueDelta == 1;
			}
		}

		if (draggedCard.IsDiamonds())
		{
			return nodeToDropOn.Name == "DiamondFoundation";
		}
		return true;
	}

	//Updates point with assumption that card drag-n-drop was successfull
	public static int CalculatePointsChange(Card draggedCard)
	{
		if (draggedCard.IsDiamonds())
		{
			return draggedCard.value;
		}

		//check full suit pile
		if (collectedFullSuits[draggedCard.suit])
		{
			return 0;
		}
		// going up the tree
		Card cardPointer = draggedCard;
		while(cardPointer.HasPreviousCard())
		{
			Card previousCard = cardPointer.GetPreviousCard();
			if (previousCard.suit != cardPointer.suit || (previousCard.value - cardPointer.value != 1))
			{
				return 0;
			}
			cardPointer = previousCard;
		}
		if (cardPointer.value != 13)
		{
			return 0;
		}

		//going dowh the tree
		cardPointer = draggedCard;
		while(cardPointer.HasNextCard())
		{
			Card nexrCard = cardPointer.GetNextCard();
			if (nexrCard.suit != cardPointer.suit || (cardPointer.value - nexrCard.value != 1))
			{
				return 0;
			}
			cardPointer = nexrCard;
		}
		if (cardPointer.value != 1)
		{
			return 0;
		}

		collectedFullSuits[draggedCard.suit] = true;
		return 3;
	}
}
