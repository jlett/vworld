using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class BlockDirt : BlockTerrain {
	public BlockDirt() : base() {

	}
	
	public override Tile GetTexturePosition (Direction direction) {
		Tile tile = new Tile();
		tile.x = 1;
		tile.y = 0;
		return tile;
	}
}