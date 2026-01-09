using KogamaModFramework.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KogamaModFramework;

internal static class State
{
    internal static bool InitializedPatches = false;

    internal static void InitializeAllPatches()
    {
        if (InitializedPatches) return;
        var harmony = new HarmonyLib.Harmony("KogamaModFramework");
        harmony.PatchAll(typeof(State).Assembly);

        InitializedPatches = true;
    }

}
