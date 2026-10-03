using System;
using Godot;

namespace Diamondmine.scripts;

public class EventManager
{
    public static Action<Node> OpenSubMenuEvent;


    public static void BroadcastOpenSubMenuEvent(Node scene)
    {
        OpenSubMenuEvent?.Invoke(scene);
    }

}