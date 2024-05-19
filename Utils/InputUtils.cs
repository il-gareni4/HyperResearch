using System.Collections.Generic;
using Terraria.ModLoader;

namespace HyperResearch.Utils;

public static class InputUtils
{
    /// <summary>Returns the string representation of the KeyBind assigned key</summary>
    /// <returns>
    ///     <para>"NULL" if keybind is null</para>
    ///     <para>"Not assigned" if keybind is not assigned</para>
    ///     <para>Otherwise assigned key</para>
    /// </returns>
    public static string GetKeybindString(ModKeybind? keybind)
    {
        if (keybind is null) return "<NULL>";
        List<string> keys = keybind.GetAssignedKeys();
        return keys.Count == 0 ? "<Not assigned>" : keys[0];
    }
}