using UnityEngine;
using System.Collections;

//counts the game time
public class GameTime : MonoBehaviour {
	public float time = 20f;
	public float remainingTime;

	void Awake() {
		time = time * 60f;
		remainingTime = time;
	}
	
	// Update is called once per frame
	void Update () {
		if (remainingTime > 0f) remainingTime = Mathf.Max(0f, remainingTime - Time.deltaTime * Time.timeScale);
	}
}
