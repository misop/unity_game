using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//counts score either as a value added to it or calculates score from combo
public class Score : MonoBehaviour {
	public float score;
	public float stelathBonus = 5f;
	public float timeBonus = 200f;
	public float healthBonus = 100f;
	public float[] scoreTable;

	private PlayerAbilities abilities;
	private EnemySight enemySight;

	void Awake() {
		abilities = GameObject.FindGameObjectWithTag(Tags.player).GetComponent<PlayerAbilities>();
		enemySight = GameObject.FindGameObjectWithTag(Tags.enemy).GetComponent<EnemySight>();
	}

	public void AddScore(float value) {
		score += value;
	}

	public void ComputeScore(int phase, List<int> combo) {
		float multiplier = phase;
		if (!enemySight.sighted) multiplier *= stelathBonus;

		for (int i = 0; i < combo.Count; i++) {
			score += multiplier * scoreTable[combo[i]-1];
			if (abilities.playingMusic) {
				score += multiplier * scoreTable[3];
			}
		}
	}
}
