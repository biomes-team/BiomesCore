using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace BiomesCore.LordToils
{
	/// <summary>
	/// Wanders around a thing.
	/// </summary>
	public class WanderAroundThing : LordToil
	{
		private readonly Thing thing;

		public WanderAroundThing(Thing followThing)
		{
			thing = followThing;
		}

		public override void UpdateAllDuties()
		{
			if (thing == null || !thing.Spawned)
			{
				return;
			}

			foreach (var ownedPawn in lord.ownedPawns)
			{
				ownedPawn.mindState.duty = new PawnDuty(BiomesCoreDefOf.BMT_WanderAroundPoint, thing)
				{
					focusSecond = thing,
					radius = -100.0F, // Do not defend!
					wanderRadius = 10.0F // Only wander around.
				};
			}
		}
	}
}