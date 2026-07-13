using System;
using System.Reflection;
using Godot;

namespace Diamondmine.scripts;
public partial class StatisticsData : Resource
{
    private static uint games;
    private static uint wins;
    private static uint loses;
    private static uint totalPoints;

    private static readonly string saveLocation = "res://Stats.txt";//TODO chage for release and make it customisable through godot

    public static float GetAverage()
    {
        return games == 0 ? 0f : (float)totalPoints/(float)games;
    }
    
    public static float GetAverageChange(uint currentPoints)
    {
        float newAverage = (float)(totalPoints + currentPoints)/(games+1f);
        return newAverage - GetAverage();
    }

    public static void EndGame(uint currentPoints, bool isWin)
    {
        games++;
        wins += (uint)(isWin ? 1 : 0);
        loses += (uint)(isWin ? 0 : 1);
        totalPoints += currentPoints;

        Save();
    }

    public static void Save()
    {
        using var saveFile = FileAccess.Open(saveLocation, FileAccess.ModeFlags.Write);

        var saveData = new Godot.Collections.Dictionary<string, uint>
        {
            {"games", games},
            {"wins", wins},
            {"loses", loses},
            {"totalPoints", totalPoints},
        };
        var jsonStr = Json.Stringify(saveData);

        saveFile.StoreString(jsonStr);
        saveFile.Close();
    }

    public static void Load()
    {
        if (!FileAccess.FileExists(saveLocation))
        {
            //make new save and quit;
            games = 0;
            wins = 0;
            loses = 0;
            totalPoints = 0;
            Save();
            return;
        }

        using var saveFile = FileAccess.Open(saveLocation, FileAccess.ModeFlags.Read);
        string content = saveFile.GetAsText();

        Godot.Collections.Dictionary<string, Variant> parsedContent = (Godot.Collections.Dictionary<string, Variant>)Json.ParseString(content);
        if (parsedContent != null)
        {
            foreach (FieldInfo field in typeof(StatisticsData).GetFields())
            {
                try
                {
                    field.SetValue(null, (uint)parsedContent[field.Name]);
                }
                catch (Exception e)
                {
                    var msg = "error while trying to write "+field.Name+" property: "+e.Message;
			        Logger.GetLogger().Log(Logger.LogTypes.exception, msg);
                    throw new Exception(msg);
                }
                parsedContent.Remove(field.Name);
            }


            foreach ((string key, _) in parsedContent)
            {
			    Logger.GetLogger().Log(Logger.LogTypes.error, "unused field "+key+" in a save file");
            }           
        }

    }
}
