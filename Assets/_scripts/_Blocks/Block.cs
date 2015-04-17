using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[Serializable]
public class Block {

	public enum Direction {north, south, east, west, up, down};
	public struct Tile { public int x; public int y;}
	const float tileSize = 0.25f;//1 divided by number of tiles per side (aka 0.25 on a 4x4 texture)

	public bool isTerrain = false;//should we pseudo marching cubes the block
	public bool isSmoothShaded = false;//should we manually calc the normals to make the block smoooooth
	public bool changed = true;

	//base constructor
	public Block() {

	}

	public virtual MeshData BlockData(Chunk chunk, int x, int y, int z, MeshData meshData) {
		meshData.useRenderDataForCollision = true;

		if(isSmoothShaded)
			meshData.useCustomNormals = true;

		bool northSolid = chunk.GetBlock(x, y, z+1).IsSolid(Direction.south); 
		bool southSolid = chunk.GetBlock(x, y, z-1).IsSolid(Direction.north);
		bool eastSolid = chunk.GetBlock(x+1, y, z).IsSolid(Direction.west);
		bool westSolid = chunk.GetBlock(x-1, y, z).IsSolid(Direction.east);
		bool upSolid = chunk.GetBlock(x, y+1, z).IsSolid(Direction.down);
		bool downSolid = chunk.GetBlock(x, y-1, z).IsSolid(Direction.up);
		if(!northSolid) {
			meshData = FaceDataNorth(chunk, x, y, z, meshData);
		}
		if(!southSolid) {
			meshData = FaceDataSouth(chunk, x, y, z, meshData);
		}
		if(!eastSolid) {
			meshData = FaceDataEast(chunk, x, y, z, meshData);
		}
		if(!westSolid) {
			meshData = FaceDataWest(chunk, x, y, z, meshData);
		}
		if(!upSolid) {
			meshData = FaceDataUp(chunk, x, y, z, meshData);
		}
		if(!downSolid) {
			meshData = FaceDataDown(chunk, x, y, z, meshData);
		}
		return meshData;
	}

	protected virtual MeshData FaceDataNorth(Chunk chunk, int x, int y, int z, MeshData meshData) {
		meshData = AddFaceQuad(chunk, x, y, z, new int[4] {3, 0, 1, 2}, Direction.north, meshData);

		if(isSmoothShaded) {
			meshData.AddNormal(new Vector3(1, -1, 1));
			meshData.AddNormal(new Vector3(1, 1, 1));
			meshData.AddNormal(new Vector3(-1, 1, 1));
			meshData.AddNormal(new Vector3(-1, -1, 1));
		}
		
		return meshData;
	}

	protected virtual MeshData FaceDataSouth(Chunk chunk, int x, int y, int z, MeshData meshData) {
		meshData = AddFaceQuad(chunk, x, y, z, new int[4] {7, 4, 5, 6}, Direction.south, meshData);

		if(isSmoothShaded) {
			meshData.AddNormal(new Vector3(-1, -1, -1));
			meshData.AddNormal(new Vector3(-1, 1, -1));
			meshData.AddNormal(new Vector3(1, 1, -1));
			meshData.AddNormal(new Vector3(1, -1, -1));
		}

		return meshData;
	}

	protected virtual MeshData FaceDataEast(Chunk chunk, int x, int y, int z, MeshData meshData) {
		meshData = AddFaceQuad(chunk, x, y, z, new int[4] {6, 5, 0, 3}, Direction.east, meshData);

		if(isSmoothShaded) {
			meshData.AddNormal(new Vector3(1, -1, -1));
			meshData.AddNormal(new Vector3(1, 1, -1));
			meshData.AddNormal(new Vector3(1, 1, 1));
			meshData.AddNormal(new Vector3(1, -1, 1));
		}

		return meshData;
	}

	protected virtual MeshData FaceDataWest(Chunk chunk, int x, int y, int z, MeshData meshData) {
		meshData = AddFaceQuad(chunk, x, y, z, new int[4] {2, 1, 4, 7}, Direction.west, meshData);

		if(isSmoothShaded) {
			meshData.AddNormal(new Vector3(-1, -1, 1));
			meshData.AddNormal(new Vector3(-1, 1, 1));
			meshData.AddNormal(new Vector3(-1, 1, -1));
			meshData.AddNormal(new Vector3(-1, -1, -1));
		}

		return meshData;
	}

	protected virtual MeshData FaceDataUp(Chunk chunk, int x, int y, int z, MeshData meshData) {
		meshData = AddFaceQuad(chunk, x, y, z, new int[4] {4, 1, 0, 5}, Direction.up, meshData);

		if(isSmoothShaded) {
			meshData.AddNormal(new Vector3(-1, 1, 1));
			meshData.AddNormal(new Vector3(1, 1, 1));
			meshData.AddNormal(new Vector3(1, 1, -1));
			meshData.AddNormal(new Vector3(-1, 1, -1));
		}

		return meshData;
	}
	
	protected virtual MeshData FaceDataDown(Chunk chunk, int x, int y, int z, MeshData meshData) {
		meshData = AddFaceQuad(chunk, x, y, z, new int[4] {2, 7, 6, 3}, Direction.down, meshData);

		if(isSmoothShaded) {
			meshData.AddNormal(new Vector3(-1, -1, -1));
			meshData.AddNormal(new Vector3(1, -1, -1));
			meshData.AddNormal(new Vector3(1, -1, 1));
			meshData.AddNormal(new Vector3(-1, -1, 1));
		}

		return meshData;
	}

	/*helper functions, numbers refer to point refered to in notes
	 * NUMBER IN CLOCKWISE ORDER
	 * 0 = +++
	 * 1 = -++
	 * 2 = --+
	 * 3 = +-+
	 * 4 = -+-
	 * 5 = ++-
	 * 6 = +--
	 * 7 = ---
	 * 
	 * 8 = 	0++
	 * 9 = 	-0+
	 * 10 = 0-+
	 * 11 = +0+
	 * 12 = 0+-
	 * 13 = +0-
	 * 14 = 0--
	 * 15 = -0-
	 * 16 = -+0
	 * 17 = ++0
	 * 18 = +-0
	 * 19 = --0
	 * dir = which face to texture with
	 */
	protected MeshData AddFaceTri(Chunk chunk, int x, int y, int z, int[] t, Direction dir, MeshData meshData) {
		return AddFaceTri(chunk, x, y, z, t, GetTexturePosition(dir), meshData);
	}

	protected MeshData AddFaceTri(Chunk chunk, int x, int y, int z, int[] t, Tile tile, MeshData meshData) {
		if(t.Length != 3) {
			Debug.LogError("3 points are required for a tri... idiot");
		}
		for(int i = 0; i < 3; i++) {
			if(t[i] == 0) {
				meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
			} else if(t[i] == 1) {
				meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
			} else if(t[i] == 2) {
				meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));
			} else if(t[i] == 3) {
				meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
			} else if(t[i] == 4) {
				meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
			} else if(t[i] == 5) {
				meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
			} else if(t[i] == 6) {
				meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
			} else if(t[i] == 7) {
				meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
			} else if(t[i] == 8) {
				meshData.AddVertex(new Vector3(x, y + 0.5f, z + 0.5f));
			} else if(t[i] == 9) {
				meshData.AddVertex(new Vector3(x - 0.5f, y, z + 0.5f));
			} else if(t[i] == 10) {
				meshData.AddVertex(new Vector3(x, y - 0.5f, z + 0.5f));
			} else if(t[i] == 11) {
				meshData.AddVertex(new Vector3(x + 0.5f, y, z + 0.5f));
			} else if(t[i] == 12) {
				meshData.AddVertex(new Vector3(x, y + 0.5f, z - 0.5f));
			} else if(t[i] == 13) {
				meshData.AddVertex(new Vector3(x + 0.5f, y, z - 0.5f));
			} else if(t[i] == 14) {
				meshData.AddVertex(new Vector3(x, y - 0.5f, z - 0.5f));
			} else if(t[i] == 15) {
				meshData.AddVertex(new Vector3(x - 0.5f, y, z - 0.5f));
			} else if(t[i] == 16) {
				meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z));
			} else if(t[i] == 17) {
				meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z));
			} else if(t[i] == 18) {
				meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z));
			} else if(t[i] == 19) {
				meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z));
			} else {
				Debug.LogError("AddFaceTri(...) vertex out of bounds: " + t[i]);
			}
		}
		meshData.AddTriangle();
		
		meshData.uv.AddRange(GetTriUVs(tile));
		return meshData;
	}
	
	protected MeshData AddFaceQuad(Chunk chunk, int x, int y, int z, int[] t, Tile tile, MeshData meshData) {
		if(t.Length != 4) {
			Debug.LogError("4 points are required for a quad... idiot");
		}
		for(int i = 0; i < 4; i++) {
			if(t[i] == 0) {
				meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
			} else if(t[i] == 1) {
				meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
			} else if(t[i] == 2) {
				meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));
			} else if(t[i] == 3) {
				meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
			} else if(t[i] == 4) {
				meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
			} else if(t[i] == 5) {
				meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
			} else if(t[i] == 6) {
				meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
			} else if(t[i] == 7) {
				meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
			} else if(t[i] == 8) { 
				meshData.AddVertex(new Vector3(x, y + 0.5f, z + 0.5f));
			} else if(t[i] == 9) {
				meshData.AddVertex(new Vector3(x - 0.5f, y, z + 0.5f));
			} else if(t[i] == 10) {
				meshData.AddVertex(new Vector3(x, y - 0.5f, z + 0.5f));
			} else if(t[i] == 11) {
				meshData.AddVertex(new Vector3(x + 0.5f, y, z + 0.5f));
			} else if(t[i] == 12) {
				meshData.AddVertex(new Vector3(x, y + 0.5f, z - 0.5f));
			} else if(t[i] == 13) {
				meshData.AddVertex(new Vector3(x + 0.5f, y, z - 0.5f));
			} else if(t[i] == 14) {
				meshData.AddVertex(new Vector3(x, y - 0.5f, z - 0.5f));
			} else if(t[i] == 15) {
				meshData.AddVertex(new Vector3(x - 0.5f, y, z - 0.5f));
			} else if(t[i] == 16) {
				meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z));
			} else if(t[i] == 17) {
				meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z));
			} else if(t[i] == 18) {
				meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z));
			} else if(t[i] == 19) {
				meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z));
			} else {
				Debug.LogError("AddFaceQuad(...) vertex out of bounds: " + t[i]);
			}
		}
		meshData.AddQuadTriangles();
		
		meshData.uv.AddRange(GetQuadUVs(tile));
		return meshData;
	}

	protected MeshData AddFaceQuad(Chunk chunk, int x, int y, int z, int[] t, Direction dir, MeshData meshData) {
		return AddFaceQuad(chunk, x, y, z, t, GetTexturePosition(dir), meshData);
	}

	//to be overridden by special case blocks
	public virtual bool IsSolid(Direction direction) {
		return true;//by default block is solid on all sides
	}

	//to be overridden by textured blocks (aka like everything)
	public virtual Tile GetTexturePosition(Direction direction, Chunk chunk = null, int x = 0, int y = 0, int z = 0) {
		Tile tile = new Tile();
		tile.x = 0;
		tile.y = 0;

		return tile;
	}

	public virtual Vector2[] GetQuadUVs(Direction dir) {
		return GetQuadUVs(GetTexturePosition(dir));
	}
	
	public virtual Vector2[] GetTriUVs(Direction dir) {
		return GetTriUVs(GetTexturePosition(dir));
	}

	public virtual Vector2[] GetQuadUVs(Tile tilePos) {
		Vector2[] UVs = new Vector2[4];
		
		UVs[0] = new Vector2(tileSize*tilePos.x, tileSize*tilePos.y);
		UVs[1] = new Vector2(tileSize*tilePos.x, tileSize*tilePos.y + tileSize);
		UVs[2] = new Vector2(tileSize*tilePos.x + tileSize, tileSize*tilePos.y + tileSize);
		UVs[3] = new Vector2(tileSize*tilePos.x + tileSize, tileSize*tilePos.y);
		
		return UVs;
	}
	
	public virtual Vector2[] GetTriUVs(Tile tilePos) {
		Vector2[] UVs = new Vector2[3];
		
		UVs[0] = new Vector2(tileSize*tilePos.x, tileSize*tilePos.y);
		UVs[1] = new Vector2(tileSize*tilePos.x, tileSize*tilePos.y + tileSize);
		UVs[2] = new Vector2(tileSize*tilePos.x + tileSize, tileSize*tilePos.y);
		
		return UVs;
	}
}
