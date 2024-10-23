using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace BiomesCore
{
    public class JobGiver_StealPlayerFood : ThinkNode_JobGiver
    {
        private HungerCategory minCategory;

        private float maxLevelPercentage = 1f;

        public bool forceScanWholeMap;

        public List<PawnKindDef> thiefKinds; // Mostly used to limit this job to be used by certain animals

        public Danger maxDanger = Danger.Some;

        private static HashSet<Thing> filtered = new HashSet<Thing>();

        public override ThinkNode DeepCopy(bool resolve = true)
        {
            JobGiver_StealPlayerFood obj = (JobGiver_StealPlayerFood)base.DeepCopy(resolve);
            obj.minCategory = minCategory;
            obj.maxLevelPercentage = maxLevelPercentage;
            obj.forceScanWholeMap = forceScanWholeMap;
            return obj;
        }

        public override float GetPriority(Pawn pawn)
        {
            if (!thiefKinds.NullOrEmpty() && !thiefKinds.Contains(pawn.kindDef))
                return 0f;
            if (pawn.Faction == Faction.OfPlayer)
                return 0f;

            Need_Food food = pawn.needs.food;
            if (food == null)
                return 0f;
            if ((int)pawn.needs.food.CurCategory < 3 && FoodUtility.ShouldBeFedBySomeone(pawn))
                return 0f;
            if ((int)food.CurCategory < (int)minCategory)
                return 0f;
            if (food.CurLevelPercentage > maxLevelPercentage)
                return 0f;
            if (food.CurLevelPercentage < pawn.RaceProps.FoodLevelPercentageWantEat)
                return 9.5f;
            return 0f;
        }

        protected override Job TryGiveJob(Pawn pawn)
        {
            if (!thiefKinds.NullOrEmpty() && !thiefKinds.Contains(pawn.kindDef) || pawn.Faction == Faction.OfPlayer) return null;
            Need_Food food = pawn.needs.food;
            if (food == null || (int)food.CurCategory < (int)minCategory || food.CurLevelPercentage > maxLevelPercentage || food.CurLevelPercentage > pawn.RaceProps.FoodLevelPercentageWantEat)
                return null;

            if (!NearestFoodToSteal(pawn, maxDanger, forceScanWholeMap, out var foodSource, out var foodDef))
                return null;

            Log.Message(foodSource.Label);
            if (foodSource is Pawn pawn2)
            {
                Job job = JobMaker.MakeJob(JobDefOf.PredatorHunt, pawn2);
                job.killIncappedTarget = true;
                return job;
            }
            if (foodSource is Plant && foodSource.def.plant.harvestedThingDef == foodDef)
                return JobMaker.MakeJob(JobDefOf.Harvest, foodSource);

            if (foodSource is Building_NutrientPasteDispenser building_NutrientPasteDispenser && !building_NutrientPasteDispenser.HasEnoughFeedstockInHoppers())
            {
                Building building = building_NutrientPasteDispenser.AdjacentReachableHopper(pawn);
                if (building != null)
                {
                    ISlotGroupParent hopperSgp = building as ISlotGroupParent;
                    Job job2 = WorkGiver_CookFillHopper.HopperFillFoodJob(pawn, hopperSgp);
                    if (job2 != null)
                        return job2;
                }
                foodSource = FoodUtility.BestFoodSourceOnMap(pawn, pawn, pawn.needs.food.CurCategory == HungerCategory.Starving, out foodDef, FoodPreferability.MealLavish, false, !pawn.IsTeetotaler(), false, false, false, false, false, false, forceScanWholeMap);
                if (foodSource == null)
                    return null;
            }

            float nutrition = FoodUtility.GetNutrition(pawn, foodSource, foodDef);
            Pawn pawn3 = (foodSource.ParentHolder as Pawn_InventoryTracker)?.pawn;
            if (pawn3 != null && pawn3 != pawn)
            {
                Job job3 = JobMaker.MakeJob(JobDefOf.TakeFromOtherInventory, foodSource, pawn3);
                job3.count = FoodUtility.WillIngestStackCountOf(pawn, foodDef, nutrition);
                return job3;
            }
            Job job4 = JobMaker.MakeJob(JobDefOf.Ingest, foodSource);
            job4.count = FoodUtility.WillIngestStackCountOf(pawn, foodDef, nutrition);
            return job4;
        }

        public static bool NearestFoodToSteal(Pawn pawn, Danger maxDanger, bool forceScanWholeMap, out Thing food, out ThingDef foodDef)
        {
            food = null;
            foodDef = null;

            bool pawnCanManipulate = pawn.RaceProps.ToolUser && pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation);
            FoodPreferability minPref = FoodPreferability.DesperateOnly;
            if (pawn.NonHumanlikeOrWildMan())
                minPref = FoodPreferability.NeverForNutrition;

            Predicate<Thing> foodValidator = delegate (Thing t)
            {
                // The faction check is mostly for loose food, with the zone check handling plants
                if (t.Faction != Faction.OfPlayer && !(pawn.Map?.zoneManager?.ZoneAt(t.Position) is Zone_Growing))
                    return false;

                IntVec3 intVec;
                if (t is Building_NutrientPasteDispenser building_NutrientPasteDispenser)
                {
                    if (!pawnCanManipulate || (int)ThingDefOf.MealNutrientPaste.ingestible.preferability < (int)minPref || (int)ThingDefOf.MealNutrientPaste.ingestible.preferability > (int)FoodPreferability.MealLavish || !pawn.WillEat(ThingDefOf.MealNutrientPaste, pawn, true, false) || (t.Faction != pawn.Faction && t.Faction != pawn.HostFaction) || !building_NutrientPasteDispenser.powerComp.PowerOn || !t.InteractionCell.Standable(t.Map) || !pawn.Map.reachability.CanReachNonLocal(pawn.Position, new TargetInfo(t.InteractionCell, t.Map), PathEndMode.OnCell, TraverseParms.For(pawn, maxDanger)))
                        return false;

                    intVec = building_NutrientPasteDispenser.InteractionCell;
                }
                else
                {
                    if ((int)t.def.ingestible.preferability < (int)minPref || (int)t.def.ingestible.preferability > (int)FoodPreferability.MealLavish || !pawn.WillEat(t, pawn, true, false) || !t.def.IsNutritionGivingIngestible || !t.IngestibleNow || t.IsDessicated() || (!pawn.AnimalAwareOf(t) && !forceScanWholeMap))
                        return false;

                    int stackCount = 1;
                    float singleFoodNutrition = FoodUtility.NutritionForEater(pawn, t);
                    if (!pawn.CanReserve(t, 10, stackCount))
                        return false;

                    intVec = t.PositionHeld;
                }
                return !pawn.roping.IsRoped || intVec.InHorDistOf(pawn.roping.RopedTo.Cell, 8f);
            };
            ThingRequest thingRequest = (!((pawn.RaceProps.foodType & (FoodTypeFlags.Plant | FoodTypeFlags.Tree)) != 0)) ? ThingRequest.ForGroup(ThingRequestGroup.FoodSourceNotPlantOrTree) : ThingRequest.ForGroup(ThingRequestGroup.FoodSource);
            Thing bestThing;
            int maxRegionsToScan = -1;
            filtered.Clear();
            foreach (Thing item in GenRadial.RadialDistinctThingsAround(pawn.Position, pawn.Map, 2f, true))
                if (item is Pawn pawn2 && pawn != pawn2 && pawn.IsNonMutantAnimal && pawn.CurJob != null && pawn.CurJob.def == JobDefOf.Ingest && pawn.CurJob.GetTarget(TargetIndex.A).HasThing)
                    filtered.Add(pawn.CurJob.GetTarget(TargetIndex.A).Thing);

            Predicate<Thing> validator = delegate (Thing t)
            {
                if (!foodValidator(t))
                    return false;

                if (filtered.Contains(t))
                    return false;

                if (!(t is Building_NutrientPasteDispenser) && (int)t.def.ingestible.preferability <= 2)
                    return false;

                return !t.IsNotFresh();
            };
            bestThing = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, thingRequest, PathEndMode.OnCell, TraverseParms.For(pawn), 9999f, validator, null, 0, maxRegionsToScan, false, RegionType.Set_Passable, false);
            filtered.Clear();
            if (bestThing != null)
            {
                food = bestThing;
                foodDef = FoodUtility.GetFinalIngestibleDef(bestThing);
                return true;
            }

            if (pawn.RaceProps.predator)
            {
                List<Pawn> prey = Find.CurrentMap.mapPawns.SpawnedColonyAnimals.Where((arg) => pawn.CanReach(arg, PathEndMode.ClosestTouch, maxDanger) &&
                        arg.BodySize <= pawn.RaceProps.maxPreyBodySize).ToList();

                if (!prey.NullOrEmpty())
                {
                    prey.SortBy((arg) => arg.Position.DistanceToSquared(pawn.Position));
                    food = prey[0];
                    foodDef = FoodUtility.GetFinalIngestibleDef(food);
                    return true;
                }
            }

            return false;
        }
    }
}
