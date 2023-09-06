using System.Collections.Generic;
using Terraria.ModLoader;

namespace BetterResearch.Utils
{
    public static class InputUtils
    {
        public static string GetKeybindString(ModKeybind keybind)
        {
            if (keybind is null) return "<NULL>";
            List<string> keys = keybind.GetAssignedKeys();
            if (keys.Count == 0) return "<Not assigned>";
            else return keys[0];
        }
    }
}