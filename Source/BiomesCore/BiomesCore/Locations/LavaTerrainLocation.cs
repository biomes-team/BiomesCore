using RimWorld;
using Verse;

namespace BiomesCore.Locations
{
	/// <summary>
	/// This location implements the features of LavaTerrainDefExtension.
	/// </summary>
	public class LavaTerrainLocation : TerrainLocation
	{
		/// <summary>
		/// Assumes that LavaTerrainLocation keeps a Rare tickerType.
		/// </summary>
		private const float SecondsToTickFactor = TickerTypeInterval.RareTickerInterval / 60.0F;

		private LavaTerrainDefExtension properties;

		/// <summary>
		/// Dummy CompGlower created to interact with the glow grid. Since the glow grid is not compatible with Locations,
		/// this code is used instead.
		/// </summary>
		private CompGlower instanceGlowerComp;

		/// <summary>
		/// Def used for the fake ThingWithComps used as the parent of the dummy CompGlower.
		/// The only requirement for this def is that it must not have a ThingCategory.Plant.
		/// </summary>
		private static ThingDef _simulatedThingDef = new ThingDef
		{
			category = ThingCategory.None
		};

		protected override void OnInitialize(TerrainLocationDefExtension extension)
		{
			properties = (LavaTerrainDefExtension) extension;


			// The only fields used by the glow grid are CompGlower.Props, CompGlower.parent.Position, and
			// CompGlower.parent.def.category.
			instanceGlowerComp = new CompGlower();
			instanceGlowerComp.parent = new ThingWithComps();
			instanceGlowerComp.parent.SetPositionDirect(Position);
			instanceGlowerComp.parent.def = _simulatedThingDef;

			instanceGlowerComp.Initialize(properties.CompPropertiesGlower);
		}

		public override void TerrainRegistered(LocationGrid grid)
		{
			Map.mapDrawer.MapMeshDirty(Position, MapMeshFlag.Things);
			Map.glowGrid.RegisterGlower(instanceGlowerComp);
			grid.AddThingBurnerAt(Position);
		}

		public override void Tick(int gameTick)
		{
			// This type is ticked every second.
			Room room = Position.GetRoom(Map);
			if (room != null && !room.UsesOutdoorTemperature && room.Temperature >= properties.heatPushMinTemperature &&
			    room.Temperature <= properties.heatPushMaxTemperature)
			{
				Position.GetRoom(Map)?.PushHeat(properties.heatPerSecond * SecondsToTickFactor);
			}
		}

		public override void TerrainRemoved(LocationGrid grid)
		{
			Map.mapDrawer.MapMeshDirty(Position, MapMeshFlag.Things);
			Map.glowGrid.DeRegisterGlower(instanceGlowerComp);
			grid.RemoveThingBurnerAt(Position);
		}
	}
}