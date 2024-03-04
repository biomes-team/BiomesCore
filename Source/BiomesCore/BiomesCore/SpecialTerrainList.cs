using System.Collections.Generic;
using Verse;

namespace BiomesCore
{
	public class TerrainInstance : IExposable
	{
		private string def;
		private string map;
		private string pos;

		public TerrainInstance()
		{
		}

		/// <summary>
		/// Saving/loading
		/// </summary>
		public virtual void ExposeData()
		{
			if (Scribe.mode != LoadSaveMode.LoadingVars)
			{
				// This data can be loaded, but it will never be saved.
				return;
			}

			Scribe_Values.Look(ref map, "map");
			Scribe_Values.Look(ref pos, "pos");
			Scribe_Values.Look(ref def, "def");
		}
	}

	/// <summary>
	/// Dummy map component. Keeps save-game compatibility with 1.4 saves with the old active terrain system.
	/// </summary>
	public class SpecialTerrainList : MapComponent
	{
		public SpecialTerrainList(Map map) : base(map)
		{
		}

		public override void ExposeData()
		{
			base.ExposeData();
			if (Scribe.mode != LoadSaveMode.LoadingVars)
			{
				// This map component can be loaded, but it will never be saved.
				return;
			}

			var terrains = new Dictionary<IntVec3, TerrainInstance>();
			Scribe_Collections.Look(ref terrains, "terrains", LookMode.Value, LookMode.Deep);
		}
	}
}