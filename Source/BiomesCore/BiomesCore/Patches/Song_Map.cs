using Verse;
using HarmonyLib;
using RimWorld;
using BiomesCore.DefModExtensions;
using System.Linq;
using System.Text;
using System.Reflection;
using LudeonTK;

namespace BiomesCore.Patches
{
    [HarmonyPatch(typeof(MusicManagerPlay), "AppropriateNow")]
    public static class Song_MapAppropriate_Patch
    {
		public static void Postfix(ref bool  __result, SongDef song)
		{
			Map map = Find.CurrentMap;
			Song_MapRestrictions ext = song.GetModExtension<Song_MapRestrictions>();
			if (__result && map != null && ext != null)
			{
				if (ext.BiomeDefRestrictions().Count() > 0)
					__result = ext.BiomeDefRestrictions().Any(r => r.defName == map.Biome.defName);

				if (ext.WeatherDefRestrictions().Count() > 0)
					__result = ext.WeatherDefRestrictions().Any(r => r.defName == map.weatherManager.curWeather.defName);

				if (ext.GameConditionDefRestrictions().Count() > 0)
					__result = ext.GameConditionDefRestrictions().Any(r => map.gameConditionManager.GetActiveCondition(GameConditionDef.Named(r.defName)) != null);

				if (ext.dangerRange.HasValue && !ext.dangerRange.Value.Includes(DangerWatcher_CalculateDangerRating.DangerRatingPerMap[map.uniqueID])) //If it has danger values defined and the current one isn't contained in the range..
					__result = false; //It's not appropriate right now.
			}
		}

		// The built-in music debugger doesn't load in 1.1, so this is copied to test the application of the patch above
		[DebugOutput]
		public static void AppropriateSongNowData()
		{

			MethodInfo AppropriateNowInfo = AccessTools.Method(typeof(MusicManagerPlay), "AppropriateNow");
			MusicManagerPlay mmp = Find.MusicManagerPlay;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Songs appropriate to play now:");
			foreach (SongDef item in DefDatabase<SongDef>.AllDefs.Where((SongDef s) => (bool)AppropriateNowInfo.Invoke(mmp, new object[] { s })))
			{
				stringBuilder.AppendLine("   " + item.defName);
			}
			stringBuilder.AppendLine();
			Log.Message(stringBuilder.ToString());
		}
	}
}
