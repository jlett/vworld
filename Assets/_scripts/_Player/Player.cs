using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	
	public World world;
	private PhotonView photonView;
	
	void Start () {
		photonView = PhotonView.Get(gameObject);

		world = GameObject.FindGameObjectWithTag("World").GetComponent<World>();

		if(world == null){
			Debug.LogError("uhhh, where's my world data? -player");
		}

//		if(!photonView.isMine) {
//			GetComponent<LoadChunks>().enabled = false;
//			GetComponent<GameModeSurvival>().enabled = false;
//			GetComponent<GameModeCreative>().enabled = false;
//			GetComponent<CharacterController>().enabled = false;
//		} else {
//			tag = "MyPlayer";
//			transform.GetChild(0).gameObject.SetActive(true);
//			GetComponent<LoadChunks>().world = world;
//		}
	}
}
