using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	GameMode gameMode;
	public World world;
	
	void Start () {
		transform.GetComponent<LoadChunks>().world = world;
	}
}
