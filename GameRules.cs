using Godot;
using System;

public static class GameRules
{
	public const uint COLLISION_LAYER_DRAGGABLE = 1;
	public const uint COLLISION_LAYER_NON_DRAGGABLE = 2;
	public const uint COLLISION_LAYER_DROPPABLE = 4;
	public const uint COLLISION_LAYER_DROPPABLE_DIAMONDS = 8;

	public enum Suits
	{
		diamonds = 1,
		hearts = 2,
		clubs = 3,
		spades = 4
	}

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
			} else
			{
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

	public static uint GetDropMaskForCard(Card draggedCard)
	{
		if (draggedCard.IsDiamonds())
		{
			return COLLISION_LAYER_DROPPABLE_DIAMONDS;
		}

		return COLLISION_LAYER_DROPPABLE;
		
	}

	public static bool CanDrop(Card draggedCard, Card cardToDropOn)
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
}
