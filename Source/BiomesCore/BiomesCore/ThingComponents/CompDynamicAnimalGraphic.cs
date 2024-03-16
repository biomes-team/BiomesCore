using System.Collections.Generic;
using Verse;

namespace BiomesCore.ThingComponents
{
	public abstract class CompDynamicAnimalGraphic : ThingComp
	{
		private CompProperties_DynamicAnimalGraphic _props;

		protected Pawn _pawn;

		private int _alternateGraphicIndex;

		public override void Initialize(CompProperties properties)
		{
			base.Initialize(properties);
			_pawn = null;
			_props = (CompProperties_DynamicAnimalGraphic) properties;
			_alternateGraphicIndex = int.MinValue;
		}

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			_pawn = parent as Pawn;
			if (_pawn?.kindDef == null || _props == null)
			{
				Log.Error(
					$"{GetType().Name} must be assigned to a pawn and have properties of type {nameof(CompProperties_DynamicAnimalGraphic)}");
				return;
			}

			if (_pawn.kindDef.alternateGraphics.NullOrEmpty() || _props.alternateGraphics.NullOrEmpty())
			{
				return;
			}

			int pawnAlternateCount = _pawn.kindDef.alternateGraphics.Count;
			int compAlternateCount = _props.alternateGraphics.Count;
			if (compAlternateCount != pawnAlternateCount)
			{
				Log.Error(
					$"{GetType().Name}: Alternate graphics count mismatch between pawn({pawnAlternateCount}) and comp({compAlternateCount}).");
				_alternateGraphicIndex = -1;
				return;
			}

			// Alternate graphics are only applied if both alternate graphic lists match in size.
			// This replicates the logic in PawnGraphicUtils.TryGetAlternate().
			Rand.PushState(_pawn.thingIDNumber ^ 46101);
			if (Rand.Value <= _pawn.kindDef.alternateGraphicChance)
			{
				List<Pair<int, float>> indexesByWeight = new List<Pair<int, float>>();
				for (int alternateIndex = 0; alternateIndex < _pawn.kindDef.alternateGraphics.Count; ++alternateIndex)
				{
					indexesByWeight.Add(new Pair<int, float>(alternateIndex,
						_pawn.kindDef.alternateGraphics[alternateIndex].Weight));
				}

				_alternateGraphicIndex = indexesByWeight.RandomElementByWeight(x => x.Second).First;
			}
		}


		/// <summary>
		/// Determines if the comp should override the normal animal graphic.
		/// </summary>
		/// <returns>True if the comp takes over the animal graphic.</returns>
		public abstract bool Active();

		public Graphic Graphic(Graphic currentGraphic)
		{
			Graphic graphic = _props.graphicData.Graphic;

			// Alternate graphics are only processed if both lists have the same size.
			// Validation of this has already been done at Initialize.
			if (_alternateGraphicIndex >= 0)
			{
				graphic = _props.alternateGraphics[_alternateGraphicIndex].Graphic;
			}

			// Return a copy matching the drawSize and shader of the original.
			return graphic?.GetCopy(currentGraphic.drawSize, currentGraphic.Shader);
		}

		protected void ForceGraphicUpdateNow()
		{
			if (parent is Pawn pawn)
			{
				pawn.Drawer.renderer.SetAllGraphicsDirty();
			}
		}
	}
}