using UnityEngine;
using System.Collections;

//tracks players health, stamina and mana
//stamina and mana are also recharged if not used for a certain time
public class PlayerBars : MonoBehaviour {
	public float health = 1f;
	public float stamina = 5f;
	public float mana = 10f;
	public float staminaRechargeCooldown = 5f;
	public float manaRechargeCooldown = 3f;
	public float maxHealth;
	public float maxStamina;
	public float maxMana;

	private float staminaTimer = 0f;
	private float manaTimer = 0f;

	void Awake() {
		maxHealth = health;
		maxStamina = stamina;
		maxMana = mana;
	}

	// Update is called once per frame
	void Update () {
		if (staminaTimer == 0f) {
			stamina = Mathf.Min(maxStamina, stamina + Time.deltaTime);
		} else {
			staminaTimer = Mathf.Max(0f, staminaTimer - Time.deltaTime);
		}
		if (manaTimer == 0f) {
			mana = Mathf.Min(maxMana, mana + Time.deltaTime);
		} else {
			manaTimer = Mathf.Max(0f, manaTimer - Time.deltaTime);
		}
	}

	public bool TakeStamina(float requiredStamina) {
		if (stamina >= requiredStamina) {
			stamina -= requiredStamina;
			staminaTimer = staminaRechargeCooldown;
			return true;
		}
		return false;
	}
	
	public bool TakeMana(float requiredMana) {
		if (mana >= requiredMana) {
			mana -= requiredMana;
			manaTimer = manaRechargeCooldown;
			return true;
		}
		return false;
	}

	public void TakeHealth(float healthRequired) {
		health = Mathf.Max(0f, health - healthRequired);
	}
}
