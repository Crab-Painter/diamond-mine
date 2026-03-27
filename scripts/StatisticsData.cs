namespace Diamondmine.scripts;
public class StatisticsData
{
    public uint games = 1;
    public uint wins = 0;
    public uint loses = 0;
    public uint totalPoints = 7;

    public float GetAverage()
    {
        return games == 0 ? 0f : (float)totalPoints/(float)games;
    }
    
    public float GetChange(uint currentPoints)
    {
        float newAverage = (float)(totalPoints + currentPoints)/(games+1f);
        return newAverage - GetAverage();
    }

    public static void Save()
    {
        
    }

    public static void Load()
    {
        
    }
}
