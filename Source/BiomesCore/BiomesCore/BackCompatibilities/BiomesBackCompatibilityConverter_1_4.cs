using System;
using System.Collections.Generic;
using System.Text;
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
			"AteRawGastropod",
			"CaveIn",
			"Earthquake",
			"EarthquakeSound",
		};

		private static readonly Dictionary<string, string> fullReplacement = new Dictionary<string, string>()
		{
			{"EatFish", "BMT_EatFish"},
			{"FloatinggardenGranite", "BMT_Chinampa_Granite"},
			{"floatinggardenLimestone", "BMT_Chinampa_Limestone"},
			{"FloatinggardenMarble", "BMT_Chinampa_Marble"},
			{"FloatinggardenSandstone", "BMT_Chinampa_Sandstone"},
			{"FloatinggardenSlate", "BMT_Chinampa_Slate"}
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
			if (result != null)
			{
				Log.Error($"{defName} -> {result}");
			}
			*/

			return ReplaceDefName(defName);
		}
	}
}