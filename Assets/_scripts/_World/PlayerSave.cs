using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class PlayerSave {
	public Vector3 pos;
	
	public PlayerSave(Player p) {
		pos = p.transform.position;
	}
}