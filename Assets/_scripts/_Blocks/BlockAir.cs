using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class BlockAir : Block {

	public BlockAir() : base() {
		
	}

	public override MeshData BlockData (Chunk chunk, int x, int y, int z, MeshData meshData) {
		return meshData;
	}

	public override bool IsSolid (Direction direction) {
		return false;//not solid on all sides
	}

	public override Tile GetTexturePosition(Direction direction, Chunk chunk = null, int x = 0, int y = 0, int z = 0) {
		Tile tile = new Tile();
		tile.x = 3;
		tile.y = 1;
		
		return tile;
	}
}
