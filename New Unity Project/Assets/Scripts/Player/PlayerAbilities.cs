using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//handles all players abilities
//each ability can have assigned activation range, cooldown, graphical effect, sound effect, mana cost
public class PlayerAbilities : MonoBehaviour {
	public float activateRange = 1f;
	public float globalCooldown = 1f;
	public Object effect1;
	public float cooldown1 = 3f;
	public float manaCost1 = 3f;
	public float range1 = 10f;
	public Object effect2;
	public float cooldown2 = 15f;
	public float manaCost2 = 5f;
	public float range2 = 10f;
	public Object effect3;
	public AudioClip sound3;
	public float cooldown3 = 15f;
	public float manaCost3 = 5f;
	public float range3 = 10f;
	public AudioClip sound4;
	public float cooldown4 = 60f;
	public float manaCost4 = 10f;
	public bool playingMusic = false;

	private PlayerBars playerBars;
	private float globalCooldownTimer = 0f;
	private float cooldown1Timer = 0f;
	private float cooldown2Timer = 0f;
	private float cooldown3Timer = 0f;
	private float cooldown4Timer = 0f;
	private float timer4 = 0f;
	private GameObject enemy;
	private EnemySight enemySenses;
	private EnemyAI enemyAI;
	private EnemyHealth enemyHealth;

	void Awake() {
		playerBars = GameObject.FindGameObjectWithTag(Tags.player).GetComponent<PlayerBars>();
		enemy = GameObject.FindGameObjectWithTag(Tags.enemy);
		enemySenses = enemy.GetComponent<EnemySight>();
		enemyAI = enemy.GetComponent<EnemyAI>();
		enemyHealth = enemy.GetComponent<EnemyHealth>();
	}
	
	// Update is called once per frame
	void Update () {
		LoverCooldowns();
		if (globalCooldownTimer > 0f) {
			globalCooldownTimer = Mathf.Max(0f, globalCooldownTimer - Time.deltaTime);
			return;
		}
		if (timer4 == 0f) {
			playingMusic = false;
		}

		//checks if ability has been activated
		if (Input.GetButtonDown("ActivateObject")) {
			ActivateObject();
		}
		if (Input.GetButtonDown("Ability1")) {
			HauntObject();
		}
		if (Input.GetButtonDown("Ability2")) {
			TurnLightsOff();
		}
		if (Input.GetButton("Ability3")) {
			SpookySound();
		}
		if (Input.GetButtonDown("Ability4")) {
			ScaryMusic();
		}
	}

	void LoverCooldowns() {
		if (cooldown1Timer > 0) cooldown1Timer = Mathf.Max(0f, cooldown1Timer - Time.deltaTime);
		if (cooldown2Timer > 0) cooldown2Timer = Mathf.Max(0f, cooldown2Timer - Time.deltaTime);
		if (cooldown3Timer > 0) cooldown3Timer = Mathf.Max(0f, cooldown3Timer - Time.deltaTime);
		if (cooldown4Timer > 0) cooldown4Timer = Mathf.Max(0f, cooldown4Timer - Time.deltaTime);
		if (timer4 > 0) timer4 = Mathf.Max(0f, timer4 - Time.deltaTime);
	}

	void ActivateObject() {
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out hit)) {
			ShowText showScript = hit.collider.gameObject.GetComponent<ShowText>();
			if (showScript != null && hit.distance <= activateRange) {
				showScript.ShowImage();
			}
		}
	}

	void HauntObject() {
		if (cooldown1Timer > 0) return;

		if (playerBars.TakeMana(manaCost1)) {
			CreateEffect(effect1, 1f, transform.position + transform.forward * 3f + transform.up * 0.6f);
			globalCooldownTimer = globalCooldown;
			cooldown1Timer = cooldown1;

			//get the object polayer is looking at and try to haunt it
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit)) {
				HauntedObject hauntedScript = hit.collider.gameObject.GetComponent<HauntedObject>();
				if (hauntedScript != null && hit.distance <= range1) {
					//hauntedScript.active = true;
					hauntedScript.Haunt();
				}
			}
		}
	}

	void TurnLightsOff() {
		if (cooldown2Timer > 0) return;
		
		if (playerBars.TakeMana(manaCost2)) {
			CreateEffect(effect2, 1f, transform.position + transform.forward * 1f + transform.up * 0.6f);
			globalCooldownTimer = globalCooldown;
			cooldown2Timer = cooldown2;
			
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			//if player hit his enemy it will turn his light off
			if (Physics.Raycast(ray, out hit)) {
				EnemyLight enemyLight = hit.collider.gameObject.transform.parent.gameObject.GetComponent<EnemyLight>();
				if (enemyLight != null && hit.distance <= range2) {
					enemyLight.TurnOff(3f);
					enemyHealth.HitBy(2);
				}
			}
		}
	}

	void SpookySound() {
		if (cooldown3Timer > 0) return;

		if (playerBars.TakeMana(manaCost3)) {
			globalCooldownTimer = globalCooldown;
			cooldown3Timer = cooldown3;

			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Vector3 spawn;

			//plays a sound at a given location
			if (Physics.Raycast(ray, out hit, range3)) {
				spawn = ray.origin + ray.direction * hit.distance;
			} else {
				spawn = transform.position + transform.forward * range3 + transform.up * 0.6f;
			}
			AudioSource.PlayClipAtPoint(sound3, spawn);
			CreateEffect(effect3, 1f, spawn);
			//enemySenses.HearSound(spawn, 10);
			if (enemyAI.HearSound(spawn, 10)) {
				enemyHealth.HitBy(3);
			}
		}
	}
	
	void ScaryMusic() {
		if (cooldown4Timer > 0) return;

		if (playerBars.TakeMana(manaCost4)) {
			globalCooldownTimer = globalCooldown;
			cooldown4Timer = cooldown4;
			timer4 = sound4.length;

			AudioSource.PlayClipAtPoint(sound4, Camera.main.transform.position);
			playingMusic = true;
		}
	}

	void CreateEffect(Object effect, float time, Vector3 pos) {
		Object createdEffect = Instantiate(effect, pos, transform.rotation);
		Destroy(createdEffect, time);
	}
	
	public bool Ability1OnCooldown(out float cd) {
		cd = cooldown1Timer;
		return cooldown1Timer > 0f;
	}
	
	public bool Ability2OnCooldown(out float cd) {
		cd = cooldown2Timer;
		return cooldown2Timer > 0f;
	}
	
	public bool Ability3OnCooldown(out float cd) {
		cd = cooldown3Timer;
		return cooldown3Timer > 0f;
	}
	
	public bool Ability4OnCooldown(out float cd) {
		cd = cooldown4Timer;
		return cooldown4Timer > 0f;
	}
}
