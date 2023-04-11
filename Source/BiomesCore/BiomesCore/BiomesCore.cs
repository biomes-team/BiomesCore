using HarmonyLib;
using System;
using System.Reflection;
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
            Log("Initialized");
        }

        public static void Log(string message) => Verse.Log.Message(PrefixMessage(message));
        public static void Error(string message) => Verse.Log.Error(PrefixMessage(message));

        private static string PrefixMessage(string message) => $"[{Name} v{Version}] {message}";
    }
}