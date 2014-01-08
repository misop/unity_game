using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//tracks enemies health
//remembers current phase, how many phases there are and order of abilities required to move to the next phase
public class EnemyHealth : MonoBehaviour {
	public float comboTimeOut = 2f;
	public int phases = 3;
	public int phase = 1;
	public int[] phase1;
	public int[] phase2;
	public int[] phase3;
	public List<int> combo;

	private float comboTimer = 0f;
	private bool activeCombo = false;
	private EnemyAI enemyAI;
	private Score score;

	void Awake() {
		combo = new List<int>();
		enemyAI = GameObject.FindGameObjectWithTag(Tags.enemy).GetComponent<EnemyAI>();
		score = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<Score>();
	}

	void Update() {
		if (comboTimer > 0) {
			comboTimer = Mathf.Max(0f, comboTimer - Time.deltaTime);
		}
		if (activeCombo == true && comboTimer == 0f) {
			activeCombo = false;
			CheckDamage();
			combo.Clear();
		}
	}

	void CheckDamage() {
		bool success = false;
		if (phase == 1) success = ComboSuccess(phase1);
		if (phase == 2) success = ComboSuccess(phase2);
		if (phase == 3) success = ComboSuccess(phase3);

		if (success) {
			score.ComputeScore(phase, combo);
			phase++;
			activeCombo = false;
			comboTimer = 0f;
			enemyAI.Hide();
		}
	}

	bool ComboSuccess(int[] comboList) {
		if (combo.Count >= comboList.Length) {
			bool hit = true;
			for (int i = 0; i < comboList.Length; i++) {
				if (comboList[i] != combo[i]) {
					hit = false;
					break;
				}
			}
			return hit;
		}

		return false;
	}
	//add ability to the list
	public void HitBy(int ability) {
		if (comboTimer == 0f) {
			activeCombo = true;
			combo.Clear();
		}
		comboTimer = comboTimeOut;
		combo.Add(ability);
	}

	public bool Dead() {
		return (phase > phases);
	}

	public bool UnderAttack() {
		return comboTimer > 0f;
	}
}
