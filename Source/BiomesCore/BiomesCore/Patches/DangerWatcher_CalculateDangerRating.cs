using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using System.Reflection.Emit;
using Verse;
using RimWorld;

namespace BiomesCore.Patches
{
    [HarmonyPatch(typeof(DangerWatcher), "CalculateDangerRating")]
    public class DangerWatcher_CalculateDangerRating
    {
        /// <summary>
        /// Holds the last value for danger level on a per-map basis 
        /// </summary>
        public static Dictionary<int, float> DangerRatingPerMap = new Dictionary<int, float>();
        internal static void _putDangerRatinglPerMap(float value, int key)
        {
            //Log.Message("[BiomesCore] Assigning danger rating of " + value + " for map with ID " + key + " to the dictionary..");
            DangerRatingPerMap[key] = value;
        }

        [HarmonyDebug]
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool match = false;
            var insts = instructions.ToList(); //So we can index into it.
            for (int i = 0; i < insts.Count; i++)
            {
                yield return insts[i]; //Send the instruction as normal..
                if (!match && i > 1 && insts[i - 1].IsStloc() && insts[i].IsLdloc()) //If it's the pattern we are matching.. (in this case the bit of IL after the variable we want is assigned)
                {
                    match = true; //Prevent further matching..
                    yield return new CodeInstruction(OpCodes.Dup); //Copy the variable to the stack an extra time.
                    yield return new CodeInstruction(OpCodes.Ldarg_0); //Get argument 0 to the method we're in, which is the instance ("this") and put it on the stack.
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(DangerWatcher), "map")); //Get map for that instance (this is now on the stack instead).
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Map), "uniqueID")); //Get uniqueID for that map (this is now on the stack instead).
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(DangerWatcher_CalculateDangerRating), "_putDangerLevelPerMap")); //Call our method using the stuff on the stack, leaving it how it was before we did anything.
                }
            }
        }
    }
}
