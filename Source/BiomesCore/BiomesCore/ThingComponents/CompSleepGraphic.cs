using BiomesCore.Patches.ModThingComps;
using RimWorld;
using Verse;

namespace BiomesCore.ThingComponents
{
	public class CompProperties_CompSleepGraphic : CompProperties_DynamicAnimalGraphic
	{
		/// <summary>
		/// If this is set to true, graphics will only change when the pawn is sleeping under a roof.
		/// </summary>
		public bool roofedOnly;

		/// <summary>
		/// Forces the pawn to face south while sleeping.
		/// </summary>
		public bool faceSouth;

		public CompProperties_CompSleepGraphic() => compClass = typeof(CompSleepGraphic);
	}

	public class CompSleepGraphic : CompDynamicAnimalGraphic
	{
		private CompProperties_CompSleepGraphic Props => (CompProperties_CompSleepGraphic) props;

		[Unsaved] private bool wasAwake;

		private void SetAngleAndFacing(Pawn pawn, bool isAwake)
		{
			if (isAwake)
			{
				PawnRenderer_BodyAngle_Patch.OverrideAngle.Remove(pawn);
				PawnRenderer_LayingFacing_Patch.OverrideDirection.Remove(pawn);
			}
			else
			{
				PawnRenderer_LayingFacing_Patch.OverrideDirection.Add(pawn);
				if (Props.faceSouth)
				{
					PawnRenderer_BodyAngle_Patch.OverrideAngle[pawn] = 0.0F;
				}
			}
		}

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);
			if (!(parent is Pawn pawn))
			{
				return;
			}

			wasAwake = pawn.Awake();
			SetAngleAndFacing(pawn, wasAwake);
		}

		public override void CompTick()
		{
			base.CompTick();
			if (!(parent is Pawn pawn))
			{
				return;
			}

			var isAwake = pawn.Awake();
			if (wasAwake == isAwake)
			{
				return;
			}

			wasAwake = !wasAwake;
			SetAngleAndFacing(pawn, isAwake);
			ForceGraphicUpdateNow();
		}

		public override void PostDeSpawn(Map map)
		{
			base.PostDeSpawn(map);
			if (parent is Pawn pawn)
			{
				PawnRenderer_BodyAngle_Patch.OverrideAngle.Remove(pawn);
				PawnRenderer_LayingFacing_Patch.OverrideDirection.Remove(pawn);
			}
		}

		public override bool Active()
		{
			return parent is Pawn pawn && !pawn.Dead && !pawn.Awake() &&
			       (!Props.roofedOnly || pawn.Position.Roofed(pawn.Map));
		}
	}
}