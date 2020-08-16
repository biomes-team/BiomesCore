using RimWorld;
using Verse;
using UnityEngine;
using System.Collections.Generic;
using BiomesCore.DefModExtensions;

namespace BiomesCore.Bridges
{
	static class BridgeExtensions
    {
		// Use cache to avoid repeated material file loads on rendering
		public static Dictionary<TerrainDef, Material> BridgeLoopMats = new Dictionary<TerrainDef, Material>();
		public static Dictionary<TerrainDef, Material> BridgeRightMats = new Dictionary<TerrainDef, Material>();

		public static Material BiomesBridgeLoopMat(this TerrainDef terrain)
        {
			if (terrain.HasModExtension<TerrainDef_Bridge>())
			{
				if (!BridgeLoopMats.TryGetValue(terrain, out Material mat))
				{
					string loopTextPath = terrain.GetModExtension<TerrainDef_Bridge>().loopTexPath;
					if (loopTextPath != null)
					{
						mat = MaterialPool.MatFrom(loopTextPath, ShaderDatabase.Transparent);
						BridgeLoopMats.Add(terrain, mat);
					}
					else
					{
						mat = null;
					}
				}
				return mat;
			}
            return null;
		}
		public static Material BiomesBridgeRightMat(this TerrainDef terrain)
		{
			if (terrain.HasModExtension<TerrainDef_Bridge>())
			{
				if (!BridgeRightMats.TryGetValue(terrain, out Material mat))
				{
					string rightTexPath = terrain.GetModExtension<TerrainDef_Bridge>().rightTexPath;
					if (rightTexPath != null)
					{
						mat = MaterialPool.MatFrom(rightTexPath, ShaderDatabase.Transparent);
						BridgeRightMats.Add(terrain, mat);
					}
					else
					{
						mat = null;
					}
				}
				return mat;
			}
			return null;
		}

		public static bool IsBiomesBridge(this TerrainDef terrain)
        {
            return terrain.HasModExtension<TerrainDef_Bridge>();
        }
    }


    [StaticConstructorOnStartup]
    public class SectionLayer_BridgeProps : SectionLayer
    {
		public override bool Visible => DebugViewSettings.drawTerrain;

		public SectionLayer_BridgeProps(Section section)
			: base(section)
		{
			relevantChangeTypes = MapMeshFlag.Terrain;
		}

		public override void Regenerate()
		{
			ClearSubMeshes(MeshParts.All);
			Map map = base.Map;
			TerrainGrid terrainGrid = map.terrainGrid;
			CellRect cellRect = section.CellRect;
			float y = AltitudeLayer.TerrainScatter.AltitudeFor();
			foreach (IntVec3 item in cellRect)
			{
				if (ShouldDrawPropsBelow(item, terrainGrid))
				{
					IntVec3 c = item;
					TerrainDef terrainDef = terrainGrid.TerrainAt(c);
					c.x++;
					Material material = (!c.InBounds(map) || !ShouldDrawPropsBelow(c, terrainGrid)) ? terrainDef.BiomesBridgeRightMat() : terrainDef.BiomesBridgeLoopMat();
					LayerSubMesh subMesh = GetSubMesh(material);
					int count = subMesh.verts.Count;
					subMesh.verts.Add(new Vector3(item.x, y, item.z - 1));
					subMesh.verts.Add(new Vector3(item.x, y, item.z));
					subMesh.verts.Add(new Vector3(item.x + 1, y, item.z));
					subMesh.verts.Add(new Vector3(item.x + 1, y, item.z - 1));
					subMesh.uvs.Add(new Vector2(0f, 0f));
					subMesh.uvs.Add(new Vector2(0f, 1f));
					subMesh.uvs.Add(new Vector2(1f, 1f));
					subMesh.uvs.Add(new Vector2(1f, 0f));
					subMesh.tris.Add(count);
					subMesh.tris.Add(count + 1);
					subMesh.tris.Add(count + 2);
					subMesh.tris.Add(count);
					subMesh.tris.Add(count + 2);
					subMesh.tris.Add(count + 3);
				}
			}
			FinalizeMesh(MeshParts.All);
		}

		public bool ShouldDrawPropsBelow(IntVec3 c, TerrainGrid terrGrid)
		{
			TerrainDef terrainDef = terrGrid.TerrainAt(c);
			if (terrainDef == null || !terrainDef.IsBiomesBridge() || terrainDef.BiomesBridgeLoopMat() == null || terrainDef.BiomesBridgeRightMat() == null)
			{
				return false;
			}
			IntVec3 c2 = c;
			c2.z--;
			Map map = base.Map;
			if (!c2.InBounds(map))
			{
				return false;
			}
			TerrainDef terrainDef2 = terrGrid.TerrainAt(c2);
			if (terrainDef2.IsBiomesBridge())
			{
				return false;
			}
			if (terrainDef2.passability != Traversability.Impassable && !c2.SupportsStructureType(map, terrainDef.terrainAffordanceNeeded))
			{
				return false;
			}
			return true;
		}
	}
}
