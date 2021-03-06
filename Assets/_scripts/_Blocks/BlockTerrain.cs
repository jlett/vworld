using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class BlockTerrain : Block {
	public BlockTerrain() : base() {
		isTerrain = true;
	}

	public override MeshData BlockData(Chunk chunk, int x, int y, int z, MeshData meshData) {
		meshData.useRenderDataForCollision = true;

		Vector3[] verts = new Vector3[8];

		bool upSolid = chunk.GetBlock(x, y+1, z).IsSolid(Direction.down); 
		bool downSolid = chunk.GetBlock(x, y-1, z).IsSolid(Direction.up);

		bool northTer = chunk.GetBlock(x, y, z+1).isTerrain; 
		bool southTer = chunk.GetBlock(x, y, z-1).isTerrain;
		bool eastTer = chunk.GetBlock(x+1, y, z).isTerrain;
		bool westTer = chunk.GetBlock(x-1, y, z).isTerrain;
		bool upTer = chunk.GetBlock(x, y+1, z).isTerrain;
		bool downTer = chunk.GetBlock(x, y-1, z).isTerrain;

		bool neTer = chunk.GetBlock(x+1, y, z+1).isTerrain;
		bool nwTer = chunk.GetBlock(x-1, y, z+1).isTerrain;
		bool seTer = chunk.GetBlock(x+1, y, z-1).isTerrain;
		bool swTer = chunk.GetBlock(x-1, y, z-1).isTerrain;

		//for each vertex:
		//if up/down terrain - keep at corner
		//else if both left and right terrain - keep at corner
		//else if one of left and right terrain - move up/down .5
		//else if none terrain - move up/down .5

		//vert 0-3
		if(upTer || upSolid) {
			verts[0] = new Vector3(0.5f, 0.5f, 0.5f);
			verts[1] = new Vector3(-0.5f, 0.5f, 0.5f);
			verts[4] = new Vector3(-0.5f, 0.5f, -0.5f);
			verts[5] = new Vector3(0.5f, 0.5f, -0.5f);
		} else {
			if(northTer && eastTer && neTer)
				verts[0] = new Vector3(0.5f, 0.5f, 0.5f);
			else
				verts[0] = new Vector3(0.5f, 0.1f, 0.5f);

			if(southTer && eastTer && seTer)
				verts[5] = new Vector3(0.5f, 0.5f, -0.5f);
			else
				verts[5] = new Vector3(0.5f, 0.1f, -0.5f);

			if(northTer && westTer && nwTer)
				verts[1] = new Vector3(-0.5f, 0.5f, 0.5f);
			else
				verts[1] = new Vector3(-0.5f, 0.1f, 0.5f);

			if(southTer && westTer && swTer)
				verts[4] = new Vector3(-0.5f, 0.5f, -0.5f);
			else
				verts[4] = new Vector3(-0.5f, 0.1f, -0.5f);
		}

		if(downTer || downSolid) {
			verts[3] = new Vector3(0.5f, -0.5f, 0.5f);
			verts[2] = new Vector3(-0.5f, -0.5f, 0.5f);
			verts[7] = new Vector3(-0.5f, -0.5f, -0.5f);
			verts[6] = new Vector3(0.5f, -0.5f, -0.5f);
		}  else {
			if(northTer && eastTer && neTer)
				verts[3] = new Vector3(0.5f, -0.5f, 0.5f);
			else
				verts[3] = new Vector3(0.5f, -0.1f, 0.5f);
			
			if(southTer && eastTer && seTer)
				verts[6] = new Vector3(0.5f, -0.5f, -0.5f);
			else
				verts[6] = new Vector3(0.5f, -0.1f, -0.5f);
			
			if(northTer && westTer && nwTer)
				verts[2] = new Vector3(-0.5f, -0.5f, 0.5f);
			else
				verts[2] = new Vector3(-0.5f, -0.1f, 0.5f);
			
			if(southTer && westTer && swTer)
				verts[7] = new Vector3(-0.5f, -0.5f, -0.5f);
			else
				verts[7] = new Vector3(-0.5f, -0.1f, -0.5f);
		}

		meshData = AddCubeWithVerts(chunk, x, y, z, verts, meshData);

		return meshData;
	}

	public override bool IsSolid (Direction direction) {
		return true;
	}
}
