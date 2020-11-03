using HarmonyLib;
using RimWorld;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace BiomesCore.Patches
{
    [HarmonyPatch(typeof(ThingDefGenerator_Meat), "ImpliedMeatDefs")]
    class BiomesCoreMeatGenerator
    {
        static IEnumerable<ThingDef> Postfix(IEnumerable<ThingDef> meatThings)
        {
            foreach (var thingDef in meatThings)
            {
                if (thingDef.defName == "Meat_BiomesIslands_WhiteShark")
                {
                    thingDef.graphicData.texPath = "Things/Item/Resource/MeatFoodRaw/Meat_Insect";
                }
                if (thingDef.defName == "Meat_BiomesIslands_BlueSeaSnail")
                {
                    thingDef.graphicData.texPath = "Things/Item/Resource/MeatFoodRaw/Meat_Insect";
                }
                if (thingDef.defName == "Meat_BiomesIslands_RimCrab")
                {
                    thingDef.graphicData.texPath = "Things/Item/Resource/MeatFoodRaw/Meat_Insect";
                    thingDef.description = "Raw butchered crab flesh. Delicious when cooked into meals, but can also be eaten raw.";
                    thingDef.ingestible.specialThoughtAsIngredient = ThoughtDef.Named("BiomesIslands_Crab");
                }
                if (thingDef.defName == "Meat_BiomesIslands_Whale")
                {
                    thingDef.graphicData.texPath = "Things/Item/Resource/MeatFoodRaw/Meat_Insect";
                    thingDef.description = "Raw fat from an aquatic mammal. Not as tasty as meat, but nutrient dense.";
                    thingDef.stackLimit = 150;
                    //thingDef.ingestible.specialThoughtAsIngredient = ThoughtDef.Named("BiomesIslands_Whale");
                }
                yield return thingDef;
            }
        }
    }

//    [HarmonyPatch(typeof(Pawn), "ButcherProducts")]
//    class BiomesCoreBlubberProduct
//    {
//        IEnumerable<Thing> Postfix(IEnumerable<Thing> thing)
//        {
//            Log.Error("Before or after real error?");
//            foreach (var thing2 in thing)
//            {
//                yield return thing2;
//            }
//        }
//    }
}
