using UnityEngine;
using System.Collections;

//dsiplays crosshair in the middle of the screen and locs the cursor
public class Crosshair : MonoBehaviour {
	public Texture2D crosshairImage;

	void Update() {
		Screen.lockCursor = true;
	}

	void OnGUI()
	{
		float xMin = (Screen.width / 2) - (crosshairImage.width / 2);
		float yMin = (Screen.height / 2) - (crosshairImage.height / 2);
		GUI.DrawTexture(new Rect(xMin, yMin, crosshairImage.width, crosshairImage.height), crosshairImage);
	}
}
