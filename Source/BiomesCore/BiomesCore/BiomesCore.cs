using HarmonyLib;
using System;
using System.Reflection;
using Verse;

namespace BiomesCore
{
    //[StaticConstructorOnStartup]
    public sealed class BiomesCoreStarter : Mod
    {
        public BiomesCoreStarter(ModContentPack content) : base(content)
        {
            new Harmony("rimworld.biomes.core").PatchAll();
            Log.Message("Biomes! Core - Initialized");
        }
    }
    //public sealed class BiomesCoreMod : Mod
    //{
    //    public BiomesCoreMod(ModContentPack content) : base(content)
    //    {
    //        new Harmony(BiomesCore.Id).PatchAll();
    //        BiomesCore.Log("Initialized");
    //    }
    //}

    [StaticConstructorOnStartup]
    public static class BiomesCore
    {
        public const string Id = "rimworld.biomes.core";
        public const string Name = "Biomes! Core";
        public static string Version = (Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyInformationalVersionAttribute)) as AssemblyInformationalVersionAttribute).InformationalVersion;

        public static void Log(string message) => Verse.Log.Message(PrefixMessage(message));

        private static string PrefixMessage(string message) => $"[{Name} v{Version}] {message}";
    }
}