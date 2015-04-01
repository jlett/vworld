using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class Block {

	public enum Direction {north, south, east, west, up, down};
	public struct Tile { public int x; public int y;}
	const float tileSize = 0.25f;//1 divided by number of tiles per side (aka 0.25 on a 4x4 texture)

	public bool changed = true;

	//base constructor
	public Block() {

	}

	public virtual MeshData BlockData(Chunk chunk, int x, int y, int z, MeshData meshData) {
		meshData.useRenderDataForCollision = true;

		meshData = AddVertices(chunk, x, y, z, meshData);
		meshData = AddUVs(chunk, x, y, z, meshData);

		if(!chunk.GetBlock(x, y, z+1).IsSolid(Direction.south)) {
			meshData = FaceDataNorth(chunk, x, y, z, meshData);
		}
		if(!chunk.GetBlock(x, y, z-1).IsSolid(Direction.north)) {
			meshData = FaceDataSouth(chunk, x, y, z, meshData);
		}
		if(!chunk.GetBlock(x+1, y, z).IsSolid(Direction.west)) {
			meshData = FaceDataEast(chunk, x, y, z, meshData);
		}
		if(!chunk.GetBlock(x-1, y, z).IsSolid(Direction.east)) {
			meshData = FaceDataWest(chunk, x, y, z, meshData);
		}
		if(!chunk.GetBlock(x, y+1, z).IsSolid(Direction.down)) {
			meshData = FaceDataUp(chunk, x, y, z, meshData);
		}
		if(!chunk.GetBlock(x, y-1, z).IsSolid(Direction.up)) {
			meshData = FaceDataDown(chunk, x, y, z, meshData);
		}
		return meshData;
	}

	//add all the vertices, triangles will be stiched together in the following 6 functions
	//can be overridden by a child for non-cube voxels
	protected virtual MeshData AddVertices(Chunk chunk, int x, int y, int z, MeshData meshData) {
		meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
		meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
		meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));
		meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));

		meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
		meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
		meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
		meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
		
		return meshData;
	}

	protected virtual MeshData FaceDataNorth(Chunk chunk, int x, int y, int z, MeshData meshData) {
		meshData.AddQuadTriangles(7, 6, 5, 4);
		return meshData;
	}

	protected virtual MeshData FaceDataSouth(Chunk chunk, int x, int y, int z, MeshData meshData) {
		meshData.AddQuadTriangles(3, 2, 1, 0);
		return meshData;
	}

	protected virtual MeshData FaceDataEast(Chunk chunk, int x, int y, int z, MeshData meshData) {
		meshData.AddQuadTriangles(2, 7, 4, 1);
		return meshData;
	}

	protected virtual MeshData FaceDataWest(Chunk chunk, int x, int y, int z, MeshData meshData) {
		meshData.AddQuadTriangles(3, 0, 5, 6);
		return meshData;
	}

	protected virtual MeshData FaceDataUp(Chunk chunk, int x, int y, int z, MeshData meshData) {
		meshData.AddQuadTriangles(6, 7, 2, 3);
		return meshData;
	}
	
	protected virtual MeshData FaceDataDown(Chunk chunk, int x, int y, int z, MeshData meshData) {
		meshData.AddQuadTriangles(0, 1, 4, 5);
		return meshData;
	}

	//to be overridden by special case blocks
	public virtual bool IsSolid(Direction direction) {
		return true;//by default block is solid on all sides
	}

	//defines lower left point on textue
	//to be overridden by textured blocks (aka like everything)
	public virtual Tile GetTexturePosition() {
		Tile tile = new Tile();
		tile.x = 0;
		tile.y = 0;

		return tile;
	}

	public virtual MeshData AddUVs(Chunk chunk, int x, int y, int z, MeshData meshData) {
		Vector2[] UVs = new Vector2[8];
		Tile tilePos = GetTexturePosition();

		UVs[0] = new Vector2(tileSize*tilePos.x + tileSize, tileSize*tilePos.y);
		UVs[1] = new Vector2(tileSize*tilePos.x + tileSize, tileSize*tilePos.y + tileSize);
		UVs[2] = new Vector2(tileSize*tilePos.x, tileSize*tilePos.y + tileSize);
		UVs[3] = new Vector2(tileSize*tilePos.x, tileSize*tilePos.y);

		UVs[4] = new Vector2(tileSize*tilePos.x + tileSize, tileSize*tilePos.y);
		UVs[5] = new Vector2(tileSize*tilePos.x + tileSize, tileSize*tilePos.y + tileSize);
		UVs[6] = new Vector2(tileSize*tilePos.x, tileSize*tilePos.y + tileSize);
		UVs[7] = new Vector2(tileSize*tilePos.x, tileSize*tilePos.y);

		meshData.uv.AddRange(UVs);
		return meshData;
	}
}
