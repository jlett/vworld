using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class BlockTerrain : Block {
	public BlockTerrain() : base() {
		isTerrain = true;
	}

	public override MeshData BlockData(Chunk chunk, int x, int y, int z, MeshData meshData) {

		return base.BlockData(chunk, x, y, z, meshData);
	}
}
