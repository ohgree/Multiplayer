﻿using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace ServerMod
{
    [HarmonyPatch(typeof(Area))]
    [HarmonyPatch("Set")]
    public static class AreaSetPatch
    {
        public static bool dontHandle;

        static bool Prefix(Area __instance, IntVec3 c, bool val)
        {
            if (ServerMod.client == null || dontHandle) return true;

            int index = __instance.Map.cellIndices.CellToIndex(c);
            if (__instance[index] == val) return false;

            __instance.Map.GetComponent<ServerModMapComp>().AreaChange(__instance.GetUniqueLoadID(), index, val);

            return false;
        }
    }

    [HarmonyPatch(typeof(Area))]
    [HarmonyPatch(nameof(Area.Invert))]
    public static class AreaInvertPatch
    {
        public static bool dontHandle;

        static bool Prefix(Area __instance)
        {
            if (ServerMod.client == null || dontHandle) return true;

            byte[] extra = Server.GetBytes(0, __instance.Map.GetUniqueLoadID(), Faction.OfPlayer.GetUniqueLoadID(), __instance.GetUniqueLoadID());
            ServerMod.client.Send(Packets.CLIENT_ACTION_REQUEST, new object[] { ServerAction.AREA, extra });

            return false;
        }
    }

    [HarmonyPatch(typeof(AreaManager))]
    [HarmonyPatch("Remove")]
    public static class AreaRemovePatch
    {
        static bool Prefix(Area __instance)
        {
            if (ServerMod.client == null) return true;
            Messages.Message("Action not supported in multiplayer.", MessageTypeDefOf.RejectInput);
            return false;
        }
    }

    [HarmonyPatch(typeof(AreaManager))]
    [HarmonyPatch(nameof(AreaManager.TryMakeNewAllowed))]
    public static class AreaAddPatch
    {
        static bool Prefix(Area __instance)
        {
            if (ServerMod.client == null) return true;
            Messages.Message("Action not supported in multiplayer.", MessageTypeDefOf.RejectInput);
            return false;
        }
    }

}
