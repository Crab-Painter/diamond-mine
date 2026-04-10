using Godot;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Diamondmine.scripts;

public static class GameRules
{
	public const uint COLLISION_LAYER_DRAGGABLE = 1;
	public const uint COLLISION_LAYER_DROPPABLE = 2;

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
			card.isClosed = true;
			card.CollisionLayer = 0;

			if (zId == 4)
			{
				card.CollisionLayer += COLLISION_LAYER_DRAGGABLE;
				if (suit != (int)Suits.diamonds)
				{
					card.CollisionLayer += COLLISION_LAYER_DROPPABLE;                
				}
				card.isClosed = false;
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

	public static void ClearBoardFromCards(GameManager rootNode)
	{
		for (int i=1;i<=13;i++)
		{
			
		}

		bool includeChildrenOfChildren = false;//it's false by default, but i leave this here for clearance, because i already had this question
		var rootChildren = rootNode.GetChildren(includeChildrenOfChildren);
		string pattern = @"^Foundation\d+$";
		Regex r = new(pattern);
		foreach (Node rootChild in rootChildren)
		{
			if (r.IsMatch(rootChild.Name))
			{
				Foundation foundation = (Foundation)rootChild;
				var children = foundation.GetChildren();
				foreach (Node child in children)
				{
					if (child.Name == "Card")
					{
						child.QueueFree();
					}
				}
				foundation.furtestCard = foundation;
				foundation.CollisionLayer = 0;
			}

// comment for now to catch the reason of the bug first, not just mask its followups
			// if (rootChild.Name == "Card")
			// {
			// 	rootChild.QueueFree();
			// }
		}

		Area2D diamondFoundation = rootNode.GetNode<Area2D>("DiamondFoundation");
		var diamondChildren = diamondFoundation.GetChildren();
		foreach (Node child in diamondChildren)
		{
			if (child.Name == "Card")
			{
				child.QueueFree();
			}
		}
		diamondFoundation.CollisionLayer = COLLISION_LAYER_DROPPABLE;
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
		
		return draggedCard.IsDiamonds() == (nodeToDropOn.Name == "DiamondFoundation");
	}

	//Updates point with assumption that card drag-n-drop was successfull
	public static uint CalculatePointsChange(Card draggedCard)
	{
		if (draggedCard.IsDiamonds())
		{
			return (uint)draggedCard.value;
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

	public static bool IsWin(uint pointsAmount)
	{
		return pointsAmount == 100;
	}
}
