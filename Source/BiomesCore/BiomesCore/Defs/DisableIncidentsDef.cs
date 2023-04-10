using System.Collections.Generic;
using RimWorld;
using Verse;

namespace BiomesCore
{
	public class DisableIncidentsDef : Def
	{
		public bool isCavern;

		public List<IncidentDef> incidents;

		public override IEnumerable<string> ConfigErrors()
		{
			foreach (var line in base.ConfigErrors())
			{
				yield return line;
			}

			if (!isCavern)
			{
				yield return $"{GetType().Name} must specify at least one BiomesMap condition.";
			}

			if (incidents.NullOrEmpty())
			{
				yield return $"{GetType().Name} must list one or more disabled incidents.";
			}
		}
	}
}