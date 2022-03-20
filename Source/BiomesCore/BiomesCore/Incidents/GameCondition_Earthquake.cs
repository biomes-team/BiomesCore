using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using Verse.Sound;

namespace BiomesCore
{
    public class GameCondition_Earthquake : GameCondition
    {
        public int LastCaveInTick = 0;
        public float Severity = 0;
        public CameraShaker Shaker = null;
        public float CaveInChance = .7f;

        public override string Label => "earthquake";

        public override string LabelCap => "Earthquake";

        public GameCondition_Earthquake(float severity, int duration)
        {
            Duration = duration;
            Shaker = Find.CameraDriver.shaker;
            Severity = severity;
            CaveInChance -= Severity * .01f; //Lower cave-in threshold by 1% per severity of earthquake.

            //Stuff that would happen if you do it with GameConditionMaker..
            startTick = Find.TickManager.TicksGame;
            def = BiomesCoreDefOf.Earthquake;
            uniqueID = Find.UniqueIDsManager.GetNextGameConditionID();
            PostMake();
        }

        public override void GameConditionTick()
        {
            var ticks = Find.TickManager.TicksGame;
            if (ticks % 15 == 0)
                Shaker.DoShake(Rand.Value); //Shake & Bake
            if (ticks - LastCaveInTick > 250 && Rand.Value > .7f) //30% chance per tick as long as it's been 250 ticks or more since the last time..
            {
                Find.Storyteller.incidentQueue.Add(
                    new QueuedIncident(
                        new FiringIncident(BiomesCoreDefOf.CaveIn, null, StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.Misc, Find.CurrentMap)), ticks, Duration)); //Cave-In!
                LastCaveInTick = ticks;
            }
            if (ticks % 250 == 0) //Only stun every so often.
                Find.CurrentMap.mapPawns.AllPawns.ForEach(p =>
                {
                    if (Rand.Value > .7f && p.stances != null && p.stances.stunner != null) //30% chance and it can be stunned..
                        p.stances.stunner.StunFor(250, null, false);
                });
            if (Rand.Value > .9) //10% chance per tick for another sound.
                SoundDefOf.Earthquake.PlayOneShot(SoundInfo.OnCamera(MaintenanceType.PerTick));
        }
    }
}
