using System;
using System.Collections.Generic;
using System.Xml;
using Verse;

namespace BiomesCore.BackCompatibilities
{
	public class BiomesBackCompatibilityConverter_1_4 : BackCompatibilityConverter
	{
		public override bool AppliesToVersion(int majorVer, int minorVer)
		{
			return majorVer == 1 && minorVer <= 4;
		}

		public override void PostExposeData(object obj)
		{
		}

		public override Type GetBackCompatibleType(Type baseType, string providedClassName, XmlNode node)
		{
			return null;
		}

		private static readonly List<string> oldPrefixList = new List<string>
		{
			"BC_",
			"BiomeCore_",
			"Biomes_",
			"BiomesIslands_",
			"BiomesCore_",
			"RWB",
		};

		private static readonly List<string> missingPrefixList = new List<string>
		{
			"AquaticArrival",
			"AteRawGastropod",
			"EggLimepeelAngelfishFertilized",
			"EggSardineFertilized",
			"CaveIn",
			"CrabMigration",
			"FishTail",
			"GlowTide",
			"Earthquake",
			"EarthquakeSound",
			"RevenousCrabs",
		};

		private static readonly Dictionary<string, string> fullReplacement = new Dictionary<string, string>()
		{
			{"EatFish", "BMT_EatFish"}
		};

		private static string ReplaceDefName(string defName)
		{
			foreach (string prefix in oldPrefixList)
			{
				if (defName.StartsWith(prefix))
				{
					return $"BMT_{defName.Substring(prefix.Length)}";
				}

				if (defName.StartsWith($"Corpse_{prefix}"))
				{
					return $"Corpse_BMT_{defName.Substring(prefix.Length + 7)}";
				}

				if (defName.StartsWith($"Meat_{prefix}"))
				{
					return $"Meat_BMT_{defName.Substring(prefix.Length + 5)}";
				}
			}

			foreach (string missingPrefix in missingPrefixList)
			{
				if (defName == missingPrefix)
				{
					return $"BMT_{defName}";
				}
			}

			return fullReplacement.TryGetValue(defName, out string replacement) ? replacement : null;
		}

		public override string BackCompatibleDefName(Type defType, string defName, bool forDefInjections = false,
			XmlNode node = null)
		{
			string result = ReplaceDefName(defName);

			/*
			if (result != null && defName.Contains("Coral"))
			{
				Log.Error($"{defName} -> {result}");
			}
			*/

			return result;
		}

		private static readonly Dictionary<ushort, TerrainDef> _backCompatibleTerrainsWithShortHash =
			new Dictionary<ushort, TerrainDef>()
			{
				{153, BackCompatibilityDefOf.BMT_Pebbles},
				{4487, BackCompatibilityDefOf.BMT_WaterShallowLagoon},
				{5540, BackCompatibilityDefOf.BMT_Lava},
				{15314, BackCompatibilityDefOf.BMT_CoralRock_Rough},
				{20303, BackCompatibilityDefOf.BMT_CoralRock_Smooth},
				{20929, BackCompatibilityDefOf.BMT_Chinampa_Sandstone},
				{21599, BackCompatibilityDefOf.BMT_Chinampa_Slate},
				{27580, BackCompatibilityDefOf.BMT_FineTileCoral},
				{30231, BackCompatibilityDefOf.BMT_Chinampa_Coral},
				{32745, BackCompatibilityDefOf.BMT_Magma},
				{36463, BackCompatibilityDefOf.BMT_Chinampa_Marble},
				{40925, BackCompatibilityDefOf.BMT_FlagstoneCoral},
				{54665, BackCompatibilityDefOf.BMT_TileCoral},
				{58239, BackCompatibilityDefOf.BMT_WaterAbyssalDeep},
				{63746, BackCompatibilityDefOf.BMT_Chinampa_Granite},
				{63809, BackCompatibilityDefOf.BMT_CoralRock_RoughHewn}
			};

		public static TerrainDef BackCompatibleTerrainShortHash(ushort shortHash)
		{
			return _backCompatibleTerrainsWithShortHash.TryGetValue(shortHash, out TerrainDef terrainDef)
				? terrainDef
				: null;
		}
	}
}