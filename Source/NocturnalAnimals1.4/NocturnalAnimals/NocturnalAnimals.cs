using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace NocturnalAnimals
{
	[StaticConstructorOnStartup]
	public class NocturnalAnimals
	{
		public static readonly List<ThingDef> AllAnimals;
		

		static NocturnalAnimals()
		{
			AllAnimals = DefDatabase<ThingDef>.AllDefsListForReading
				.Where(def => def.race is { Animal: true })
				.OrderBy(def => def.label).ToList();
			UpdateAnimalSleepTypes();
			var HarmonyInstance = new Harmony("XeoNovaDan.NocturnalAnimals");
			HarmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
		}

		public static void UpdateAnimalSleepTypes()
		{
			foreach (var animal in AllAnimals)
			{
				ExtendedRaceProperties.Update(animal);
			}
		}
	}
}
