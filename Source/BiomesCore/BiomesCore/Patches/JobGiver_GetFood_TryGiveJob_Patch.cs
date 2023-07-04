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

            bool desperate = pawn.needs.food.CurCategory == HungerCategory.Starving;

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

        private static Thing CustomThingEater(Pawn pawn, CompCustomThingEater extension, bool desperate)
        {
            if (!extension.TryLookForFood())
            {
                return null;
            }

            Thing nearestCustomThing = null;
            var acceptableThings = extension.Props.thingsToNutrition.Keys;
            pawn.Map.floodFiller.FloodFill(pawn.Position,
                checkCell => (!checkCell.IsForbidden(pawn) || desperate) &&
                    pawn.Map.reachability.CanReach(checkCell,checkCell, PathEndMode.OnCell, TraverseParms.For(pawn)),
                processCell =>
            {
                foreach (var thing in processCell.GetThingList(pawn.Map))
                {
                    if (acceptableThings.Contains(thing.def) && pawn.CanReserve(thing))
                    {
                        nearestCustomThing = thing;
                        return true;
                    }
                }

                return false;
            }, 1600);

            return nearestCustomThing;
        }
    }
}
