using UnityEngine;
using System.Collections;

public class GameMode : MonoBehaviour {
	protected bool pauseMenuUp = false;

	void Start() {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	void Update() {
		HandleInput();
	}

	protected virtual void HandleInput(){
		if(Input.GetKeyDown(KeyCode.Escape)) {
			pauseMenuUp = !pauseMenuUp;
			if(pauseMenuUp) {
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
				ShowPauseMenu();
			} else {
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			}
		}
	}

	protected virtual void ShowPauseMenu() {
		
	}
}
