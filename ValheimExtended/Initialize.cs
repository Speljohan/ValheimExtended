using BepInEx;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ValheimExtended
{
    [BepInPlugin("org.bepinex.plugins.valheim_extended", "Valheim Extended", "0.1.0")]
    class ValheimExtendedPlugin : BaseUnityPlugin
    {

        void Awake()
        {
            var harmony = new Harmony("mod.valheim_extended");
            harmony.PatchAll();
        }

    }
}
