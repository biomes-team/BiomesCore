using Verse;
using RimWorld;

namespace BiomesCore.ThingComponents
{
    public class CompRuinedWithoutWater : CompTemperatureRuinable
    {
		private IntVec3 positionOnLastCheck = IntVec3.Invalid;
		bool ruinedByLackOfWater = false;

		private void DoTicks(int ticks)
		{
			//Copied this over from CompTemperatureRuinable since it's private..
			if (!Ruined)
			{
				float ambientTemperature = parent.AmbientTemperature;
				if (ambientTemperature > Props.maxSafeTemperature)
				{
					ruinedPercent += (ambientTemperature - Props.maxSafeTemperature) * Props.progressPerDegreePerTick * (float)ticks;
				}
				else if (ambientTemperature < Props.minSafeTemperature)
				{
					ruinedPercent -= (ambientTemperature - Props.minSafeTemperature) * Props.progressPerDegreePerTick * (float)ticks;
				}
				var pos = parent.Position;
				if (positionOnLastCheck != pos)
                {
					positionOnLastCheck = pos;
					if (!pos.GetTerrain(parent.MapHeld).IsWater)
					{
						ruinedPercent = 1f; //Ruined!
						ruinedByLackOfWater = true;
					}
				}
				if (ruinedPercent >= 1f)
				{
					ruinedPercent = 1f;
					parent.BroadcastCompSignal("RuinedByTemperature");
				}
				else if (ruinedPercent < 0f)
				{
					ruinedPercent = 0f;
				}
			}
		}

        public override string CompInspectStringExtra()
        {
			if (ruinedByLackOfWater)
				return "RuinedByLackOfWater".Translate();
            return base.CompInspectStringExtra();
        }
    }
}
