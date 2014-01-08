using UnityEngine;
using System.Collections;

//shows some bitmap and pauses the game
public class ShowText : MonoBehaviour {
	public Texture2D image;

	private bool active = false;
	private Pause pause;

	void Awake() {
		pause = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<Pause>();
	}

	public void ShowImage() {
		if (pause.Puase()) {
			active = true;
		}
	}

	void OnGUI() {
		if (active) {
			float xMin = (Screen.width / 2) - (image.width / 2);
			float yMin = (Screen.height / 2) - (image.height / 2);
			GUI.DrawTexture(new Rect(xMin, yMin, image.width, image.height), image);
			if(GUI.Button(new Rect(Screen.width / 2 - 90, Screen.height - 40, 180, 40), "Resume")) {
				active = false;
				pause.Play();
			}
		}
	}
}
