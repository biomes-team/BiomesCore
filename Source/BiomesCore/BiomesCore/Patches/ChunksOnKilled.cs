using System;
using System.Collections.Generic;
using System.Linq;
using BiomesCore.ThingComponents;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BiomesCore.Patches
{
	[HarmonyPatch(typeof(GenLeaving))]
	public class GenLeaving_DoLeavingsFor
	{
		private static ThingDef FindChunkOfTerrain(TerrainDef def)
		{
			foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefs)
			{
				if (thingDef.building != null && thingDef.building.naturalTerrain == def)
				{
					return thingDef.building.mineableThing;
				}
			}

			return null;
		}

		[HarmonyPostfix]
		[HarmonyPatch("DoLeavingsFor", typeof(Thing), typeof(Map), typeof(DestroyMode), typeof(CellRect),
			typeof(Predicate<IntVec3>), typeof(List<Thing>))]
		public static void ChunksOnKilled(Thing diedThing, Map map)
		{
			var chunksOnKilled = diedThing.TryGetComp<CompChunksOnKilled>();
			if (chunksOnKilled != null)
			{
				var position = diedThing.Position;
				var terrainDef = position.GetTerrain(map);
				if (terrainDef.smoothedTerrain != null)
				{
					var chunkDef = FindChunkOfTerrain(terrainDef);
					if (chunkDef != null)
					{
						var count = chunksOnKilled.Props.chunkCountRange.RandomInRange;
						for (int num = 0; num < count; ++num)
						{
							Thing thing = ThingMaker.MakeThing(chunkDef);
							thing.stackCount = 1;
							GenSpawn.Spawn(thing, position, map, Rot4.Random);
						}
					}
				}
			}
		}
	}
}