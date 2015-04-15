using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class BlockAir : Block {

	public BlockAir() : base() {
		
	}
	
	public override bool IsSolid (Direction direction) {
		return false;//not solid on all sides
	}
}
