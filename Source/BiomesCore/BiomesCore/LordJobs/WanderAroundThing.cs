using Verse;
using Verse.AI.Group;

namespace BiomesCore.LordJobs
{
	/// <summary>
	/// Wanders around a thing until it is destroyed.
	/// </summary>
	public class WanderAroundThing : LordJob
	{
		public override bool CanBlockHostileVisitors => false;

		public override bool AddFleeToil => false;

		private Thing thing;

		public WanderAroundThing()
		{
		}

		public WanderAroundThing(Thing wanderAroundThis)
		{
			thing = wanderAroundThis;
		}

		public override StateGraph CreateGraph()
		{
			StateGraph graph = new StateGraph
			{
				StartingToil = new LordToils.WanderAroundThing(thing)
			};
			return graph;
		}

		public override void ExposeData()
		{
			Scribe_References.Look(ref thing, "thing");
		}
	}
}