using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class BlockTerrain : Block {

	public BlockTerrain() : base() {
		isTerrain = true;
	}
}
