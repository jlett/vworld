using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class BlockTerrain : Block {
	public BlockTerrain() : base() {
		isTerrain = true;
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

		//for each vertex in order that they are listed in Block.AddFaceTri/Quad

		/* checklist for each vert:
		 *  if there are less than 2 nearby terrain blocks:
		 * 		use the cutout triangle
		 * 		set block not solid on those three sides //TODO
		 * 	else
		 * 		build the three external corner triangles, one for each side that is not solid
		 * 
		 * after verts...
		 * if face direction is solid
		 * 		build the center diamond quad
		 */

		if(!northSolid)
			meshData = AddFaceQuad(chunk, x, y, z, new int[4] {10, 11, 8, 9}, Direction.north, meshData);
		if(!southSolid)
			meshData = AddFaceQuad(chunk, x, y, z, new int[4] {14, 15, 12, 13}, Direction.south, meshData);
		if(!eastSolid)
			meshData = AddFaceQuad(chunk, x, y, z, new int[4] {18, 13, 17, 11}, Direction.east, meshData);
		if(!westSolid)
			meshData = AddFaceQuad(chunk, x, y, z, new int[4] {19, 9, 16, 15}, Direction.west, meshData);
		if(!upSolid)
			meshData = AddFaceQuad(chunk, x, y, z, new int[4] {12, 16, 8, 17}, Direction.up, meshData);
		if(!downSolid)
			meshData = AddFaceQuad(chunk, x, y, z, new int[4] {10, 19, 14, 18}, Direction.down, meshData);

		if(Convert.ToInt32(northTer) + Convert.ToInt32(eastTer) + Convert.ToInt32(upTer) < 2) {
			meshData = AddFaceTri(chunk, x, y, z, new int[3] {17, 8, 11}, Direction.up, meshData);
		} else {
			if(!northSolid)
				meshData = AddFaceTri(chunk, x, y, z, new int[3] {0, 8, 11}, Direction.up, meshData);
			if(!eastSolid)
				meshData = AddFaceTri(chunk, x, y, z, new int[3] {0, 11, 17}, Direction.up, meshData);
			if(!upSolid)
				meshData = AddFaceTri(chunk, x, y, z, new int[3] {0, 17, 8}, Direction.up, meshData);
		}
		if(Convert.ToInt32(northTer) + Convert.ToInt32(westTer) + Convert.ToInt32(upTer) < 2) {
			meshData = AddFaceTri(chunk, x, y, z, new int[3] {8, 16, 9}, Direction.up, meshData);
		} else {
			if(!northSolid)
				meshData = AddFaceTri(chunk, x, y, z, new int[3] {1, 9, 8}, Direction.up, meshData);
			if(!westSolid)
				meshData = AddFaceTri(chunk, x, y, z, new int[3] {1, 16, 9}, Direction.up, meshData);
			if(!upSolid)
				meshData = AddFaceTri(chunk, x, y, z, new int[3] {1, 8, 16}, Direction.up, meshData);
		}
		if(Convert.ToInt32(northTer) + Convert.ToInt32(westTer) + Convert.ToInt32(downTer) < 2) {
			meshData = AddFaceTri(chunk, x, y, z, new int[3] {10, 9, 19}, Direction.down, meshData);
		} else {
			if(!northSolid)
				meshData = AddFaceTri(chunk, x, y, z, new int[3] {2, 10, 9}, Direction.up, meshData);
			if(!westSolid)
				meshData = AddFaceTri(chunk, x, y, z, new int[3] {2, 9, 19}, Direction.up, meshData);
			if(!downSolid)
				meshData = AddFaceTri(chunk, x, y, z, new int[3] {2, 19, 10}, Direction.up, meshData);
		}
		if(Convert.ToInt32(northTer) + Convert.ToInt32(eastTer) + Convert.ToInt32(downTer) < 2) {
			meshData = AddFaceTri(chunk, x, y, z, new int[3] {11, 10, 18}, Direction.down, meshData);
		} else {
			if(!northSolid)
				meshData = AddFaceTri(chunk, x, y, z, new int[3] {3, 11, 10}, Direction.up, meshData);
			if(!eastSolid)
				meshData = AddFaceTri(chunk, x, y, z, new int[3] {3, 18, 11}, Direction.up, meshData);
			if(!downSolid)
				meshData = AddFaceTri(chunk, x, y, z, new int[3] {3, 10, 18}, Direction.up, meshData);
		}
		if(Convert.ToInt32(southTer) + Convert.ToInt32(westTer) + Convert.ToInt32(upTer) < 2) {
			meshData = AddFaceTri(chunk, x, y, z, new int[3] {16, 12, 15}, Direction.up, meshData);
		} else {
			if(!southSolid)
				meshData = AddFaceTri(chunk, x, y, z, new int[3] {4, 12, 15}, Direction.up, meshData);
			if(!westSolid)
				meshData = AddFaceTri(chunk, x, y, z, new int[3] {4, 15, 16}, Direction.up, meshData);
			if(!upSolid)
				meshData = AddFaceTri(chunk, x, y, z, new int[3] {4, 16, 12}, Direction.up, meshData);
		}
		if(Convert.ToInt32(southTer) + Convert.ToInt32(eastTer) + Convert.ToInt32(upTer) < 2) {
			meshData = AddFaceTri(chunk, x, y, z, new int[3] {12, 17, 13}, Direction.up, meshData);
		} else {
			if(!southSolid)
				meshData = AddFaceTri(chunk, x, y, z, new int[3] {5, 13, 12}, Direction.up, meshData);
			if(!eastSolid)
				meshData = AddFaceTri(chunk, x, y, z, new int[3] {5, 17, 13}, Direction.up, meshData);
			if(!upSolid)
				meshData = AddFaceTri(chunk, x, y, z, new int[3] {5, 12, 17}, Direction.up, meshData);
		}
		if(Convert.ToInt32(southTer) + Convert.ToInt32(eastTer) + Convert.ToInt32(downTer) < 2) {
			meshData = AddFaceTri(chunk, x, y, z, new int[3] {13, 18, 14}, Direction.down, meshData);
		} else {
			if(!southSolid)
				meshData = AddFaceTri(chunk, x, y, z, new int[3] {6, 14, 13}, Direction.up, meshData);
			if(!eastSolid)
				meshData = AddFaceTri(chunk, x, y, z, new int[3] {6, 13, 18}, Direction.up, meshData);
			if(!downSolid)
				meshData = AddFaceTri(chunk, x, y, z, new int[3] {6, 18, 14}, Direction.up, meshData);
		}
		if(Convert.ToInt32(southTer) + Convert.ToInt32(westTer) + Convert.ToInt32(downTer) < 2) {
			meshData = AddFaceTri(chunk, x, y, z, new int[3] {15, 14, 19}, Direction.down, meshData);
		} else {
			if(!southSolid)
				meshData = AddFaceTri(chunk, x, y, z, new int[3] {7, 15, 14}, Direction.up, meshData);
			if(!westSolid)
				meshData = AddFaceTri(chunk, x, y, z, new int[3] {7, 19, 15}, Direction.up, meshData);
			if(!downSolid)
				meshData = AddFaceTri(chunk, x, y, z, new int[3] {7, 14, 19}, Direction.up, meshData);
		}

		//for each face in order NSEWUD


		return meshData;
	}
}
