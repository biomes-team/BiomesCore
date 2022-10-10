using System.Collections.Generic;
using Verse;

namespace NocturnalAnimals
{
	internal class NocturnalAnimalsSettings : ModSettings
	{
		public Dictionary<string, int> AnimalSleepType = new Dictionary<string, int>();
		private List<string> animalSleepTypeKeys;
		private List<int> animalSleepTypeValues;

		public bool VerboseLogging;

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref VerboseLogging, "VerboseLogging");
			Scribe_Collections.Look(ref AnimalSleepType, "AnimalSleepType", LookMode.Value,
				LookMode.Value,
				ref animalSleepTypeKeys, ref animalSleepTypeValues);
		}

		public void ResetManualValues()
		{
			animalSleepTypeKeys = new List<string>();
			animalSleepTypeValues = new List<int>();
			AnimalSleepType = new Dictionary<string, int>();
			NocturnalAnimals.UpdateAnimalSleepTypes();
		}
	}
}