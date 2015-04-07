using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour {

	public GameObject mainMenu, optionsMenu, playMenu;
	public GameObject newWorldOptions, loadWorldOptions, joinWorldOptions;

	public string roomName;
	private RoomInfo[] roomsList;

	void Start () {
		ShowMainMenu();

		PhotonNetwork.ConnectUsingSettings("0.0.1");
	}

	void Update() {

	}

	void OnReceivedRoomListUpdate() {
		roomsList = PhotonNetwork.GetRoomList();
	}

	void OnJoinedRoom() {
		Debug.Log("connected to room");
	}

	//--------------------------------------------------button functions--------------------------------------------------

	//main menu buttons
	public void ShowPlayMenu() {
		mainMenu.SetActive(false);
		optionsMenu.SetActive(false);
		playMenu.SetActive(true);
	}

	public void ShowOptionsMenu() {
		mainMenu.SetActive(false);
		optionsMenu.SetActive(true);
		playMenu.SetActive(false);
	}

	public void ShowMainMenu() {
		mainMenu.SetActive(true);
		optionsMenu.SetActive(false);
		playMenu.SetActive(false);

		newWorldOptions.SetActive(false);
		loadWorldOptions.SetActive(false);
		joinWorldOptions.SetActive(false);
	}

	public void ExitGame() {
		Application.Quit();
	}

	//play menu buttons
	public void ShowNewWorldOptions() {
		newWorldOptions.SetActive(true);
		loadWorldOptions.SetActive(false);
		joinWorldOptions.SetActive(false);
	}

	public void ShowLoadWorldOptions() {
		newWorldOptions.SetActive(false);
		loadWorldOptions.SetActive(true);
		joinWorldOptions.SetActive(false);
	}

	public void ShowJoinWorldOptions() {
		newWorldOptions.SetActive(false);
		loadWorldOptions.SetActive(false);
		joinWorldOptions.SetActive(true);
	}
}
