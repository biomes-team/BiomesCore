using System;
using System.Collections.Generic;
using Mono.CompilerServices.SymbolWriter;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace BiomesCore.Planet
{
	/// <summary>
	/// Exposes vanilla noise modules for general usage.
	/// Manages the life cycle of additional world generation information stored in WorldGenInfo instances.
	/// </summary>
	public static class WorldGenInfoHandler
	{
		private static Dictionary<Type, WorldGenInfo> _infos = new Dictionary<Type, WorldGenInfo>();

		/// <summary>
		/// Vanilla noise elevation.
		/// </summary>
		public static ModuleBase NoiseElevation;

		/// <summary>
		/// Vanilla world seed. Use Gen.HashCombineInt along with this value to generate new seeds, to ensure that random
		/// number generation is the same regardless of the presence of other mods.
		/// </summary>
		public static int WorldSeed;

		/// <summary>
		/// Register WorldGenInfo types into the handler. This should be done during game initialization by the mods that
		/// require the WorldGenInfo. If multiple mods require the same info, all of them should register it.
		/// </summary>
		/// <typeparam name="TWorldGenInfo">Type to be registered.</typeparam>
		public static void Register<TWorldGenInfo>() where TWorldGenInfo : WorldGenInfo, new()
		{
			_infos[typeof(TWorldGenInfo)] = new TWorldGenInfo();
		}

		/// <summary>
		/// Get the instance of a WorldGenInfo type.
		/// </summary>
		/// <typeparam name="TWorldGenInfo">Type to be retrieved.</typeparam>
		/// <returns>Instance.</returns>
		public static TWorldGenInfo Get<TWorldGenInfo>() where TWorldGenInfo : WorldGenInfo
		{
			Type type = typeof(TWorldGenInfo);
			return _infos.ContainsKey(type) ? (TWorldGenInfo) _infos[typeof(TWorldGenInfo)] : null;
		}

		/// <summary>
		/// Setup all additional world generation information prior to world generation taking place.
		/// </summary>
		/// <param name="noiseElevation">Elevation noise module from vanilla.</param>
		public static void Setup(ModuleBase noiseElevation)
		{
			NoiseElevation = noiseElevation;
			WorldSeed = Find.World.info.Seed;
			foreach (WorldGenInfo info in _infos.Values)
			{
				info.Initialize();
			}
		}

		/// <summary>
		/// Generate world generation information data for each tile in the grid.
		/// </summary>
		public static void GenerateTileFor(Tile tile, int tileID)
		{
			Vector3 tileCenter = Find.WorldGrid.GetTileCenter(tileID);
			foreach (WorldGenInfo info in _infos.Values)
			{
				info.GenerateForTile(tile, tileID, tileCenter);
			}
		}
	}
}