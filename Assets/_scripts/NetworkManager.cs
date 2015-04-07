using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {

	public GameObject playerPrefab;
	public RoomInfo[] roomsList;
	private string worldName;

	void Start () {
		PhotonNetwork.ConnectUsingSettings("0.0.1");
	}

	void OnReceivedRoomListUpdate() {
		roomsList = PhotonNetwork.GetRoomList();
	}
	
	void OnJoinedRoom() {
		object[] data = new object[1];
		data[0] = worldName;
		PhotonNetwork.Instantiate("_prefabs/" + playerPrefab.name, new Vector3(0, 100, 0), Quaternion.identity, 0, data);	
	}

	public void StartWorld(string name) {
		worldName = name;
		PhotonNetwork.CreateRoom(name, true, true, 4);
	}
}
