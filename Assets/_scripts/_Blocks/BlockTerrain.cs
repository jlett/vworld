using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class BlockTerrain : Block {
	public BlockTerrain() : base() {

	}

	public override MeshData BlockData(Chunk chunk, int x, int y, int z, MeshData meshData) {
		//split the cube into 8 sub-cubes, one cooresponding with each vertex
		//if the vertex is touching more than one not terrain block, cooresponding cube is split to a triangle
		//alternatively the cube could be removed altogether, whichever looks better
		//
		//note that these checks are with isTerrain, but isSolid used to check if each outside face rendered
		
		bool northSolid = chunk.GetBlock(x, y, z+1).IsSolid(Direction.south); 
		bool southSolid = chunk.GetBlock(x, y, z-1).IsSolid(Direction.north);
		bool eastSolid = chunk.GetBlock(x+1, y, z).IsSolid(Direction.west);
		bool westSolid = chunk.GetBlock(x-1, y, z).IsSolid(Direction.east);
		bool upSolid = chunk.GetBlock(x, y+1, z).IsSolid(Direction.down);
		bool downSolid = chunk.GetBlock(x, y-1, z).IsSolid(Direction.up);
		
		bool northTer = chunk.GetBlock(x, y, z+1).isTerrain; 
		bool southTer = chunk.GetBlock(x, y, z-1).isTerrain;
		bool eastTer = chunk.GetBlock(x+1, y, z).isTerrain;
		bool westTer = chunk.GetBlock(x-1, y, z).isTerrain;
		bool upTer = chunk.GetBlock(x, y+1, z).isTerrain;
		bool downTer = chunk.GetBlock(x, y-1, z).isTerrain;


		return meshData;
	}
}
