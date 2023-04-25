using System.Collections.Generic;
using Verse;

namespace BiomesCore.ThingComponents
{
	public abstract class CompProperties_CompPlantGraphic : CompProperties
	{
		public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
		{
			foreach (var line in base.ConfigErrors(parentDef))
			{
				yield return line;
			}

			if (!parentDef.IsPlant)
			{
				yield return $"{GetType().Name} can only be applied to plants.";
			}
		}

		protected abstract void Initialize(ThingDef parentDef);

		public override void PostLoadSpecial(ThingDef parentDef)
		{
			LongEventHandler.ExecuteWhenFinished(() => Initialize(parentDef));
		}
	}

	public abstract class CompPlantGraphic : ThingComp
	{
		public abstract bool Active();

		public abstract Graphic Graphic();
	}
}