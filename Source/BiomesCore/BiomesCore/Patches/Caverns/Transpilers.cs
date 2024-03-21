using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using BiomesCore.Reflections;
using HarmonyLib;
using Verse;

namespace BiomesCore.Patches.Caverns
{
	/// <summary>
	/// Helpers to implement common transpiler operations for Caverns.
	/// </summary>
	public static class Transpilers
	{
		public static List<CodeInstruction> CavernsAwarePsychologicallyOutdoors(List<CodeInstruction> instructions,
			OpCode getCellCode)
		{
			return TranspilerHelper.ReplaceCall(instructions, Methods.PsychologicallyOutdoorsMethod,
				Methods.CavernAwarePsychologicallyOutdoorsMethod,
				new List<CodeInstruction> {new CodeInstruction(getCellCode)});
		}

		public static List<CodeInstruction> CellHasNonCavernRoof(List<CodeInstruction> instructions)
		{
			return TranspilerHelper.ReplaceCall(instructions, Methods.CellRoofedMethod, Methods.CellHasNonCavernRoofMethod);
		}

		public static List<CodeInstruction> RoofGridHasNonCavernRoof(List<CodeInstruction> instructions)
		{
			return TranspilerHelper.ReplaceCall(instructions, Methods.RoofGridRoofedMethod,
				Methods.RoofGridHasNonCavernRoofMethod);
		}

		public static List<CodeInstruction> GetNonCavernRoof(List<CodeInstruction> instructions)
		{
			return TranspilerHelper.ReplaceCall(instructions, Methods.GetRoofMethod, Methods.GetNonCavernRoofMethod);
		}
	}

	[StaticConstructorOnStartup]
	public static class Methods
	{
		public static readonly MethodInfo PsychologicallyOutdoorsMethod =
			AccessTools.PropertyGetter(typeof(Room), nameof(Room.PsychologicallyOutdoors));

		public static readonly MethodInfo CavernAwarePsychologicallyOutdoorsMethod =
			AccessTools.Method(typeof(Utility), nameof(Utility.CavernAwarePsychologicallyOutdoors));

		public static readonly MethodInfo CellRoofedMethod =
			AccessTools.Method(typeof(GridsUtility), nameof(GridsUtility.Roofed));

		public static readonly MethodInfo CellHasNonCavernRoofMethod =
			AccessTools.Method(typeof(Utility), nameof(Utility.CellHasNonCavernRoof));

		public static readonly MethodInfo RoofGridRoofedMethod =
			AccessTools.Method(typeof(RoofGrid), nameof(RoofGrid.Roofed), new[] {typeof(IntVec3)});

		public static readonly MethodInfo RoofGridHasNonCavernRoofMethod =
			AccessTools.Method(typeof(Utility), nameof(Utility.RoofGridHasNonCavernRoof));

		public static readonly MethodInfo UsesOutdoorTemperatureMethod =
			AccessTools.Method(typeof(GridsUtility), nameof(GridsUtility.UsesOutdoorTemperature));

		public static readonly MethodInfo NotCavernAndUsesOutdoorTemperatureMethod =
			AccessTools.Method(typeof(Utility), nameof(Utility.NotCavernAndUsesOutdoorTemperature));

		public static readonly MethodInfo GetRoofMethod =
			AccessTools.Method(typeof(GridsUtility), nameof(GridsUtility.GetRoof));

		public static readonly MethodInfo GetNonCavernRoofMethod =
			AccessTools.Method(typeof(Utility), nameof(Utility.CellGetNonCavernRoof));

		public static readonly MethodInfo GetRoofThickIfCavernMethod =
			AccessTools.Method(typeof(Utility), nameof(Utility.GetRoofThickIfCavern));

		static Methods()
		{
			foreach (FieldInfo field in typeof(Methods).GetFields(BindingFlags.Public | BindingFlags.Static))
			{
				System.Type fieldType = field.FieldType;
				if (fieldType != typeof(MethodInfo))
				{
					BiomesCore.Error($"Transpiler.Methods: Field {field.Name} must be a MethodInfo instance.");
					continue;
				}

				if (field.GetValue(null) == null)
				{
					BiomesCore.Error($"Transpiler.Methods: Field {field.Name} must have a value.");
				}
			}
		}
	}
}