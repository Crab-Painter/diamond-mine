using System;
using System.Reflection;
using Godot;

namespace Diamondmine.scripts.menu;
public partial class SettingsData : Resource
{
    public static string Resolution = "";
    public static bool IsFullscreen = false;
    public static string CardAssetsDir = "res://assets/cardAssets/";

    private static readonly string saveLocation = "res://Settings.txt";//TODO chage for release and make it customisable through godot

    public static void Save()
    {
        using var saveFile = FileAccess.Open(saveLocation, FileAccess.ModeFlags.Write);

        var saveData = new Godot.Collections.Dictionary<string, string>
        {
            {"resolution", Resolution},
            {"isFullscreen", IsFullscreen.ToString()},
            {"cardAssetsDir", CardAssetsDir},
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
            Save();
            return;
        }

        using var saveFile = FileAccess.Open(saveLocation, FileAccess.ModeFlags.Read);
        string content = saveFile.GetAsText();

        Godot.Collections.Dictionary<string, Variant> parsedContent = (Godot.Collections.Dictionary<string, Variant>)Json.ParseString(content);
        if (parsedContent != null)
        {
            foreach (FieldInfo field in typeof(SettingsData).GetFields(BindingFlags.NonPublic | BindingFlags.Static))
            {
                try
                {
                    if (parsedContent.TryGetValue(field.Name, out Variant value))
                    {
                        if (field.FieldType.Name == "Boolean")
                        {
                            field.SetValue(null, Convert.ToBoolean(value));
                        }
                        field.SetValue(null, value);
                    }
                    
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
