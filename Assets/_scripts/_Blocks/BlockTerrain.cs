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
		bool northSolid = chunk.GetBlock(x, y, z+1).IsSolid(Direction.south); 
		bool southSolid = chunk.GetBlock(x, y, z-1).IsSolid(Direction.north);
		bool eastSolid = chunk.GetBlock(x+1, y, z).IsSolid(Direction.west);
		bool westSolid = chunk.GetBlock(x-1, y, z).IsSolid(Direction.east);
		bool upSolid = chunk.GetBlock(x, y+1, z).IsSolid(Direction.down);
		bool downSolid = chunk.GetBlock(x, y-1, z).IsSolid(Direction.up);

		int cardnalsSolid = 0;
		if(eastSolid)
			cardnalsSolid++;
		if(westSolid)
			cardnalsSolid++;
		if(northSolid)
			cardnalsSolid++;
		if(southSolid)
			cardnalsSolid++;
		
		if(!upSolid && downSolid) {
			if(cardnalsSolid == 3) { 																				//case 1 (in notes)
//				meshData = FaceDataDown(chunk, x, y, z, meshData);
				if(!southSolid) {
//					meshData = FaceDataNorth(chunk, x, y, z, meshData);
//					meshData = AddFaceTri(chunk, x, y, z, new int[3] {1, 7, 2}, Direction.west, meshData);
//					meshData = AddFaceTri(chunk, x, y, z, new int[3] {0, 3, 6}, Direction.east, meshData);
					meshData = AddFaceQuad(chunk, x, y, z, new int[4] {1, 0, 6, 7}, Direction.up, meshData);
				} else if(!northSolid) {
//					meshData = FaceDataSouth(chunk, x, y, z, meshData);
//					meshData = AddFaceTri(chunk, x, y, z, new int[3] {4, 7, 2}, Direction.west, meshData);
//					meshData = AddFaceTri(chunk, x, y, z, new int[3] {5, 6, 3}, Direction.east, meshData);
					meshData = AddFaceQuad(chunk, x, y, z, new int[4] {5, 4, 2, 3}, Direction.up, meshData);
				} else if(!eastSolid) {
//					meshData = FaceDataWest(chunk, x, y, z, meshData);
//					meshData = AddFaceTri(chunk, x, y, z, new int[3] {4, 6, 7}, Direction.south, meshData);
//					meshData = AddFaceTri(chunk, x, y, z, new int[3] {1, 2, 3}, Direction.north, meshData);
					meshData = AddFaceQuad(chunk, x, y, z, new int[4] {1, 3, 6, 4}, Direction.up, meshData);
				} else if(!westSolid) {
//					meshData = FaceDataEast(chunk, x, y, z, meshData);
//					meshData = AddFaceTri(chunk, x, y, z, new int[3] {7, 5, 6}, Direction.south, meshData);
//					meshData = AddFaceTri(chunk, x, y, z, new int[3] {0, 2, 3}, Direction.north, meshData);
					meshData = AddFaceQuad(chunk, x, y, z, new int[4] {0, 5, 7, 2}, Direction.up, meshData);
				}
			} else if(cardnalsSolid == 2) {
				if((northSolid || southSolid) && (eastSolid || westSolid)) {										//case 2
					meshData = base.BlockData(chunk, x, y, z, meshData);
				}
			} else { //backup plan!
				meshData = base.BlockData(chunk, x, y, z, meshData);
			}
		} else { //backup plan!
			meshData = base.BlockData(chunk, x, y, z, meshData);
		}
		return meshData;
	}
}
