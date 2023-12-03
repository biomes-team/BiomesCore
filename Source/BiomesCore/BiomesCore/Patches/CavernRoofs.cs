using BiomesCore.DefModExtensions;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;
using System.Reflection;
using System.Reflection.Emit;
using BiomesCore.MapGeneration;
using BiomesCore.Reflections;
using RimWorld.Planet;

namespace BiomesCore.Patches
{
	[HarmonyPatch]
	public static class DisableRaidStrategies
	{
		[HarmonyTargetMethods]
		public static IEnumerable<MethodBase> TargetMethods()
		{
			yield return AccessTools.Method(typeof(RaidStrategyWorker_Siege), "CanUseWith");
			yield return AccessTools.Method(typeof(RaidStrategyWorker_SiegeMechanoid), "CanUseWith");
		}

		public static void Postfix(ref bool __result, IncidentParms parms, PawnGroupKindDef groupKind)
		{
			if (parms.target is Map map)
			{
				BiomesMap biome = map.Biome.GetModExtension<BiomesMap>();
				if (biome != null && biome.isCavern)
				{
					__result = false;
				}
			}
		}
	}

	/// <summary>
	///  Picks color for custom roofs, when roof overlay is toggled
	///  Vanilla: thin roof = white, thick roof = 50% grey
	/// </summary>
	[HarmonyPatch(typeof(RoofGrid), "GetCellExtraColor")]
	static class RoofColorPatch
	{
		static void Postfix(int index, ref RoofGrid __instance, ref Color __result)
		{
			if (BiomesCoreDefOf.BMT_RockRoofStable != null && __instance.RoofAt(index) == BiomesCoreDefOf.BMT_RockRoofStable)
			{
				//__result = Color.black;

				// light grey
				//__result = new Color(0.75f, 0.75f, 0.75f, 1f);

				// dark grey
				__result = new Color(0.25f, 0.25f, 0.25f, 1f);
			}
		}
	}


	/// <summary>
	/// Allows placement of the cave roofs on map generation
	/// </summary>
	[HarmonyPatch(typeof(GenStep_RocksFromGrid), "Generate")]
	static class CaveRoofGeneration
	{
		static bool Prefix(Map map, GenStepParams parms)
		{
			var modExtension = map.Biome.GetModExtension<BiomesMap>();
			
			// solid cave roof for caverns
			if (modExtension?.isCavern == true && !modExtension.cavernShapes.NullOrEmpty())
			{
				new RocksFromGrid_Cavern().Generate(map, parms);
				return false;
			}

			return true;
		}
	}

	//Rolled into the roof detection system. -UdderlyEvelyn 3/11/22 
	/// <summary>
	/// cave roofs don't have to be within range of a wall
	/// </summary>
	//[HarmonyPatch(typeof(RoofCollapseUtility), "WithinRangeOfRoofHolder")]
	//static class RoofCollapse_Disable
	//{
	//    static bool Prefix(IntVec3 c, Map map, ref bool __result)
	//    {
	//        if (map.roofGrid.RoofAt(c) == BiomesCoreDefOf.BMT_RockRoofStable)
	//        {
	//            __result = true;
	//            return false;
	//        }
	//        return true;
	//    }

	//}

	/// <summary>
	/// Lowers infestation chance under cave roofs
	/// </summary>
	[HarmonyPatch(typeof(InfestationCellFinder), "GetMountainousnessScoreAt")]
	static class InfestationModifier
	{
		static void Postfix(IntVec3 cell, Map map, ref float __result)
		{
			if (map.roofGrid.RoofAt(cell) == BiomesCoreDefOf.BMT_RockRoofStable)
			{
				__result *= 0.25f;
			}
		}
	}
	
	/// <summary>
	/// Undergrounders should feel indoors in caverns biomes
	/// </summary>
	[HarmonyPatch(typeof(ThoughtWorker_IsIndoorsForUndergrounder), "IsAwakeAndIndoors")]
	internal static class IndoorsForUndergrounder
	{
		private static bool Prefix(Pawn p, ref bool isNaturalRoof, ref bool __result)
		{
			if (p.Map.TileInfo.hilliness == Hilliness.Impassable) // better for performance than checking def extension
			{
				var roofDef = p.Map.roofGrid.RoofAt(p.Position);
				isNaturalRoof = roofDef?.isNatural ?? false;
				__result = p.Awake() && roofDef != null;
				return false;
			}

			return true;
		}
	}
	
	/// <summary>
	/// Undergrounders should not feel outdoors in caverns biomes
	/// </summary>
	[HarmonyPatch(typeof(ThoughtWorker_IsOutdoorsForUndergrounder), "CurrentStateInternal")]
	internal static class OutdoorsForUndergrounder
	{
		private static bool Prefix(Pawn p, ref ThoughtState __result)
		{
			if (p.Map.TileInfo.hilliness == Hilliness.Impassable) // better for performance than checking def extension
			{
				__result = p.Awake() && !p.Position.Roofed(p.Map);
				return false;
			}

			return true;
		}
	}

	[HarmonyPatch]
	static class Cavern_DropCellFinder_RandomDropSpot
	{
		public static MethodInfo IntVec3UnbreachableRoofedInfo =
			AccessTools.Method(typeof(IntVec3Extensions), "UnbreachableRoofed");

		static MethodBase TargetMethod()
		{
			// Fetch the first lambda in Generate
			return typeof(DropCellFinder).GetLambda("RandomDropSpot");
		}

		// Changes
		// IntVec3.Roofed -> IntVec3Extended.Roofed
		[HarmonyPriority(Priority.First)]
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return instructions.ReplaceFunction(
				IntVec3UnbreachableRoofedInfo,
				"Roofed",
				"DropCellFinder.RandomDropSpot");
		}
	}

	[HarmonyPatch]
	static class Cavern_DropCellFinder_TradeDropSpot
	{
		public static MethodInfo RoofGridUnbreachableRoofedInfo = AccessTools.Method(typeof(RoofGridExtensions),
			"UnbreachableRoofed", new Type[] {typeof(RoofGrid), typeof(IntVec3)});

		static MethodBase TargetMethod()
		{
			// Fetch the third lambda
			return typeof(DropCellFinder).GetLambda("TradeDropSpot", lambdaOrdinal: 2);
		}

		// Changes
		// RoofGrid.Roofed -> RoofGridExtended.Roofed
		[HarmonyPriority(Priority.First)]
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return instructions.ReplaceFunction(
				RoofGridUnbreachableRoofedInfo,
				"Roofed",
				"DropCellFinder.TradeDropSpot");
		}
	}

	[HarmonyPatch]
	static class Cavern_DropCellFinder_TryFindSafeLandingSpotCloseToColony
	{
		public static MethodInfo IntVec3UnbreachableRoofedInfo =
			AccessTools.Method(typeof(IntVec3Extensions), "UnbreachableRoofed");

		static MethodBase TargetMethod()
		{
			// Fetch the third lambda
			return typeof(DropCellFinder).GetLocalFunc("TryFindSafeLandingSpotCloseToColony", localFunc: "SpotValidator");
		}

		// Changes
		// IntVec3.Roofed -> IntVec3Extended.Roofed
		[HarmonyPriority(Priority.First)]
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return instructions.ReplaceFunction(
				IntVec3UnbreachableRoofedInfo,
				"Roofed",
				"DropCellFinder.TryFindSafeLandingSpotCloseToColony");
		}
	}

	[HarmonyPatch(typeof(DropCellFinder), "CanPhysicallyDropInto")]
	static class Cavern_DropCellFinder_CanPhysicallyDropInto
	{
		public static bool Prefix(ref bool __result, IntVec3 c, Map map, bool canRoofPunch, bool allowedIndoors = true)
		{
			if (c.InBounds(map) && map.roofGrid.RoofAt(c) == BiomesCoreDefOf.BMT_RockRoofStable)
			{
				__result = CanPhysicallyDropInto(c, map, canRoofPunch, allowedIndoors);
				return false;
			}

			return true;
		}

		public static bool CanPhysicallyDropInto(IntVec3 c, Map map, bool canRoofPunch, bool allowedIndoors = true)
		{
			if (!c.Walkable(map))
			{
				return false;
			}

			//RoofDef roof = c.GetRoof(map);
			//if (roof != null)
			//{
			//    if (!canRoofPunch)
			//    {
			//        return false;
			//    }
			//    if (roof.isThickRoof)
			//    {
			//        return false;
			//    }
			//}
			//if (!allowedIndoors)
			//{
			//    Room room = c.GetRoom(map);
			//    if (room != null && !room.PsychologicallyOutdoors)
			//    {
			//        return false;
			//    }
			//}
			if (!canRoofPunch)
			{
				foreach (Building allBuildingsAnimalPenMarker in map.listerBuildings.allBuildingsAnimalPenMarkers)
				{
					if (allBuildingsAnimalPenMarker.Position.GetDistrict(map) == c.GetDistrict(map))
					{
						CompAnimalPenMarker compAnimalPenMarker = allBuildingsAnimalPenMarker.TryGetComp<CompAnimalPenMarker>();
						if (compAnimalPenMarker != null && map.animalPenManager.GetPenMarkerState(compAnimalPenMarker).Enclosed)
						{
							return false;
						}
					}
				}
			}

			return true;
		}
	}

	[HarmonyPatch(typeof(DropCellFinder), "FindRaidDropCenterDistant")]
	static class Cavern_DropCellFinder_FindRaidDropCenterDistant
	{
		public static MethodInfo IntVec3UnbreachableRoofedInfo =
			AccessTools.Method(typeof(IntVec3Extensions), "UnbreachableRoofed");

		public static void Prefix(Map map, ref bool allowRoofed)
		{
			if (!allowRoofed)
			{
				BiomesMap biome = map.Biome.GetModExtension<BiomesMap>();
				if (biome != null && biome.isCavern)
				{
					allowRoofed = true;
				}
			}
		}

		// Changes
		// IntVec3.Roofed -> IntVec3Extended.Roofed
		[HarmonyPriority(Priority.First)]
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return instructions.ReplaceFunction(
				IntVec3UnbreachableRoofedInfo,
				"Roofed",
				"DropCellFinder.FindRaidDropCenterDistant");
		}
	}


	[HarmonyPatch(typeof(DropCellFinder), "CanPhysicallyDropInto")]
	static class CanPhysicallyDropIntoCavernRoofs
	{
		public static MethodInfo RoofDefUnbreachableRoofedInfo =
			AccessTools.Method(typeof(RoofDefExtensions), "UnbreachableRoofed");

		// Changes
		// IntVec3.Roofed -> IntVec3Extended.Roofed
		// in lambda function passed to TryFindCentralCell
		[HarmonyPriority(Priority.First)]
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			bool repeat = true;
			OpCode codeToMatch = OpCodes.Ldfld;
			string fieldToMatch = "isThickRoof";
			string parentMethodName = "CanPhysicallyDropInto";
			bool found = false;
			List<CodeInstruction> runningChanges = new List<CodeInstruction>();
			foreach (CodeInstruction instruction in instructions)
			{
				// Find the section calling fieldToMatch and change to RoofDefUnbreachableRoofedInfo call
				if ((repeat || !found) && instruction.opcode == codeToMatch &&
				    (instruction.operand as FieldInfo)?.Name == fieldToMatch)
				{
					runningChanges.Add(new CodeInstruction(OpCodes.Call, RoofDefUnbreachableRoofedInfo));
					// runningChanges.Add(instruction);
					found = true;
				}
				else
				{
					runningChanges.Add(instruction);
				}
			}

			if (!found)
			{
				Log.ErrorOnce(
					String.Format("[BiomesCaverns] Cannot find {0} in {1}, skipping patch", fieldToMatch, parentMethodName),
					parentMethodName.GetHashCode() + fieldToMatch.GetHashCode());
			}

			return runningChanges.AsEnumerable();
		}
	}


	//UdderlyEvelyn 3/11/22
	// joseasoler 15/04/23 These patches are not being executed.
	// After reviewing and testing the current situation with each one, I have logged tasks for each one that should
	// be implemented. Consider removing the entire class after caverns is finished.
	static class CavernRoofDetectionMethodReplacers
	{
		static CavernRoofDetectionMethodReplacers()
		{
			//These methods use RoofGrid.Roofed(IntVec3)
			typeof(GlowGrid).PatchToIgnoreCavernRoof_RoofedIntVec3("GameGlowAt");
			typeof(BeautyUtility)
				.PatchToIgnoreCavernRoof_RoofedIntVec3("CellBeauty"); //If this is enabled beauty stats will use indoor values.
			typeof(CellFinder)
				.PatchToIgnoreCavernRoof_RoofedIntVec3(
					"TryFindRandomPawnExitCell"); //If this is disabled it prevents pawns from leaving through random roofed tunnels.
			typeof(PawnsArrivalModeWorker_EdgeWalkIn)
				.PatchToIgnoreCavernRoof_RoofedIntVec3(
					"TryResolveRaidSpawnCenter"); //If disabled prevents raids from spawning under our roof type.
			typeof(RCellFinder)
				.PatchToIgnoreCavernRoof_RoofedIntVec3(
					"TryFindRandomPawnEntryCell"); //If disabled prevents pawns from entering in random spots along the edge with our roof (that use this method).
			typeof(RCellFinder)
				.PatchToIgnoreCavernRoof_RoofedIntVec3(
					"TryFindTravelDestFrom"); //I think this has to do with which direction they attempt to leave the map? Would stop them from finding edges with our roofs (if they use this method) if disabled.
			typeof(RimWorld.QuestGen.QuestNode_Root_ShuttleCrash_Rescue).PatchToIgnoreCavernRoof_RoofedIntVec3(
				"TryFindRaidWalkInPosition"); //If disabled the shuttle crash quests' raids will not be able to walk in from edges with our roof.
			//These methods use RoofGrid.RoofAt(IntVec3)
			typeof(RoofCollapseCellsFinder).PatchToIgnoreCavernRoof_RoofAtIntVec3(
				"CheckCollapseFlyingRoofAtAndAdjInternal"); //Our roof does not collapse..
			typeof(RoofCollapseUtility)
				.PatchToIgnoreCavernRoof_RoofAtIntVec3("WithinRangeOfRoofHolder"); //Our roof does not collapse..
			typeof(RoofCollapserImmediate)
				.PatchToIgnoreCavernRoof_RoofAtIntVec3("DropRoofInCellPhaseOne"); //Our roof does not collapse..
			typeof(RoofCollapserImmediate)
				.PatchToIgnoreCavernRoof_RoofAtIntVec3("DropRoofInCellPhaseTwo"); //Our roof does not collapse..
			//These methods use RoofGrid.RoofAt(int, int)
			typeof(RoofCollapseCellsFinder)
				.PatchToIgnoreCavernRoof_RoofedIntInt("ProcessRoofHolderDespawned"); //Our roof does not collapse..
			//These methods use IntVec3.Roofed(Map) extension method
			PatchProcessor.GetOriginalInstructions(AccessTools.Method("SectionLayer_IndoorMask:Regenerate"))
				.MethodReplacer(RoofedIntVec3ExtensionMethod,
					RoofIsNotNullAndNotOursIntVec3ExtensionMethod); //Special case since type inaccessible.
			typeof(RoofCollapseCellsFinder).PatchToIgnoreCavernRoof_RoofedIntVec3Extension(
				"CheckCollapseFlyingRoofAtAndAdjInternal"); //Our roof does not collapse..
			typeof(RoofCollapserImmediate).PatchToIgnoreCavernRoof_RoofedIntVec3Extension("DropRoofInCells",
				new Type[] {typeof(IntVec3), typeof(Map), typeof(List<Thing>)}); //Our roof does not collapse..
			typeof(RoofCollapserImmediate).PatchToIgnoreCavernRoof_RoofedIntVec3Extension("DropRoofInCells",
				new Type[] {typeof(IEnumerable<IntVec3>), typeof(Map), typeof(List<Thing>)}); //Our roof does not collapse..
			typeof(RoofCollapserImmediate).PatchToIgnoreCavernRoof_RoofedIntVec3Extension("DropRoofInCells",
				new Type[] {typeof(List<IntVec3>), typeof(Map), typeof(List<Thing>)}); //Our roof does not collapse..
		}

		static MethodInfo RoofedIntVec3ExtensionMethod = AccessTools.Method(typeof(GridsUtility), "Roofed");

		static MethodInfo RoofIsNotNullAndNotOursIntVec3ExtensionMethod =
			AccessTools.Method(typeof(CavernRoofDetectionMethodReplacers), "RoofIsNotNullAndNotOursIntVec3ExtensionMethod");

		static MethodInfo RoofedIntMethod = AccessTools.Method(typeof(RoofGrid), "Roofed", new Type[] {typeof(int)});

		static MethodInfo RoofIsNotNullAndNotOursIntMethod =
			AccessTools.Method(typeof(CavernRoofDetectionMethodReplacers), "RoofIsNotNullAndNotOursInt");

		static MethodInfo RoofedIntIntMethod =
			AccessTools.Method(typeof(RoofGrid), "Roofed", new Type[] {typeof(int), typeof(int)});

		static MethodInfo RoofIsNotNullAndNotOursIntIntMethod =
			AccessTools.Method(typeof(CavernRoofDetectionMethodReplacers), "RoofIsNotNullAndNotOursIntInt");

		static MethodInfo RoofedIntVec3Method =
			AccessTools.Method(typeof(RoofGrid), "Roofed", new Type[] {typeof(IntVec3)});

		static MethodInfo RoofIsNotNullAndNotOursIntVec3Method =
			AccessTools.Method(typeof(CavernRoofDetectionMethodReplacers), "RoofIsNotNullAndNotOursIntVec3");

		static MethodInfo RoofedAtIntMethod = AccessTools.Method(typeof(RoofGrid), "RoofAt", new Type[] {typeof(int)});

		static MethodInfo RoofAtButNullIfOursIntMethod =
			AccessTools.Method(typeof(CavernRoofDetectionMethodReplacers), "RoofAtButNullIfOursInt");

		static MethodInfo RoofedAtIntVec3Method =
			AccessTools.Method(typeof(RoofGrid), "RoofAt", new Type[] {typeof(IntVec3)});

		static MethodInfo RoofAtButNullIfOursIntVec3Method =
			AccessTools.Method(typeof(CavernRoofDetectionMethodReplacers), "RoofAtButNullIfOursIntVec3");

		static AccessTools.FieldRef<RoofGrid, RoofDef[]> RoofGrid_roofGrid_FieldRef =
			AccessTools.FieldRefAccess<RoofGrid, RoofDef[]>("roofGrid");

		static AccessTools.FieldRef<RoofGrid, Map> RoofGrid_map_FieldRef = AccessTools.FieldRefAccess<RoofGrid, Map>("map");

		static bool RoofIsNotNullAndNotOursIntVec3Extension(this IntVec3 c, Map map)
		{
			var roofDef = RoofGrid_roofGrid_FieldRef(map.roofGrid)[map.cellIndices.CellToIndex(c)];
			return roofDef != null && roofDef != BiomesCoreDefOf.BMT_RockRoofStable;
		}

		static bool RoofIsNotNullAndNotOursInt(this RoofGrid roofGrid, int index)
		{
			var roofDef = RoofGrid_roofGrid_FieldRef(roofGrid)[index];
			return roofDef != null && roofDef != BiomesCoreDefOf.BMT_RockRoofStable;
		}

		static bool RoofIsNotNullAndNotOursIntInt(this RoofGrid roofGrid, int x, int z)
		{
			var map = RoofGrid_map_FieldRef(roofGrid);
			var roofDef = RoofGrid_roofGrid_FieldRef(roofGrid)[map.cellIndices.CellToIndex(x, z)];
			return roofDef != null && roofDef != BiomesCoreDefOf.BMT_RockRoofStable;
		}

		static bool RoofIsNotNullAndNotOursIntVec3(this RoofGrid roofGrid, IntVec3 c)
		{
			var map = RoofGrid_map_FieldRef(roofGrid);
			var roofDef = RoofGrid_roofGrid_FieldRef(roofGrid)[map.cellIndices.CellToIndex(c)];
			return roofDef != null && roofDef != BiomesCoreDefOf.BMT_RockRoofStable;
		}

		static RoofDef RoofAtButNullIfOursInt(this RoofGrid roofGrid, int index)
		{
			var roofDef = RoofGrid_roofGrid_FieldRef(roofGrid)[index];
			return roofDef != BiomesCoreDefOf.BMT_RockRoofStable ? roofDef : null;
		}

		static RoofDef RoofAtButNullIfOursIntVec3(this RoofGrid roofGrid, IntVec3 c)
		{
			var map = RoofGrid_map_FieldRef(roofGrid);
			var roofDef = RoofGrid_roofGrid_FieldRef(roofGrid)[map.cellIndices.CellToIndex(c)];
			return roofDef != BiomesCoreDefOf.BMT_RockRoofStable ? roofDef : null;
		}

		static void PatchToIgnoreCavernRoof_RoofedIntVec3Extension(this Type type, string method, Type[] parameters = null)
		{
			PatchProcessor.GetOriginalInstructions(AccessTools.Method(type, method, parameters))
				.MethodReplacer(RoofedIntVec3ExtensionMethod, RoofIsNotNullAndNotOursIntVec3ExtensionMethod);
		}

		static void PatchToIgnoreCavernRoof_RoofedInt(this Type type, string method)
		{
			PatchProcessor.GetOriginalInstructions(AccessTools.Method(type, method))
				.MethodReplacer(RoofedIntMethod, RoofIsNotNullAndNotOursIntMethod);
		}

		static void PatchToIgnoreCavernRoof_RoofedIntInt(this Type type, string method)
		{
			PatchProcessor.GetOriginalInstructions(AccessTools.Method(type, method))
				.MethodReplacer(RoofedIntIntMethod, RoofIsNotNullAndNotOursIntIntMethod);
		}

		static void PatchToIgnoreCavernRoof_RoofedIntVec3(this Type type, string method)
		{
			PatchProcessor.GetOriginalInstructions(AccessTools.Method(type, method))
				.MethodReplacer(RoofedIntVec3Method, RoofIsNotNullAndNotOursIntVec3Method);
		}

		static void PatchToIgnoreCavernRoof_RoofAtInt(this Type type, string method)
		{
			PatchProcessor.GetOriginalInstructions(AccessTools.Method(type, method))
				.MethodReplacer(RoofedAtIntMethod, RoofAtButNullIfOursIntMethod);
		}

		static void PatchToIgnoreCavernRoof_RoofAtIntVec3(this Type type, string method)
		{
			PatchProcessor.GetOriginalInstructions(AccessTools.Method(type, method))
				.MethodReplacer(RoofedAtIntVec3Method, RoofAtButNullIfOursIntVec3Method);
		}
	}
}