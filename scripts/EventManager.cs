using System;
using Godot;

namespace Diamondmine.scripts;

public class EventManager
{
    public static Action<Node> OpenSubMenuEvent;
    public static Action MenuReturnRequestEvent;
    public static Action OpenMenuRequestEvent;


    public static void BroadcastOpenSubMenuEvent(Node scene)
    {
        OpenSubMenuEvent?.Invoke(scene);
    }

    public static void BroadcastMenuReturnRequestEvent()
    {
        MenuReturnRequestEvent?.Invoke();
    }

    public static void BroadcastOpenMenuRequestEvent()
    {
        OpenMenuRequestEvent?.Invoke();
    }
}