using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace BiomesCore
{
    public class JobGiver_DigUpCorpse : ThinkNode_JobGiver
    {
        public List<PawnKindDef> diggerKinds; // Mostly used to limit this job to be used by certain animals

        public List<PawnKindDef> corpseKinds; // Picks a random one of these to dig up

        public JobDef jobDef = null; // Job to give pawn. 

        public int jobTicks = 625;

        public int hashInterval = 60; // Makes it more chance based

        public bool onlyNaturalTerrain = true;

        public float minFertility = 0f; // The minimum fertility the ground should have to dig something up

        public override float GetPriority(Pawn pawn)
        {
            if (!diggerKinds.NullOrEmpty() && !diggerKinds.Contains(pawn.kindDef))
                return 0f;
            return 9.5f;
        }

        protected override Job TryGiveJob(Pawn pawn)
        {
            if (!diggerKinds.NullOrEmpty() && !diggerKinds.Contains(pawn.kindDef))
                return null;
            if (!pawn.IsHashIntervalTick(hashInterval))
                return null;
            if (onlyNaturalTerrain && !pawn.Position.GetTerrain(pawn.Map).natural)
                return null;
            if (pawn.Position.GetTerrain(pawn.Map).fertility < minFertility)
                return null;

            if (!corpseKinds.NullOrEmpty())
            {
                Pawn corpse = PawnGenerator.GeneratePawn(corpseKinds.RandomElement());
                if (corpse != null)
                {
                    Job job = JobMaker.MakeJob(jobDef, corpse, pawn);
                    job.count = jobTicks; // Uses count because I couldn't find a time related thing that I could use.
                    return job;
                }
            }
            return null;
        }
    }
}
