﻿using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {

	public GameObject playerPrefab;
	public RoomInfo[] roomsList;
	private string worldName;
	private string worldSeed;

	void Start () {
		PhotonNetwork.ConnectUsingSettings("0.0.1");
	}

	void OnReceivedRoomListUpdate() {
		roomsList = PhotonNetwork.GetRoomList();
	}

	public void JoinWorld(string name) {
		worldName = name;
		PhotonNetwork.JoinRoom(name);
	}

	void OnJoinedRoom() {
		if(PhotonNetwork.isMasterClient) {
			object[] worldData = new object[2];
			worldData[0] = worldName;
			worldData[1] = worldSeed;
			PhotonNetwork.InstantiateSceneObject("_prefabs/World", Vector3.zero, Quaternion.identity, 0, worldData);
		}
		
		object[] playerData = new object[1];
		playerData[0] = worldName;
		PhotonNetwork.Instantiate("_prefabs/" + playerPrefab.name, new Vector3(0, 100, 0), Quaternion.identity, 0, playerData);	
	}

	public void StartWorld(string name, string seed, bool isPublic) {
		worldName = name;
		worldSeed = seed;

		RoomOptions ro = new RoomOptions();
		ro.maxPlayers = 4;
		ro.isVisible = isPublic;
		ro.isOpen = true;

		PhotonNetwork.CreateRoom(name, ro, null);
	}
}
