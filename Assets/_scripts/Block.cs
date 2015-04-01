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

	protected virtual MeshData FaceDataNorth(Chunk chunk, int x, int y, int z, MeshData meshData) {
		meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
		meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
		meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
		meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));
		
		meshData.AddQuadTriangles();
		meshData.uv.AddRange(GetFaceUVs(Direction.north));

		return meshData;
	}

	protected virtual MeshData FaceDataSouth(Chunk chunk, int x, int y, int z, MeshData meshData) {
		meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
		meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
		meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
		meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
		
		meshData.AddQuadTriangles();
		meshData.uv.AddRange(GetFaceUVs(Direction.south));
		return meshData;
	}

	protected virtual MeshData FaceDataEast(Chunk chunk, int x, int y, int z, MeshData meshData) {
		meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
		meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
		meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
		meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
		
		meshData.AddQuadTriangles();
		meshData.uv.AddRange(GetFaceUVs(Direction.east));
		return meshData;
	}

	protected virtual MeshData FaceDataWest(Chunk chunk, int x, int y, int z, MeshData meshData) {
		meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));
		meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
		meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
		meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
		
		meshData.AddQuadTriangles();
		meshData.uv.AddRange(GetFaceUVs(Direction.west));
		return meshData;
	}

	protected virtual MeshData FaceDataUp(Chunk chunk, int x, int y, int z, MeshData meshData) {
		meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
		meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
		meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
		meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));

		meshData.AddQuadTriangles();
		meshData.uv.AddRange(GetFaceUVs(Direction.up));
		return meshData;
	}
	
	protected virtual MeshData FaceDataDown(Chunk chunk, int x, int y, int z, MeshData meshData) {
		meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
		meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
		meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
		meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));
		
		meshData.AddQuadTriangles();
		meshData.uv.AddRange(GetFaceUVs(Direction.down));
		return meshData;
	}

	//to be overridden by special case blocks
	public virtual bool IsSolid(Direction direction) {
		return true;//by default block is solid on all sides
	}

	//to be overridden by textured blocks (aka like everything)
	public virtual Tile GetTexturePosition(Direction direction) {
		Tile tile = new Tile();
		tile.x = 0;
		tile.y = 0;

		return tile;
	}

	public virtual Vector2[] GetFaceUVs(Direction direction) {
		Vector2[] UVs = new Vector2[4];
		Tile tilePos = GetTexturePosition(direction);

		UVs[0] = new Vector2(tileSize*tilePos.x + tileSize, tileSize*tilePos.y);
		UVs[1] = new Vector2(tileSize*tilePos.x + tileSize, tileSize*tilePos.y + tileSize);
		UVs[2] = new Vector2(tileSize*tilePos.x, tileSize*tilePos.y + tileSize);
		UVs[3] = new Vector2(tileSize*tilePos.x, tileSize*tilePos.y);

		return UVs;
	}
}
