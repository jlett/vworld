using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	
	public World world;
	private PhotonView photonView;
	
	void Start () {
		photonView = PhotonView.Get(gameObject);
		
		if(photonView.instantiationData != null) {
			object[] data = photonView.instantiationData;
			world = GameObject.FindGameObjectWithTag("World").GetComponent<World>();
		} else {
			Debug.LogError("player got no instantiation data");
		}

		if(world == null){
			Debug.LogError("uhhh, where's my world data? -player");
		}

		Debug.Log(photonView.isMine + " - " + name);

		if(!photonView.isMine) {
			GetComponent<LoadChunks>().enabled = false;
			GetComponent<GameModeSurvival>().enabled = false;
			GetComponent<GameModeCreative>().enabled = false;
			GetComponent<CharacterController>().enabled = false;
		} else {
			tag = "MyPlayer";
			transform.GetChild(0).gameObject.SetActive(true);
			GetComponent<LoadChunks>().world = world;
		}
	}
}
