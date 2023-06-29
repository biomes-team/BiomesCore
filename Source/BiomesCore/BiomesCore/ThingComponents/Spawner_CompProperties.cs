// RimWorld.CompProperties_Spawner
using RimWorld;
using Verse;

namespace BiomesCore
{

	public class CompProperties_NearSpawner : CompProperties
	{
		public ThingDef thingToSpawn;

		public int spawnCount = 1;

		public IntRange spawnIntervalRange = new IntRange(100, 100);

		// public int spawnMaxAdjacent = -1;

		// public bool spawnForbidden;

		public bool requiresPower;

		public bool writeTimeLeftToSpawn;

		public bool showMessageIfOwned;

		public string saveKeysPrefix;

		// public bool inheritFaction;

		public CompProperties_NearSpawner()
		{
			compClass = typeof(CompNearSpawner);
		}
	}

}
