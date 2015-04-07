using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class WorldSave {
	public Vector3 pos;
	public string seed;
	
	public WorldSave(Player p) {
		pos = p.transform.position;
		seed = p.world.seed;
	}
}