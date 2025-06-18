using BiomesCore.DefModExtensions;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace BiomesCore.Patches
{
    [HarmonyPatch(typeof(JobDriver_PlantWork), "MakeNewToils")]
    public class AdditionalHarvestDropsPatch
    {
        static void Prefix(JobDriver_PlantWork __instance, out float __state)
        {
            if ((__instance.job.targetA.Thing)?.def?.HasModExtension<AdditionalHarvestDrops>() == true)
            {
                __state = __instance.pawn.records.GetValue(RecordDefOf.PlantsHarvested);
            }
            else
            {
                __state = -1;
            }
            //Log.Message("WoahEpic " + __state);


        }

        static IEnumerable<Toil> Postfix(IEnumerable<Toil> values, JobDriver_PlantWork __instance, float __state)
        {
            if (__state != -1)
            {
                foreach (var item in values)
                {
                    yield return item;
                }
                yield break;
            }

            List<Toil> iter = values.ToList();
            for (int i = 0; i < iter.Count; i++)
            {
                if (iter[i].debugName == "MakeNewToils")
                {
                    AdditionalHarvestDrops drops = (__instance.job.targetA.Thing)?.def?.GetModExtension<AdditionalHarvestDrops>();
                    if (drops == null)
                    {
                        break;
                    }
                    Action act = iter[i].tickAction;
                    Plant plant = (Plant)__instance.job.targetA.Thing;
                    Pawn pawn = __instance.pawn;
                    iter[i].tickAction = () =>
                    {
                        act.Invoke();
                        if (__state != iter[i].actor.records.GetValue(RecordDefOf.PlantsHarvested))
                        {
                            DoAdditional(drops,plant,pawn);
                        }

                    };
                    break;
                }
            }

            foreach (var item in iter)
            {
                yield return item;
            }
        }

        static void DoAdditional(AdditionalHarvestDrops drops, Plant plant, Pawn pawn)
        {

            for (int i = 0; i < drops.defs.Count; i++)
            {
                ThingDef def = drops.defs[i];
                StatDef stat = def.IsDrug || plant.def.plant.drugForHarvestPurposes ? StatDefOf.DrugHarvestYield : StatDefOf.PlantHarvestYield;
                float statValue = pawn.GetStatValue(stat);
                int num =  YieldNow(plant, drops.yields[i]);
                if ((double)statValue > 1.0)
                    num = GenMath.RoundRandom((float)num * statValue);

                Thing thing = ThingMaker.MakeThing(def);
                thing.stackCount = num;
                GenPlace.TryPlaceThing(thing, pawn.Position, pawn.Map, ThingPlaceMode.Near);
            }
        }

        public static int YieldNow(Plant plant, float yield)
        {
            if (!plant.CanYieldNow())
                return 0;
            float f = yield * (float)(0.5 + (double)Mathf.InverseLerp(plant.def.plant.harvestMinGrowth, 1f, plant.Growth) * 0.5) * Mathf.Lerp(0.5f, 1f, (float)plant.HitPoints / (float)plant.MaxHitPoints);
            if (plant.def.plant.harvestYieldAffectedByDifficulty)
                f *= Find.Storyteller.difficulty.cropYieldFactor;
            return GenMath.RoundRandom(f);
        }
    }

    [HarmonyPatch(typeof(MinifiedTree), "Destroy")]
    public class AdditionalHarvestDropsPatch1
    {
        static void Postfix(MinifiedTree __instance)
        {
            AdditionalHarvestDrops drops = __instance.InnerThing.def.GetModExtension<AdditionalHarvestDrops>();

            if(drops == null)
            {
                return;
            }

            Caravan anyParent = ThingOwnerUtility.GetAnyParent<Caravan>((Thing)__instance);

            List<Thing> thingList = new List<Thing>();
            for (int i = 0; i < drops.defs.Count; i++)
            {
                ThingDef def = drops.defs[i];
                int a = AdditionalHarvestDropsPatch.YieldNow((Plant)__instance.InnerThing, drops.yields[i]);
                int num;
                for (; a > 0; a -= num)
                {
                    num = Mathf.Min(a, def.stackLimit);
                    Thing thing = ThingMaker.MakeThing(def);
                    thing.stackCount = num;
                    thingList.Add(thing);
                }
            }

            IntVec3 center = anyParent == null ? __instance.PositionHeld : IntVec3.Invalid;
            Map map = anyParent == null ? __instance.MapHeld : (Map)null;

            if (anyParent != null)
            {
                foreach (Thing thing in thingList)
                    anyParent.AddPawnOrItem(thing, true);
            }
            else if (__instance.ParentHolder is ActiveTransporterInfo parentHolder)
            {
                foreach (Thing thing in thingList)
                    parentHolder.innerContainer.TryAdd(thing);
            }
            else
            {
                if (map == null)
                    return;
                foreach (Thing thing in thingList)
                    GenPlace.TryPlaceThing(thing, center, map, ThingPlaceMode.Near);
            }
        }
    }
}
