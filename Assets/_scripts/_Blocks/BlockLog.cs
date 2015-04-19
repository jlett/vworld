using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class BlockLog : Block {
	public BlockLog() : base() {

	}
	
	public override Tile GetTexturePosition (Direction direction, Chunk chunk, int x, int y, int z) {
		Tile tile = new Tile();
		
		if(direction == Direction.up || direction == Direction.down) {
			tile.x = 2;
			tile.y = 1;
			return tile;
		}
		
		tile.x = 1;
		tile.y = 1;
		
		return tile;
	}
}
