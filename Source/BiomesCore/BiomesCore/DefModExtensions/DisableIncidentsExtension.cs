using System.Collections.Generic;
using RimWorld;
using Verse;

namespace BiomesCore.DefModExtensions
{
	public class DisableIncidentsExtension : DefModExtension
	{
		public List<IncidentDef> disabledIncidents;

		public override IEnumerable<string> ConfigErrors()
		{
			foreach (var line in base.ConfigErrors())
			{
				yield return line;
			}

			if (disabledIncidents.NullOrEmpty())
			{
				yield return $"{GetType().Name} must specify one or more disabled incidents.";
			}
		}
	}
}