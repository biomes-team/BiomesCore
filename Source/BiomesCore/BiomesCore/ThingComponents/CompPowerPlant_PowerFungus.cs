using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace BiomesCore
{
	public class CompPowerPlant_PowerFungus : CompPowerPlant
	{
		

		public CompAffectedByFacilities Facilities => parent.TryGetComp<CompAffectedByFacilities>();

		public override void UpdateDesiredPowerOutput()
		{
			base.PowerOutput = GetAllPowerPlants();
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
			int powerPlantCost = (int)powerPlantCount;
			return powerPlantCost;
		}

	}

}
