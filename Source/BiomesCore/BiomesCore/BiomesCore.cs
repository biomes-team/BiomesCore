using HarmonyLib;
using System;
using System.Reflection;
using RimWorld;
using Verse;

namespace BiomesCore
{
    [StaticConstructorOnStartup]
    public static class BiomesCore
    {
        public static Harmony HarmonyInstance;
        public const string Id = "rimworld.biomes.core";
        public const string Name = "Biomes! Core";
        public static string Version = (Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyInformationalVersionAttribute)) as AssemblyInformationalVersionAttribute).InformationalVersion;

        static BiomesCore()
        {
            HarmonyInstance = new Harmony(BiomesCore.Id);
            HarmonyInstance.PatchAll();
            LongEventHandler.ExecuteWhenFinished(Patches.ExtraStatInfo.Initialize);

            // ToDo joseasoler This line unpatches a destructive prefix from TMK which prevents any patches to
            // WildAnimalSpawner.SpawnRandomWildAnimalAt from working. This should be removed after TMK is reworked.
            MethodInfo method = AccessTools.Method(typeof(WildAnimalSpawner),
                nameof(WildAnimalSpawner.SpawnRandomWildAnimalAt));
            HarmonyInstance.Unpatch(method, HarmonyPatchType.Prefix, "net.mseal.rimworld.mod.terrain.movement");

            Log("Initialized");
        }

        public static void Log(string message) => Verse.Log.Message(PrefixMessage(message));
        public static void Error(string message) => Verse.Log.Error(PrefixMessage(message));

        private static string PrefixMessage(string message) => $"[{Name} v{Version}] {message}";
    }
}