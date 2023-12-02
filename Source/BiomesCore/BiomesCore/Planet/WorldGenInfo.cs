using System.Collections.Generic;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace BiomesCore.Planet
{
	/// <summary>
	/// Defines a new field to include in the world generation info, and how to generate it.
	/// 
	/// When creating a new WorldGenInfo, use Gen.HashCombineInt along with WorldGenInfoHandler.WorldSeed to generate new
	/// seeds, and Rand.ChanceSeeded to generate any required random values. This makes Biomes! random world generation
	/// independent from mod load order, execution order and any other factors.
	/// </summary>
	public abstract class WorldGenInfo
	{
		/// <summary>
		/// Caches the generated information value for each world tileID.
		/// </summary>
		private float[] tileData;

		public void Initialize()
		{
			tileData = new float[Find.WorldGrid.TilesCount];
			Setup();
		}

		protected abstract void Setup();

		protected abstract float GenerateTileData(Tile tile, int tileID, Vector3 tileCenter);

		public void GenerateForTile(Tile tile, int tileID, Vector3 tileCenter)
		{
			tileData[tileID] = GenerateTileData(tile, tileID, tileCenter);
		}

		public float GetValue(int tileId)
		{
			return tileData[tileId];
		}
	}
}