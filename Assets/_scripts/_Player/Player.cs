using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	
	public World world;
	private PhotonView photonView;
	public GameObject camera;
	
	void Start () {
		photonView = PhotonView.Get(gameObject);
		
		if(photonView.instantiationData != null) {
			object[] data = photonView.instantiationData;
			world = GameObject.Find((string)data[0]).GetComponent<World>();
		} 

		if(world == null){
			Debug.LogError("uhhh, where's my world data? -player");
		}

		transform.GetComponent<LoadChunks>().world = world;

		if(!photonView.isMine) {
			GetComponent<LoadChunks>().enabled = false;
			GetComponent<GameModeSurvival>().enabled = false;
			GetComponent<GameModeCreative>().enabled = false;
			GetComponent<CharacterController>().enabled = false;
		} else {
			camera.SetActive(true);
		}
	}
}
