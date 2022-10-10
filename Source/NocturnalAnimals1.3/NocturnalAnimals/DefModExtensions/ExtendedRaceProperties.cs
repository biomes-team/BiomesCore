using Verse;

namespace NocturnalAnimals
{
	public class ExtendedRaceProperties : DefModExtension
	{
		private static readonly ExtendedRaceProperties defaultValues = new ExtendedRaceProperties();

		public BodyClock bodyClock;

		public static ExtendedRaceProperties Get(Def def)
		{
			return def.GetModExtension<ExtendedRaceProperties>() ?? defaultValues;
		}


		public static void Update(ThingDef animal)
		{
			if (NocturnalAnimalsMod.instance.Settings.AnimalSleepType.ContainsKey(animal.defName))
			{
				return;
			}

			var extendedRaceProps = Get(animal);
			if (extendedRaceProps == null)
			{
				NocturnalAnimalsMod.instance.Settings.AnimalSleepType[animal.defName] = 0;
				return;
			}


			NocturnalAnimalsMod.instance.Settings.AnimalSleepType[animal.defName] =
				(int)extendedRaceProps.bodyClock;
		}
	}
}