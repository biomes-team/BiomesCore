using Verse;

namespace BiomesCore
{
	public class TerrainCompProperties_HeatPush : TerrainCompProperties
	{
		public TerrainCompProperties_HeatPush()
		{
			compClass = typeof(TerrainComp_HeatPush);
		}

		public float pushAmount;
		public int targetTemp = 5000;
	}

	/// <summary>
	/// This terrain comp will push heat up to a maximum temperature.
	/// </summary>
	public class TerrainComp_HeatPush : TerrainComp
	{
		public TerrainCompProperties_HeatPush Props => (TerrainCompProperties_HeatPush) props;

		protected virtual bool ShouldPushHeat => true;

		protected virtual float PushAmount => Props.pushAmount;

		public override void CompTick()
		{
			base.CompTick();
			if (!ShouldPushHeat)
			{
				return;
			}

			IntVec3 position = parent.Position;
			Map map = parent.Map;
			Room room = position.GetRoom(map);

			if (room != null && room.Temperature < Props.targetTemp)
			{
				room.PushHeat(PushAmount);
			}
		}
	}
}