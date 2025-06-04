using System;
using System.Collections.Generic;

/// <summary>Minimal publish/subscribe hub.</summary>
public static class EventBus
{
    private static readonly Dictionary<string, Action<object>> _table = new();

    public static void Subscribe(string evt, Action<object> cb)
    {
        _table[evt] = _table.ContainsKey(evt) ? _table[evt] + cb : cb;
    }
    public static void Unsubscribe(string evt, Action<object> cb)
    {
        if (_table.ContainsKey(evt)) _table[evt] -= cb;
    }
    public static void Publish(string evt, object data = null)
    {
        if (_table.TryGetValue(evt, out var del)) del?.Invoke(data);
    }
}

public static class NetworkEvent
{
    public const string RawServerMsg = "net.raw";   // string JSON
    // define higher‑level routed events later, e.g. net.GameStart, net.TurnResult …
}