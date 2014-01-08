using UnityEngine;
using System.Collections;

//basic AI for haunted object
//waits for enemy to enter its trigger zone and then fears him
public class HauntedObject : MonoBehaviour {
	public GameObject effect;
	public bool active = false;
	public float stunDuration = 1f;
	public float cooldown = 15f;
	
	private EnemyAI enemyAI;
	private EnemyHealth enemyHealth;
	private SelectableObject selection;
	private GameObject enemyBody;
	private float cooldownTimer = 0f;

	void Awake() {
		enemyAI = GameObject.FindGameObjectWithTag(Tags.enemy).GetComponent<EnemyAI>();
		enemyHealth = GameObject.FindGameObjectWithTag(Tags.enemy).GetComponent<EnemyHealth>();
		enemyBody = GameObject.FindGameObjectWithTag(Tags.enemyBody);
		selection = GetComponent<SelectableObject>();
	}

	void Update() {
		if (cooldownTimer > 0) {
			cooldownTimer = Mathf.Max(0f, cooldownTimer - Time.deltaTime);
		} else {
			selection.isActive = true;
		}
	}

	void OnTriggerStay(Collider other) {
		if (active && cooldownTimer == 0f && other.gameObject == enemyBody) {
			active = false;
			cooldownTimer = cooldown;
			selection.isActive = false;
			Object obj = Instantiate(effect, transform.position, Quaternion.identity);
			Destroy(obj, 1f);
			enemyAI.Stun(stunDuration);
			selection.Deselect();
			enemyHealth.HitBy(1);
		}
	}

	public void Haunt() {
		active = true;
		selection.Select();
	}
}
