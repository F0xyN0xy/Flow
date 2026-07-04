namespace Flow.Input.Native;

/// <summary>
/// Maps human-readable key names (as stored in <c>AppSettings.Hotkey</c>, e.g. "F9") to
/// Windows virtual-key codes for use with <c>RegisterHotKey</c>.
/// </summary>
internal static class VirtualKeyMap
{
    private static readonly Dictionary<string, uint> Map = BuildMap();

    public static bool TryGetVirtualKey(string? keyName, out uint virtualKey)
    {
        if (string.IsNullOrWhiteSpace(keyName))
        {
            virtualKey = 0;
            return false;
        }

        return Map.TryGetValue(keyName.Trim(), out virtualKey);
    }

    private static Dictionary<string, uint> BuildMap()
    {
        var map = new Dictionary<string, uint>(StringComparer.OrdinalIgnoreCase);

        for (var f = 1; f <= 24; f++)
        {
            // VK_F1 = 0x70 ... VK_F24 = 0x87
            map[$"F{f}"] = (uint)(0x70 + (f - 1));
        }

        for (var c = 'A'; c <= 'Z'; c++)
        {
            // Virtual-key codes for letters match their ASCII uppercase values.
            map[c.ToString()] = c;
        }

        for (var d = 0; d <= 9; d++)
        {
            // Virtual-key codes for digits match their ASCII values.
            map[$"D{d}"] = (uint)('0' + d);
            map[d.ToString()] = (uint)('0' + d);
        }

        map["Escape"] = 0x1B;
        map["Space"] = 0x20;
        map["Tab"] = 0x09;
        map["Enter"] = 0x0D;
        map["Return"] = 0x0D;
        map["Insert"] = 0x2D;
        map["Delete"] = 0x2E;
        map["Home"] = 0x24;
        map["End"] = 0x23;
        map["PageUp"] = 0x21;
        map["PageDown"] = 0x22;
        map["Left"] = 0x25;
        map["Up"] = 0x26;
        map["Right"] = 0x27;
        map["Down"] = 0x28;
        map["PrintScreen"] = 0x2C;
        map["Pause"] = 0x13;

        return map;
    }
}
