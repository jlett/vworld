using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class BlockLeaf : Block {
	public BlockLeaf() : base() {

	}
	
	public override Tile GetTexturePosition (Direction direction, Chunk chunk, int x, int y, int z) {
		Tile tile = new Tile();
		tile.x = 0;
		tile.y = 1;
		return tile;
	}

	public override bool IsSolid (Direction direction) 	{
		return false;
	}
}
