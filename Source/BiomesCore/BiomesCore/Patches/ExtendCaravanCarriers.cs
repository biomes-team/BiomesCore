using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BiomesCore.DefModExtensions;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BiomesCore.Patches
{
	[HarmonyPatch(typeof(PawnGroupKindWorker_Trader), nameof(PawnGroupKindWorker_Trader.CanGenerateFrom))]
	internal static class PawnGroupKindWorker_Trader_CanGenerateFrom
	{
		private static void Postfix(ref bool __result, PawnGroupMakerParms parms, PawnGroupMaker groupMaker)
		{
			if (!__result && Find.WorldGrid.TilesCount > parms.tile)
			{
				var biomeDef = Find.WorldGrid.tiles[parms.tile].biome;
				var extension = biomeDef?.GetModExtension<BiomesMap>();
				__result = extension != null && !extension.extraCarriers.NullOrEmpty();
			}
		}
	}

	[HarmonyPatch(typeof(PawnGroupKindWorker_Trader), "GenerateCarriers")]
	internal static class PawnGroupKindWorker_Trader_GenerateCarriers
	{
		private static List<PawnGenOption> GetExtendedCarriers(List<PawnGenOption> originalCarriers,
			PawnGroupMakerParms parms)
		{
			var biomeDef = Find.WorldGrid.tiles[parms.tile].biome;
			var extension = biomeDef?.GetModExtension<BiomesMap>();
			if (extension == null || extension.extraCarriers.NullOrEmpty())
			{
				return originalCarriers;
			}

			var result = new List<PawnGenOption>(originalCarriers);
			var totalOriginalWeight = originalCarriers.Sum(carrier => carrier.selectionWeight);
			var totalExtendedWeight = extension.extraCarriers.Sum(carrier => carrier.selectionWeight);
			var weightFactor = extension.extraCarriersRelativeWeight * (totalOriginalWeight / totalExtendedWeight);

			foreach (var carrier in extension.extraCarriers)
			{
				result.Add(new PawnGenOption
				{
					kind = carrier.kind,
					selectionWeight = carrier.selectionWeight * weightFactor
				});
			}

			return result;
		}

		public static readonly FieldInfo CarriersOriginal =
			AccessTools.Field(typeof(PawnGroupMaker), nameof(PawnGroupMaker.carriers));

		public static readonly MethodInfo CarriersExtended =
			AccessTools.Method(typeof(PawnGroupKindWorker_Trader_GenerateCarriers), nameof(GetExtendedCarriers));

		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			foreach (var line in instructions)
			{
				yield return line;
				if (line.operand as FieldInfo == CarriersOriginal && line.opcode == OpCodes.Ldfld)
				{
					yield return new CodeInstruction(OpCodes.Ldarg_1); // PawnGroupMakerParms
					yield return new CodeInstruction(OpCodes.Call, CarriersExtended);
				}
			}
		}
	}
}