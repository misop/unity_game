using UnityEngine;
using System.Collections;

//handles turning off enemies light
public class EnemyLight : MonoBehaviour {
	private float timer = 0f;
	private Light spotLight;
	private EnemyAI enemyAI;

	void Awake() {
		spotLight = GameObject.FindGameObjectWithTag(Tags.enemyLight).GetComponent<Light>();
		enemyAI = GameObject.FindGameObjectWithTag(Tags.enemy).GetComponent<EnemyAI>();
	}
	
	// Update is called once per frame
	void Update () {
		if (timer == 0f) {
			spotLight.enabled = true;
		} else {
			timer = Mathf.Max(0f, timer - Time.deltaTime);
		}
	}

	public void TurnOff(float time) {
		timer = time;
		enemyAI.Stun(time);
		if (Random.value > 0.5) enemyAI.FindClosestCorners();
		spotLight.enabled = false;
	}
}
