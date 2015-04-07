using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	GameMode gameMode;
	public World world;
	public PhotonView photonView;
	
	void Start () {
		photonView = PhotonView.Get(gameObject);
		
		if(photonView.instantiationData != null) {
			object[] data = photonView.instantiationData;
			world = GameObject.Find((string)data[0]).GetComponent<World>();
		} else {
			Debug.LogError("uhhh, where's my world data? -player");
		}

		transform.GetComponent<LoadChunks>().world = world;
	}
}
