using UnityEngine;
using System.Collections;

//pauses the game
//the different variables serve to separate pause from another interupts such as reading a scroll
public class Pause : MonoBehaviour {
	private bool paused = false;
	private bool stoped = false;
	private Crosshair crossHair;
	private GUIShow guiShow;
	private MouseLook mouseLookX;
	private MouseLook mouseLookY;

	void Awake() {
		crossHair = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<Crosshair>();
		guiShow = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<GUIShow>();
		mouseLookX = GameObject.FindGameObjectWithTag(Tags.player).GetComponent<MouseLook>();
		mouseLookY = GameObject.FindGameObjectWithTag(Tags.mainCamera).GetComponent<MouseLook>();
	}

	void Update() {
		if (!stoped && Input.GetKeyDown(KeyCode.Escape)) {
			paused = !paused;
			if (paused) {
				Stop();
			} else {
				Start();
			}
		}
	}

	void OnGUI() {
		if (paused) {
			//layout start
			GUI.BeginGroup(new Rect(Screen.width / 2 - 150, Screen.height / 2 - 130, 300, 250));

			//game resume button
			if(GUI.Button(new Rect(55, 100, 180, 40), "Resume")) {
				paused = false;
				Start();
			}			
			//restart button (level 0)
			if(GUI.Button(new Rect(55, 150, 180, 40), "Restart")) {
				Application.LoadLevel(0);
			}			
			//quit button
			if(GUI.Button(new Rect(55, 200, 180, 40), "Quit")) {
				Application.Quit();
			}		

			//layout end
			GUI.EndGroup(); 
		}
	}

	public void EndGame() {
		stoped = true;
		paused = false;
		Stop();
	}

	public bool Puase() {
		if (!paused) {
			stoped = true;
			Stop();
		}
		return !paused;
	}

	public void Play() {
		if (!paused) {
			stoped = false;
			Start();
		}
	}

	void Stop() {
		crossHair.enabled = false;
		guiShow.enabled = false;
		mouseLookX.enabled = false;
		mouseLookY.enabled = false;
		Time.timeScale = 0f;
		Screen.lockCursor = false;
	}

	void Start() {
		crossHair.enabled = true;
		guiShow.enabled = true;
		mouseLookX.enabled = true;
		mouseLookY.enabled = true;
		Time.timeScale = 1f;		
	}
}
