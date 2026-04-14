using System.Collections.Generic;
using static Diamondmine.scripts.GameRules;

namespace Diamondmine.scripts;

public class GameRulesData
{
	private GameRulesData() {}
	private static GameRulesData _instance;

	public static GameRulesData getData()
	{
		_instance ??= new()
            {
                CollectedFullSuits = new()
                {
                    {(int)Suits.hearts, false},
                    {(int)Suits.clubs, false},
                    {(int)Suits.spades, false}
                }
            };

		return _instance;
	}
	public Dictionary<int,bool> CollectedFullSuits {get;set;}

	public bool IsSuitCollected(int suit)
	{
		return CollectedFullSuits[suit];		
	}
}
