using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class BlockGrass : Block {
	public BlockGrass() : base() {

	}

	public override Tile GetTexturePosition () {
		Tile tile = new Tile();
		tile.x = 2;
		tile.y = 0;
		return tile;
	}
}
