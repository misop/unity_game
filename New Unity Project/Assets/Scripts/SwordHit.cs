using UnityEngine;
using System.Collections;

//deals damage from range if sword hits the player
//global cooldown is here because otherwise player would be instantly dead
public class SwordHit : MonoBehaviour {
	public float minDamage = 0.4f;
	public float maxDamage = 0.6f;
	public float cooldown = 1f;

	private GameObject player;
	private PlayerBars playerBars;
	private float timer = 0f;
	
	void Awake() {
		player = GameObject.FindGameObjectWithTag(Tags.player);
		playerBars = player.GetComponent<PlayerBars>();
	}

	void Update() {
		if (timer > 0f) timer -= Time.deltaTime;
	}

	void OnTriggerEnter(Collider other) {
		if (timer <= 0f && other.gameObject == player) {
			playerBars.TakeHealth(Random.value * (maxDamage - minDamage) + minDamage);
			timer = cooldown;
		}
	}

}
