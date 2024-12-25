using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameEnums;

public static class EventsManager
{
    private static Dictionary<EventID, Action<object>> _dictEvents = new Dictionary<EventID, Action<object>>();

    public static void Subscribe(EventID eventID, Action<object> callback)
    {
        if (!_dictEvents.ContainsKey(eventID))
        {
            _dictEvents.Add(eventID, callback);
            return;
        }

        _dictEvents[eventID] += callback;
    }

    public static void Unsubscribe(EventID eventID, Action<object> callback)
    {
        if (_dictEvents.ContainsKey(eventID))
            _dictEvents[eventID] -= callback;
    }

    public static void Notify(EventID eventID, object eventArgs = null)
    {
        _dictEvents[eventID]?.Invoke(eventArgs);
    }
}