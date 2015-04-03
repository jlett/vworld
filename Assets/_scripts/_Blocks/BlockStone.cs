using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class BlockStone : BlockTerrain {
	public BlockStone() : base() {

	}
	
	public override Tile GetTexturePosition (Direction direction) {
		Tile tile = new Tile();
		tile.x = 0;
		tile.y = 0;
		return tile;
	}
}
