using UnityEngine;
using System.Collections;

public class Face {
	public int rotation = 0;//each increment is 90 deg clockwise TODO actually implement this
	public Block block;
	public bool isVisible;//different from block isSolid! this is during runtime whether the face is actually culled or not

	public Face(Block b, bool s = false) {
		block = b;
		isVisible = s;
	}

	public bool CanMeshWith(Face other) {
		return ( block.GetType().Equals(other.block.GetType()) && isVisible == other.isVisible && rotation == other.rotation);
	}
}
