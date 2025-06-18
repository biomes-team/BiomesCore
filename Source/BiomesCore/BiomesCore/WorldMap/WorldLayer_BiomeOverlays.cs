using System.Collections;
using BiomesCore.DefModExtensions;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace BiomesCore.WorldMap
{
	public class WorldLayer_BiomeOverlays : WorldDrawLayer
	{
		public override IEnumerable Regenerate()
		{
			foreach (object obj in base.Regenerate())
			{
				yield return obj;
			}

			WorldGrid worldGrid = Find.WorldGrid;
			for (int tileID = 0; tileID < Find.WorldGrid.TilesCount; ++tileID)
			{
				Tile tile = Find.WorldGrid[tileID];
				BiomesMap extension = tile.PrimaryBiome.GetModExtension<BiomesMap>();
				if (extension?.overlayTexturePath == null)
				{
					continue;
				}

				// Right before hill materials.
				const int renderQueue = 3509;
				Material material = MaterialPool.MatFrom(extension.overlayTexturePath,
					ShaderDatabase.WorldOverlayTransparentLit, renderQueue);
				LayerSubMesh subMesh = GetSubMesh(material);
				Vector3 pos = worldGrid.GetTileCenter(tileID);
				WorldRendererUtility.PrintQuadTangentialToPlanet(pos, pos, worldGrid.AverageTileSize, 0.005f, subMesh, printUVs: false);
				WorldRendererUtility.PrintTextureAtlasUVs(Rand.Range(0, 2), Rand.Range(0, 2), 2, 2, subMesh);
			}

			FinalizeMesh(MeshParts.All);
		}
	}
}