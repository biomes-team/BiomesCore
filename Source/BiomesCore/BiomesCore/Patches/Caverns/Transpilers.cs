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
		/// <summary>
		/// Transpiles all instances of Room.PsychologicallyOutdoors to be compatible with Caverns.
		/// </summary>
		/// <param name="instructions">Original instructions to be patched.</param>
		/// <param name="cellCode">OpCode to use for loading the current cell.</param>
		/// <returns>Patched instructions.</returns>
		public static List<CodeInstruction> CellPsychologicallyOutdoors(List<CodeInstruction> instructions, OpCode cellCode)
		{
			return TranspilerHelper.ReplaceCall(instructions, Methods.OutdoorsOriginal, Methods.OutdoorsNew,
				new List<CodeInstruction> {new CodeInstruction(cellCode)});
		}

		/// <summary>
		/// Replaces calls to GridsUtility.Roofed with IntVec3Extensions.UnbreachableRoofed.
		/// </summary>
		/// <param name="instructions"></param>
		/// <returns></returns>
		public static List<CodeInstruction> CellUnbreachableRoofed(List<CodeInstruction> instructions)
		{
			return TranspilerHelper.ReplaceCall(instructions, Methods.RoofedOriginal, Methods.RoofedPatched);
		}
	}

	[StaticConstructorOnStartup]
	internal static class Methods
	{
		public static readonly MethodInfo OutdoorsOriginal =
			AccessTools.PropertyGetter(typeof(Room), nameof(Room.PsychologicallyOutdoors));

		public static readonly MethodInfo OutdoorsNew =
			AccessTools.Method(typeof(Utility), nameof(Utility.PsychologicallyOutdoorsOrCavern));

		public static readonly MethodInfo RoofedOriginal =
			AccessTools.Method(typeof(GridsUtility), nameof(GridsUtility.Roofed));

		public static readonly MethodInfo RoofedPatched =
			AccessTools.Method(typeof(IntVec3Extensions), nameof(IntVec3Extensions.UnbreachableRoofed));

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