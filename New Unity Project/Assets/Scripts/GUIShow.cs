using UnityEngine;
using System.Collections;

//shows all the bars in GUI
//player health, stamina, mana, enemies health, remaining time, abilities and current combo
public class GUIShow : MonoBehaviour {
	public float blinkSpeed = 0.5f;
	public Rect healthPos;
	public Rect manaPos;
	public Rect staminaPos;
	public Rect enemyHealthPos;
	public Rect timePos;
	public Rect scorePos;
	public Vector2 abilityPos;
	public Vector2 comboAbilityPos;
	public Texture2D ability1Image;
	public Texture2D ability2Image;
	public Texture2D ability3Image;
	public Texture2D ability4Image;

	private PlayerBars playerBars;
	private PlayerAbilities playerAbilities;
	private EnemyHealth enemyHealth;
	private GameTime gameTime;
	private Score score;
	private Texture2D red;
	private Texture2D green;
	private Texture2D blue;
	private Texture2D black;
	private Texture2D yellow;
	private float blinkCounter = 0f;
	private bool blink = false;
	private Color textColor;

	void Awake() {
		playerBars = GameObject.FindGameObjectWithTag(Tags.player).GetComponent<PlayerBars>();
		playerAbilities = GameObject.FindGameObjectWithTag(Tags.player).GetComponent<PlayerAbilities>();
		enemyHealth = GameObject.FindGameObjectWithTag(Tags.enemy).GetComponent<EnemyHealth>();
		gameTime = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<GameTime>();
		score = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<Score>();

		healthPos.y = Screen.height - healthPos.y;
		manaPos.y = Screen.height - manaPos.y;
		staminaPos.y = Screen.height - staminaPos.y;
		enemyHealthPos.x = Screen.width/2f - enemyHealthPos.x;

		timePos.x = Screen.width - timePos.width - timePos.x;
		
		abilityPos.x = Screen.width/2f - abilityPos.x;
		abilityPos.y = Screen.height - abilityPos.y;

		comboAbilityPos.x = Screen.width - comboAbilityPos.x;

		red = new Texture2D(1, 1);
		red.SetPixel(0,0, Color.red);
		red.Apply();
		green = new Texture2D(1, 1);
		green.SetPixel(0,0, Color.green);
		green.Apply();
		blue = new Texture2D(1, 1);
		blue.SetPixel(0,0, Color.blue);
		blue.Apply();
		black = new Texture2D(1, 1);
		black.SetPixel(0,0, Color.black);
		black.Apply();
		yellow = new Texture2D(1, 1);
		yellow.SetPixel(0,0, Color.yellow);
		yellow.Apply();

		textColor = new Color(0.97f, 0.53f, 0f);
	}

	void Update() {
		Blinking();
	}

	void OnGUI() {
		DrawPlayerBars();
		DrawEnemyHealth();
		DrawPlayerAbilities();
		DrawComboAbilities();
		DrawTime();
		DrawScore();
	}

	void DrawPlayerBars() {
		Rect healthRect = healthPos;
		healthRect.width = healthPos.width * playerBars.health;
		Rect manaRect = manaPos;
		manaRect.width = manaPos.width * playerBars.mana/playerBars.maxMana;
		Rect staminaRect = staminaPos;
		staminaRect.width = staminaPos.width * playerBars.stamina/playerBars.maxStamina;
		
		DrawBar(healthRect, red);
		DrawBar(manaRect, blue);
		DrawBar(staminaRect, green);
	}

	void DrawTime() {
		Rect timeRect = timePos;
		timeRect.width = timeRect.width * gameTime.remainingTime / gameTime.time;

		DrawBar(timeRect, yellow);
	}

	void DrawEnemyHealth() {
		Rect enemyRect = enemyHealthPos;
		for (int i = enemyHealth.phase; i <= enemyHealth.phases; i++) {
			if (i == enemyHealth.phases && enemyHealth.UnderAttack() && blink) {
				DrawBar(enemyRect, black);
			} else {
				DrawBar(enemyRect, red);
			}
			enemyRect.x += enemyRect.width + 5f;
		}
	}

	void DrawPlayerAbilities() {
		Vector2 pos = abilityPos;
		float cd;
		GUI.skin.label.normal.textColor = textColor;
		GUI.skin.label.fontSize = 18;
		GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		if (playerAbilities.Ability1OnCooldown(out cd)) {
			int coold = Mathf.RoundToInt(cd);
			GUI.Label(new Rect(pos.x + 5f, pos.y + 5f, ability1Image.width - 5f, ability1Image.height), coold.ToString());
		} else {
			GUI.DrawTexture(new Rect(pos.x, pos.y, ability1Image.width, ability1Image.height), ability1Image);
		}
		pos.x += ability1Image.width + 5f;
		if (playerAbilities.Ability2OnCooldown(out cd)) {
			int coold = Mathf.RoundToInt(cd);
			GUI.Label(new Rect(pos.x + 5f, pos.y + 5f, ability1Image.width - 5f, ability1Image.height), coold.ToString());
		} else {
			GUI.DrawTexture(new Rect(pos.x, pos.y, ability2Image.width, ability2Image.height), ability2Image);
		}
		pos.x += ability1Image.width + 5f;
		if (playerAbilities.Ability3OnCooldown(out cd)) {
			int coold = Mathf.RoundToInt(cd);
			GUI.Label(new Rect(pos.x + 5f, pos.y + 5f, ability1Image.width - 5f, ability1Image.height), coold.ToString());
		} else {
			GUI.DrawTexture(new Rect(pos.x, pos.y, ability3Image.width, ability3Image.height), ability3Image);
		}
		pos.x += ability1Image.width + 5f;
		if (playerAbilities.Ability4OnCooldown(out cd)) {
			int coold = Mathf.RoundToInt(cd);
			GUI.Label(new Rect(pos.x + 5f, pos.y + 5f, ability1Image.width - 5f, ability1Image.height), coold.ToString());
		} else {
			GUI.DrawTexture(new Rect(pos.x, pos.y, ability4Image.width, ability4Image.height), ability4Image);
		}
	}

	void DrawComboAbilities() {
		Vector2 pos = comboAbilityPos;
		for (int i = 0; i < enemyHealth.combo.Count; i++) {
			Texture2D tex;
			switch (enemyHealth.combo[i]) {
			case 1:
				tex = ability1Image;
				break;
			case 2:
				tex = ability2Image;
				break;
			case 3:
				tex = ability3Image;
				break;
			case 4:
				tex = ability4Image;
				break;
			default:
				tex = ability1Image;
				break;
			}
			GUI.DrawTexture(new Rect(pos.x, pos.y, tex.width, tex.height), tex);
			pos.y += tex.height + 5f;
		}
	}

	void DrawScore() {
		GUI.skin.label.normal.textColor = textColor;
		GUI.skin.label.fontSize = 18;
		GUI.skin.label.alignment = TextAnchor.UpperLeft;
		GUI.Label(scorePos, "Score: " + score.score.ToString());
	}

	void Blinking() {
		blinkCounter += Time.deltaTime;
		if (blinkCounter >= blinkSpeed) {
			blink = !blink;
			blinkCounter = 0f;
		}
	}

	void DrawBar(Rect position, Texture2D color) {
		float offset = 5f;
		bool good = true;
		if (position.width  < offset) {
			position.width = offset;
			good = false;
		}
		DrawQuad(new Rect(position.x - offset, position.y - offset, position.width + 2f*offset, position.height + 2f*offset), black);
		if (good == true) DrawQuad(position, color);
	}

	void DrawQuad(Rect position, Texture2D color) {
		GUI.skin.box.normal.background = color;
		GUI.Box(position, GUIContent.none);
	}

}
