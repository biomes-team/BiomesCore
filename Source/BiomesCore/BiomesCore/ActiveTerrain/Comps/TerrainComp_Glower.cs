using RimWorld;
using System;
using Verse;

namespace BiomesCore
{
	public class TerrainCompProperties_Glower : TerrainCompProperties
	{
		public TerrainCompProperties_Glower()
		{
			compClass = typeof(TerrainComp_Glower);
		}

		public float overlightRadius;

		public float glowRadius = 14f;

		public ColorInt glowColor = new ColorInt(255, 255, 255, 0) * 1.45f;
	}


	/// <summary>
	/// Glower except for terrain.
	/// </summary>
	public class TerrainComp_Glower : TerrainComp
	{
		//Unsaved fields
		[Unsaved] protected bool currentlyOn;

		/// <summary>
		/// Kept for later use
		/// </summary>
		[Unsaved] CompGlower instanceGlowerComp;

		public CompGlower AsThingComp
		{
			get { return (instanceGlowerComp == null) ? instanceGlowerComp = (CompGlower) this : instanceGlowerComp; }
		}

		public TerrainCompProperties_Glower Props
		{
			get { return (TerrainCompProperties_Glower) props; }
		}

		// renamed from ShouldBeLitNow to avoid LightsOut messing with it
		public virtual bool ShouldTerrainBeLitNow => true;

		//Main fields and their properties
		ColorInt colorInt;
		float glowRadius;
		float overlightRadius;

		public float OverlightRadius
		{
			get => overlightRadius;
			set => overlightRadius = value;
		}

		public float GlowRadius
		{
			get => glowRadius;
			set => glowRadius = value;
		}

		public ColorInt Color
		{
			get => colorInt;
			set => colorInt = value;
		}

		public void UpdateLit()
		{
			bool shouldBeLitNow = ShouldTerrainBeLitNow;
			if (currentlyOn == shouldBeLitNow)
			{
				return;
			}

			currentlyOn = shouldBeLitNow;
			parent.Map.mapDrawer.MapMeshDirty(parent.Position, MapMeshFlag.Things);
			//Ternary logic statement.
			(currentlyOn ? (Action<CompGlower>) parent.Map.glowGrid.RegisterGlower : parent.Map.glowGrid.DeRegisterGlower)(
				AsThingComp);
		}

		public override void PostPostLoad()
		{
			UpdateLit();
			if (ShouldTerrainBeLitNow)
			{
				parent.Map.glowGrid.RegisterGlower(AsThingComp);
			}
		}

		public override void PostRemove()
		{
			base.PostRemove();
			this.parent.Map.glowGrid.DeRegisterGlower(this.AsThingComp);
		}

		public override void Initialize(TerrainCompProperties props)
		{
			base.Initialize(props);
			Color = Props.glowColor;
			GlowRadius = Props.glowRadius;
			OverlightRadius = Props.overlightRadius;
			if (ShouldTerrainBeLitNow && parent.Map != null)
			{
				parent.Map.glowGrid.RegisterGlower(AsThingComp);
			}
		}

		/// <summary>
		/// Hacked-together method to convert terrain comp into thing comp for map glower component. 
		/// Note: Glower comp only works as a dummy, and many pieces of information are missing which may cause quirks/errors outside of intended use
		/// Note2: TerrainComp_Glower has its own property for storing a CompGlower. Use that as that way the object reference would be kept the same.
		/// </summary>
		public static explicit operator CompGlower(TerrainComp_Glower inst)
		{
			var glower = new CompGlower() //Create instance
			{
				parent = (ThingWithComps) ThingMaker.MakeThing(ThingDefOf.Wall,
					ThingDefOf.Steel) //The most generic ThingWithComps there is
			};
			glower.parent.SetPositionDirect(inst.parent.Position); //Set position
			glower.Initialize(new CompProperties_Glower() //Copy props
			{
				glowColor = inst.Color,
				glowRadius = inst.GlowRadius,
				overlightRadius = inst.OverlightRadius
			});
			return glower;
		}
	}
}