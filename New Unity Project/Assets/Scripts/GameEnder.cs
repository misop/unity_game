using UnityEngine;
using System.Collections;

//ends the game if some conditions are met
//also adds score for remaining time and health if player has won
public class GameEnder : MonoBehaviour {
	private PlayerBars playerBars;
	private EnemyHealth enemyHealth;
	private GameTime gameTime;
	private Pause pause;
	private Score score;
	private bool ended = false;
	private bool victory = false;
	private Color textColor;

	void Awake() {
		playerBars = GameObject.FindGameObjectWithTag(Tags.player).GetComponent<PlayerBars>();
		enemyHealth = GameObject.FindGameObjectWithTag(Tags.enemy).GetComponent<EnemyHealth>();
		gameTime = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<GameTime>();
		score = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<Score>();
		pause = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<Pause>();

		textColor = new Color(0.97f, 0.53f, 0f);
	}

	// Update is called once per frame
	void Update () {
		if (!ended && (playerBars.health <= 0f || enemyHealth.phase > enemyHealth.phases || gameTime.remainingTime <= 0f)) {
			//end the game
			victory = enemyHealth.phase > enemyHealth.phases;
			if (victory) {
				score.score += score.healthBonus * playerBars.health;
				score.score += score.timeBonus * gameTime.remainingTime / gameTime.time;
			}
			score.score = Mathf.Round(score.score);
			ended = true;
			pause.EndGame();
		}
	}

	void OnGUI() {
		if (ended) {
			//layout start
			GUI.BeginGroup(new Rect(Screen.width / 2 - 300, Screen.height / 2 - 100, 600, 250));

			GUI.skin.label.normal.textColor = textColor;
			GUI.skin.label.fontSize = 60;
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
			if (victory) {
				GUI.Label(new Rect(200, 0, 200, 70), "Victory!");
			} else {
				GUI.Label(new Rect(200, 0, 200, 70), "Failure!");
			}
			GUI.skin.label.fontSize = 40;
			GUI.Label(new Rect(200, 60, 200, 50), "Score: ");
			GUI.Label(new Rect(0, 100, 600, 50), score.score.ToString());
			//restart button (level 0)
			if(GUI.Button(new Rect(215, 150, 180, 40), "Restart")) {
				Application.LoadLevel(0);
			}			
			//quit button
			if(GUI.Button(new Rect(215, 200, 180, 40), "Quit")) {
				Application.Quit();
			}	

			//layout end
			GUI.EndGroup(); 
		}
	}
}
