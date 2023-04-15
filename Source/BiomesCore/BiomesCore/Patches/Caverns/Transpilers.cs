using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
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
			return ReplaceCall(instructions, Methods.OutdoorsOriginal, Methods.OutdoorsNew,
				new List<CodeInstruction> {new CodeInstruction(cellCode)});
		}

		/// <summary>
		/// Replaces calls to GridsUtility.Roofed with IntVec3Extensions.UnbreachableRoofed.
		/// </summary>
		/// <param name="instructions"></param>
		/// <returns></returns>
		public static List<CodeInstruction> CellUnbreachableRoofed(List<CodeInstruction> instructions)
		{
			return ReplaceCall(instructions, Methods.RoofedOriginal, Methods.RoofedPatched);
		}

		/// <summary>
		/// Replaces a call with a different one.
		/// </summary>
		/// <param name="instructions">Original set of instructions.</param>
		/// <param name="original">Original method.</param>
		/// <param name="changed">New method.</param>
		/// <param name="additionalParameters">Additional OpcCodes to execute before the call to get more parameters.</param>
		/// <returns>Modified list of instructions.</returns>
		public static List<CodeInstruction> ReplaceCall(List<CodeInstruction> instructions, MethodInfo original,
			MethodInfo changed, List<CodeInstruction> additionalParameters = null)
		{
			var newInstructions = new List<CodeInstruction>();
			foreach (var line in instructions)
			{
				if (line.operand as MethodInfo == original && (line.opcode == OpCodes.Callvirt || line.opcode == OpCodes.Call))
				{
					if (additionalParameters != null)
					{
						newInstructions.AddRange(additionalParameters);
					}

					newInstructions.Add(new CodeInstruction(OpCodes.Call, changed));
				}
				else
				{
					newInstructions.Add(line);
				}
			}

			return newInstructions;
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