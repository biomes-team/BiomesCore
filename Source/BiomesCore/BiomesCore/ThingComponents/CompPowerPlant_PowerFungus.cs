using System.Collections.Generic;
using BiomesCore.ModSettings;
using RimWorld;
using Verse;

namespace BiomesCore
{
	public class CompPowerPlant_PowerFungus : CompPowerPlant
	{
		public CompAffectedByFacilities Facilities => parent.TryGetComp<CompAffectedByFacilities>();

		public override void UpdateDesiredPowerOutput()
		{
			if ((breakdownableComp != null && breakdownableComp.BrokenDown) ||
			    (refuelableComp != null && !refuelableComp.HasFuel) || (flickableComp != null && !flickableComp.SwitchIsOn) ||
			    (autoPoweredComp != null && !autoPoweredComp.WantsToBeOn) || (toxifier != null && !toxifier.CanPolluteNow) ||
			    !base.PowerOn)
			{
				base.PowerOutput = 0f;
			}
			else
			{
				base.PowerOutput = GetAllPowerPlants();
			}
		}

		public int GetAllPowerPlants()
		{
			float powerPlantCount = 0;
			if (Facilities != null)
			{
				List<Thing> linkedFacilitiesListForReading = Facilities.LinkedFacilitiesListForReading;
				foreach (Thing item in linkedFacilitiesListForReading)
				{
					if (item.def.plant != null)
					{
						float powerPotential = item.def.plant.LifespanDays;
						powerPlantCount += powerPotential;
					}
				}
			}

			return (int) (Settings.Values.PowerGenFungusMultiplier * powerPlantCount / 100.0F);
		}
	}
}