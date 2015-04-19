using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;

public class MenuManager : MonoBehaviour {
	
	public GameObject mainMenu, optionsMenu, playMenu;
	public GameObject newWorldOptions, loadWorldOptions, joinWorldOptions;
	public GameObject loadWorldScrollContent, joinWorldScrollContent;
	public GameObject newWorldNameInput, newWorldSeedInput;

	public GameObject listItem, world;
	public GameObject playerPrefab;

	void Start () {
		ShowMainMenu();
	}

	void Update() {

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

		//clear any existing items
		foreach (Transform item in loadWorldScrollContent.transform) 
			Destroy(item.gameObject);

		//load items
		int numItems = 8;
		int itemHeight = ((int)listItem.GetComponent<RectTransform>().rect.height) + 6;//includes padding

		if(loadWorldScrollContent.GetComponent<RectTransform>().rect.height < numItems*itemHeight) {
			loadWorldScrollContent.GetComponent<RectTransform>().sizeDelta = new Vector2(listItem.GetComponent<RectTransform>().rect.width, numItems*itemHeight);
		}

		for(int i = 0; i < numItems; i++) {
			GameObject itemObject = GameObject.Instantiate(listItem) as GameObject;
			itemObject.transform.SetParent(loadWorldScrollContent.transform, false);
			itemObject.GetComponent<Toggle>().group = loadWorldScrollContent.GetComponent<ToggleGroup>();
			itemObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -i*itemHeight);
		}
	}
	
	public void ShowJoinWorldOptions() {
		newWorldOptions.SetActive(false);
		loadWorldOptions.SetActive(false);
		joinWorldOptions.SetActive(true);

		//clear any existing items
		foreach (Transform item in joinWorldScrollContent.transform) 
			Destroy(item.gameObject);
		
		//load items
		RoomInfo[] roomsList = GetComponent<NetworkManager>().roomsList;
		int numItems = roomsList.Length;
		int itemHeight = ((int)listItem.GetComponent<RectTransform>().rect.height) + 6;//includes padding
		
		if(joinWorldScrollContent.GetComponent<RectTransform>().rect.height < numItems*itemHeight) {
			joinWorldScrollContent.GetComponent<RectTransform>().sizeDelta = new Vector2(listItem.GetComponent<RectTransform>().rect.width, numItems*itemHeight);
		}
		
		for(int i = 0; i < numItems; i++) {
			GameObject itemObject = GameObject.Instantiate(listItem) as GameObject;
			itemObject.transform.SetParent(joinWorldScrollContent.transform, false);
			itemObject.GetComponent<Toggle>().group = joinWorldScrollContent.GetComponent<ToggleGroup>();
			itemObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -i*itemHeight);
			itemObject.GetComponentInChildren<Text>().text = roomsList[i].name;
		}
	}

	public void CreateWorld() {
		bool isPublic = newWorldOptions.GetComponentInChildren<Toggle>().isOn;
		string worldName = newWorldNameInput.GetComponent<InputField>().text;
		string worldSeed = newWorldSeedInput.GetComponent<InputField>().text;

		mainMenu.SetActive(false);
		optionsMenu.SetActive(false);
		playMenu.SetActive(false);
	
		GetComponent<NetworkManager>().StartWorld(worldName, worldSeed, isPublic);

		Debug.Log("creating world with name: " + worldName + ", seed: " + worldSeed + " and public: " + isPublic.ToString());
	}

	public void LoadWorld() {
		if(loadWorldScrollContent.GetComponent<ToggleGroup>().ActiveToggles().Count() == 0)
			return;
		string name = loadWorldScrollContent.GetComponent<ToggleGroup>().ActiveToggles().First().GetComponentsInChildren<Text>()[0].text;

		mainMenu.SetActive(false);
		optionsMenu.SetActive(false);
		playMenu.SetActive(false);

		Debug.Log(name);
	}

	public void JoinWorld() {
		if(joinWorldScrollContent.GetComponent<ToggleGroup>().ActiveToggles().Count() == 0)
			return;
		string name = joinWorldScrollContent.GetComponent<ToggleGroup>().ActiveToggles().First().GetComponentsInChildren<Text>()[0].text;

		mainMenu.SetActive(false);
		optionsMenu.SetActive(false);
		playMenu.SetActive(false);

		GetComponent<NetworkManager>().JoinWorld(name);
	}
}
