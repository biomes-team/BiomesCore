using System;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;
using System.Linq;
using System.Reflection;

namespace WaterWalker
{
    public class WaterWalkerExtension : DefModExtension { }

    [StaticConstructorOnStartup]
    public static class WaterWalkerPatcher
    {
        static WaterWalkerPatcher()
        {
            var harmony = new Harmony("draegon.waterwalker");

            harmony.Patch(AccessTools.Method(typeof(RimWorld.JobGiver_GetRest), "TryGiveJob"),
                postfix: new HarmonyMethod(typeof(JobGiver_GetRest_TryGiveJob_Patch), nameof(JobGiver_GetRest_TryGiveJob_Patch.Postfix)));

            harmony.Patch(AccessTools.Method(typeof(Pawn_PathFollower), "CostToMoveIntoCell", new[] { typeof(IntVec3) }),
                postfix: new HarmonyMethod(typeof(Pawn_PathFollower_CostToMoveIntoCell_Patch), nameof(Pawn_PathFollower_CostToMoveIntoCell_Patch.Postfix)));

            harmony.Patch(AccessTools.Method(typeof(GenGrid), "WalkableBy", new[] { typeof(IntVec3), typeof(Map), typeof(Pawn) }),
                postfix: new HarmonyMethod(typeof(GenGrid_WalkableBy_Patch), nameof(GenGrid_WalkableBy_Patch.Postfix)));

            harmony.Patch(AccessTools.Method(typeof(Reachability), "CanReach", new[] { typeof(IntVec3), typeof(LocalTargetInfo), typeof(PathEndMode), typeof(TraverseParms) }),
                prefix: new HarmonyMethod(typeof(Reachability_CanReach_Patch), nameof(Reachability_CanReach_Patch.Prefix)));
        }

        internal static void TryReplaceWithWaterSleepJobIfNeeded(ref Job __result, Pawn pawn)
        {
            if (pawn == null || !pawn.def.HasModExtension<WaterWalkerExtension>()) return;
            Map map = pawn.Map;
            if (map == null) return;

            if (__result != null)
            {
                if (__result.targetA.IsValid && !__result.targetA.HasThing)
                {
                    IntVec3 targ = __result.targetA.Cell;
                    if (targ.InBounds(map) && targ.GetTerrain(map)?.IsWater == true)
                    {
                        return;
                    }
                }
            }

            JobDef prefer = JobDefOf.LayDown;

            bool IsAcceptableWaterCell(IntVec3 c)
            {
                if (!c.InBounds(map)) return false;
                var terr = c.GetTerrain(map);
                if (terr == null || !terr.IsWater) return false;
                if (!c.WalkableBy(map, pawn)) return false;
                if (!pawn.CanReach(c, PathEndMode.OnCell, Danger.Deadly)) return false;
                if (!c.Standable(map)) return false;
                if (pawn.IsColonist && c.IsForbidden(pawn)) return false;
                return true;
            }

            if (IsAcceptableWaterCell(pawn.Position))
            {
                __result = JobMaker.MakeJob(prefer, pawn.Position);
                return;
            }

            float searchRadius = pawn.RaceProps.Animal ? 20f : 15f;
            if (CellFinder.TryFindRandomReachableNearbyCell(pawn.Position, map, searchRadius, TraverseParms.For(pawn), IsAcceptableWaterCell, null, out IntVec3 found))
            {
                __result = JobMaker.MakeJob(prefer, found);
                return;
            }

            int fallbackRadius = 80;
            if (CellFinder.TryFindRandomCellNear(pawn.Position, map, fallbackRadius, IsAcceptableWaterCell, out found))
            {
                __result = JobMaker.MakeJob(prefer, found);
                return;
            }
        }
    }

    [HarmonyPatch(typeof(RimWorld.JobGiver_GetRest), "TryGiveJob")]
    public static class JobGiver_GetRest_TryGiveJob_Patch
    {
        public static void Postfix(Pawn pawn, ref Job __result)
        {
            WaterWalkerPatcher.TryReplaceWithWaterSleepJobIfNeeded(ref __result, pawn);
        }
    }

    [HarmonyPatch(typeof(Pawn_PathFollower), "CostToMoveIntoCell", new[] { typeof(IntVec3) })]
    public static class Pawn_PathFollower_CostToMoveIntoCell_Patch
    {
        private static readonly AccessTools.FieldRef<Pawn_PathFollower, Pawn> PawnField =
            AccessTools.FieldRefAccess<Pawn_PathFollower, Pawn>("pawn");

        public static void Postfix(ref float __result, Pawn_PathFollower __instance, IntVec3 c)
        {
            if (__instance == null) return;
            Pawn pawn = PawnField(__instance);
            if (pawn == null || !pawn.def.HasModExtension<WaterWalkerExtension>() || pawn.Map == null) return;
            var terrain = c.GetTerrain(pawn.Map);
            if (terrain == null || !terrain.IsWater)
            {
                __result = 999999f;
            }
        }
    }

    [HarmonyPatch(typeof(GenGrid), "WalkableBy")]
    public static class GenGrid_WalkableBy_Patch
    {
        public static void Postfix(ref bool __result, IntVec3 c, Map map, Pawn pawn)
        {
            if (!__result) return;
            if (pawn != null && pawn.def.HasModExtension<WaterWalkerExtension>())
            {
                if (map == null) { __result = false; return; }
                var terrain = c.GetTerrain(map);
                if (terrain == null || !terrain.IsWater)
                {
                    __result = false;
                }
            }
        }
    }

    // Overrides vanilla reachability for water walkers
    [HarmonyPatch(typeof(Reachability), "CanReach", new[] { typeof(IntVec3), typeof(LocalTargetInfo), typeof(PathEndMode), typeof(TraverseParms) })]
    public static class Reachability_CanReach_Patch
    {
        private const int MAX_VISITED_NODES = 20000;

        public static bool Prefix(IntVec3 start, LocalTargetInfo dest, TraverseParms traverseParams, ref bool __result)
        {
            Pawn pawn = traverseParams.pawn;
            if (pawn == null || !pawn.def.HasModExtension<WaterWalkerExtension>())
            {
                return true; // Not our pawn, let vanilla logic run
            }

            // For our pawn, we take over completely, never let vanilla run
            Map map = pawn.Map;
            if (map == null)
            {
                __result = false;
                return false;
            }

            // Both start and destination MUST be water for a path to be possible.
            IntVec3 destCell = dest.Cell;
            if (!start.InBounds(map) || !start.GetTerrain(map).IsWater || !destCell.InBounds(map) || !destCell.GetTerrain(map).IsWater)
            {
                __result = false;
                return false;
            }

            // If start and dest are both valid water, run custom BFS to check for a water-only path
            var q = new Queue<IntVec3>();
            var visited = new HashSet<IntVec3>();
            q.Enqueue(start);
            visited.Add(start);
            int visitedCount = 0;
            while (q.Count > 0)
            {
                if (++visitedCount > MAX_VISITED_NODES) { __result = false; return false; }
                IntVec3 cur = q.Dequeue();
                if (cur == destCell) { __result = true; return false; }
                for (int i = 0; i < 8; i++)
                {
                    IntVec3 nc = cur + GenAdj.AdjacentCells[i];
                    if (!nc.InBounds(map) || visited.Contains(nc)) continue;
                    visited.Add(nc);
                    var terr = nc.GetTerrain(map);
                    if (terr != null && terr.IsWater)
                    {
                        q.Enqueue(nc);
                    }
                }
            }

            // No path found between the two water bodies.
            __result = false;
            return false;
        }
    }

    // Unbeaching if stuck on land, and dying if no water can be found

    [DefOf]
    public static class WaterWalkerDefOf
    {
        public static JobDef WaterWalker_Unbeach;
        public static JobDef WaterWalker_Die;
    }

    public class JobGiver_Unbeach : ThinkNode_JobGiver
    {
        private const int MaxCellsToScan = 20000;

        protected override Job TryGiveJob(Pawn pawn)
        {
            if (!pawn.def.HasModExtension<WaterWalkerExtension>()) return null;
            if (!pawn.Spawned || pawn.Position.GetTerrain(pawn.Map).IsWater) return null;

            var queue = new Queue<IntVec3>();
            var visited = new HashSet<IntVec3>();
            queue.Enqueue(pawn.Position);
            visited.Add(pawn.Position);
            int cellsScanned = 0;

            while (queue.Count > 0)
            {
                if (++cellsScanned > MaxCellsToScan) break;

                IntVec3 current = queue.Dequeue();
                for (int i = 0; i < 8; i++)
                {
                    IntVec3 neighbor = current + GenAdj.AdjacentCells[i];
                    if (!neighbor.InBounds(pawn.Map) || !visited.Add(neighbor)) continue;

                    if (neighbor.GetTerrain(pawn.Map).IsWater)
                    {
                        Job job = JobMaker.MakeJob(WaterWalkerDefOf.WaterWalker_Unbeach, pawn);
                        job.targetB = new LocalTargetInfo(neighbor);
                        return job;
                    }
                    queue.Enqueue(neighbor);
                }
            }

            return JobMaker.MakeJob(WaterWalkerDefOf.WaterWalker_Die, pawn);
        }
    }

    public class JobDriver_Unbeach : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            Toil teleportToil = new Toil();
            teleportToil.initAction = () =>
            {
                IntVec3 destination = job.targetB.Cell;
                pawn.Position = destination;
                pawn.pather.StopDead();
            };
            teleportToil.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return teleportToil;
        }
    }

    public class JobDriver_Die : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            Toil die = new Toil();
            die.initAction = () =>
            {
                pawn.Destroy(DestroyMode.Vanish);
            };
            die.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return die;
        }
    }
}