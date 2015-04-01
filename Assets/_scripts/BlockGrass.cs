using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class BlockGrass : Block {
	public BlockGrass() : base() {

	}

	public override Tile GetTexturePosition (Direction direction) {
		Tile tile = new Tile();
		if(direction == Direction.up) {
			tile.x = 2;
			tile.y = 0;
		} else if(direction == Direction.down) {
			tile.x = 0;
			tile.y = 0;
		} else {
			tile.x = 3;
			tile.y = 0;
		}
		return tile;
	}
}
