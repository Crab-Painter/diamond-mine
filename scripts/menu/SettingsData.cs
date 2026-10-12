using System;
using System.Reflection;
using System.Runtime.InteropServices.Swift;
using Godot;

namespace Diamondmine.scripts.menu;
public partial class SettingsData : Resource
{
    public static string Resolution = "640x360";
    public static bool IsFullscreen = false;
    public static string CardAssetsDir = "res://assets/cardAssets/";

    private static readonly string saveLocation = "res://Settings.txt";//TODO chage for release and make it customisable through godot

    public static void Save()
    {
        using var saveFile = FileAccess.Open(saveLocation, FileAccess.ModeFlags.Write);

        var saveData = new Godot.Collections.Dictionary<string, string>
        {
            {"Resolution", Resolution},
            {"IsFullscreen", IsFullscreen.ToString()},
            {"CardAssetsDir", CardAssetsDir},
        };
        var jsonStr = Json.Stringify(saveData);

        saveFile.StoreString(jsonStr);
        saveFile.Close();
    }

    public static void Load()
    {
        Logger.GetLogger().Log(Logger.LogTypes.fileAccess, "start Load SettingsData");

        if (!FileAccess.FileExists(saveLocation))
        {
            Logger.GetLogger().Log(Logger.LogTypes.fileAccess, "save file does not exists. Creating one and exit Load()");
            //make new save and quit;
            Save();
            return;
        }

        using var saveFile = FileAccess.Open(saveLocation, FileAccess.ModeFlags.Read);
        if (saveFile == null)
        {
            Logger.GetLogger().Log(Logger.LogTypes.fileAccess, "loaded file is null");
        }
        string content = saveFile.GetAsText();

        Godot.Collections.Dictionary<string, Variant> parsedContent = (Godot.Collections.Dictionary<string, Variant>)Json.ParseString(content);
        if (parsedContent != null)
        {
            Logger.GetLogger().Log(Logger.LogTypes.fileAccess, "start reading parced content");
            foreach (FieldInfo field in typeof(SettingsData).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                Logger.GetLogger().Log(Logger.LogTypes.fileAccess, "start field " + field.Name);
                try
                {
                    if (parsedContent.TryGetValue(field.Name, out Variant value))
                    {
                        Logger.GetLogger().Log(Logger.LogTypes.fileAccess, "value type is: " + field.FieldType.Name);
                        Logger.GetLogger().Log(Logger.LogTypes.fileAccess, "value is: " + value.ToString());
                        if (field.FieldType.Name == "Boolean")
                        {
                            Logger.GetLogger().Log(Logger.LogTypes.fileAccess, "set bool value");
                            field.SetValue(null, Convert.ToBoolean(value.ToString()));
                        }
                        else if (field.FieldType.Name == "String")
                        {
                            Logger.GetLogger().Log(Logger.LogTypes.fileAccess, "set string value");
                            field.SetValue(null, value.ToString());
                        }
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
        else
        {
            Logger.GetLogger().Log(Logger.LogTypes.fileAccess, "parced file is null");
        }

    }
}
