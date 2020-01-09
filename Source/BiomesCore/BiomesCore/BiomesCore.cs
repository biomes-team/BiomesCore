using Harmony;
using System.Reflection;

namespace BiomesCore
{
    public static class BiomesCore
    {
        public const string Id = "rimworld.biomes.core";
        public const string Name = "Biomes! Core";
        public static string Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        static BiomesCore()
        {
            HarmonyInstance.Create(Id).PatchAll();
            Log("Initialized");
        }

        public static void Log(string message) => Verse.Log.Message(PrefixMessage(message));

        private static string PrefixMessage(string message) => $"[{Name} v{Version}] {message}";
    }
}
