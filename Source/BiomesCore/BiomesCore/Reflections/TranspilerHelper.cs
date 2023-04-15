using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace BiomesCore.Reflections
{
	public static class TranspilerHelper
	{
		/// <summary>
		/// Replaces a call with a different one.
		/// </summary>
		/// <param name="instructions">Original set of instructions.</param>
		/// <param name="original">Original method.</param>
		/// <param name="changed">New method.</param>
		/// <param name="additionalParameters">Code instructions required for obtaining additional parameters.</param>
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
}