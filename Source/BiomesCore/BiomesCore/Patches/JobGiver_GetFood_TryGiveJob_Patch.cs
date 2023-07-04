using System;
using System.Collections.Generic;
using BiomesCore.DefModExtensions;
using HarmonyLib;
using System.Linq;
using Verse;
using RimWorld;
using Verse.AI;

namespace BiomesCore.Patches
{
    [HarmonyPatch(typeof(JobGiver_GetFood), "TryGiveJob")]
    public static class JobGiver_GetFood_TryGiveJob_Patch
    {
        public static bool Prefix(ref Job __result, Pawn pawn, HungerCategory ___minCategory,
            float ___maxLevelPercentage, JobGiver_GetFood __instance)
        {
            var extension = pawn.def.GetModExtension<Biomes_AnimalControl>();
            Need_Food food = pawn.needs.food;
            if (extension == null || food == null)
            {
                return true;
            }

            if (!extension.eatWhenFed && (int) food.CurCategory < (int) ___minCategory ||
                food.CurLevelPercentage > ___maxLevelPercentage)
            {
                return true;
            }

            // Manhunter animals ignore their custom feeding behaviour unless they are desperately hungry.
            bool desperate = pawn.needs.food.CurCategory == HungerCategory.Starving;
            if (!desperate && pawn.InMentalState && (pawn.MentalStateDef == MentalStateDefOf.Manhunter ||
                    pawn.MentalStateDef == MentalStateDefOf.ManhunterPermanent))
            {
                return true;
            }

            if (extension.isBloodDrinkingAnimal)
            {
                var nearestPawn = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map,
                    ThingRequest.ForGroup(ThingRequestGroup.Pawn), PathEndMode.OnCell,
                    TraverseParms.For(TraverseMode.NoPassClosedDoors), 100.0F,
                    thing => IsValidBloodfeedingTarget(thing, pawn));

                if (nearestPawn != null)
                {
                    __result = JobMaker.MakeJob(BiomesCoreDefOf.BC_BloodDrinking, nearestPawn);
                    return false;
                }
            }

            if (extension.isBottomFeeder)
            {
                var bottomFeeder = pawn.GetComp<CompBottomFeeder>();
                if (bottomFeeder != null)
                {
                    var nearestCell = IntVec3.Invalid;
                    pawn.Map.floodFiller.FloodFill(pawn.Position, c => true, c =>
                    {
                        if (!c.GetTerrain(pawn.Map).HasTag(bottomFeeder.Props.feedingTerrainTag)) return false;
                        if (!pawn.CanReserveAndReach(c, PathEndMode.OnCell, Danger.Deadly)) return false;
                        if (c.IsForbidden(pawn) && !desperate)
                        {
                            return false;
                        }
                        nearestCell = c;
                        return true;
                    });
                    
                    if (nearestCell.IsValid)
                    {
                        __result = JobMaker.MakeJob(BiomesCoreDefOf.BC_BottomFeeder, nearestCell);
                        return false;
                    }
                }
            }
            
            if (extension.isCustomThingEater)
            {
                var customThingEater = pawn.GetComp<CompCustomThingEater>();
                if (customThingEater != null)
                {
                    Thing nearestCustomThing = CustomThingEater(pawn, customThingEater, desperate);

                    if (nearestCustomThing != null && !nearestCustomThing.Destroyed)
                    {
                        __result = JobMaker.MakeJob(BiomesCoreDefOf.BC_EatCustomThing, nearestCustomThing);
                        return false;
                    }
                }
            }

            // Since this job giver is called more frequently for eatWhenFed animals, the vanilla code should only be
            // allowed to execute if the animal is actually hungry. Otherwise they will consume every piece of regular
            // food in a frenzy. See JobGiver_GetFood_GetPriority_Patch for details.
            return __instance.GetPriority(pawn) > 6.6F;
        }

        private static bool IsValidBloodfeedingTarget(Thing victimThing, Pawn feeder)
        {
            return victimThing is Pawn victim && feeder.CanReserve(victim) &&
                   victim.def != feeder.def &&
                   victim.RaceProps.IsFlesh &&
                   victim.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.BloodLoss) is null;
        }

        private static bool RequestedThingFromReachableRegion(Region region, Pawn pawn, bool desperate, ThingRequest request, ref Thing filth)
        {
            var filthList = region.ListerThings.ThingsMatching(request);

            for (int index = 0; index < filthList.Count; ++index)
            {
                var thing = filthList[index];
                if ((desperate || !thing.IsForbidden(pawn)) && pawn.CanReserve(thing) &&
                    ReachabilityWithinRegion.ThingFromRegionListerReachable(thing, region, PathEndMode.ClosestTouch, pawn))
                {
                    filth = thing;
                    break;
                }
            }

            return filth != null;
        }

        private static bool CellHasReservableThing(IntVec3 processCell, Pawn pawn,
            Dictionary<ThingDef, float>.KeyCollection acceptableThings, ref Thing customThing)
        {
            var thingList = processCell.GetThingList(pawn.Map);
            for (var index = 0; index < thingList.Count; ++index)
            {
                var thing = thingList[index];
                if (acceptableThings.Contains(thing.def) && pawn.CanReserve(thing))
                {
                    customThing = thing;
                    return true;
                }
            }

            return false;
        }

        private static Thing CustomThingEater(Pawn pawn, CompCustomThingEater extension, bool desperate)
        {
            const int maxFilthRegions = 18;
            const int maxLookupCells = 3200;

            if (!extension.TryLookForFood())
            {
                return null;
            }

            Thing customThing = null;
            var position = pawn.Position;
            var traverseParams = TraverseParms.For(pawn);

            if (extension.Props.filthNutrition > 0.0F)
            {
                // Region look-up optimized for filth.
                // This might not necessarily yield the closest edible filth, but it will always be very close.
                bool RegionEntryCondition(Region from, Region to) => to.Allows(traverseParams, false);
                var request = ThingRequest.ForGroup(ThingRequestGroup.Filth);
                var regionProcessor = (RegionProcessor)
                    (region => RequestedThingFromReachableRegion(region, pawn, desperate, request, ref customThing));
                RegionTraverser.BreadthFirstTraverse(pawn.Position, pawn.Map, RegionEntryCondition, regionProcessor,
                    maxFilthRegions);
            }

            if (customThing != null || extension.Props.thingsToNutritionMapper.Count == 0)
            {
                return customThing;
            }

            // Generic cell look-up for any other items. Slower than region look-ups.
            var acceptableThings = extension.Props.thingsToNutrition.Keys;
            bool CellCheckCondition(IntVec3 checkCell) => (!checkCell.IsForbidden(pawn) || desperate) &&
                pawn.Map.reachability.CanReach(checkCell, checkCell, PathEndMode.OnCell, traverseParams);
            Func<IntVec3, bool> processor = cell => CellHasReservableThing(cell, pawn, acceptableThings, ref customThing);
            pawn.Map.floodFiller.FloodFill(position, CellCheckCondition, processor, maxLookupCells);

            return customThing;
        }
    }
}
