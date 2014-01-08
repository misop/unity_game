using UnityEngine;
using System.Collections;

//idea from unity totorial to use only one collider and test Field of view
//also added a lot of my own stuff
public class EnemySight : MonoBehaviour {
	public Vector3 resetPosition = new Vector3(1000, 1000, 1000);
	public float fieldOfViewAngle = 110.0f;
	public float rangeAngle = 30f;
	public bool playerInSight;
	public bool playerInReach;
	public bool playerHeard;
	public Vector3 lastSight;
	public bool sighted = false;

	private NavMeshAgent nav;
	private SphereCollider col;
	private Animator anim;
	private GameObject player;
	private Vector3 previousSight;
	private FPSControllerInputExtras playerMovement;
	private CharacterMotor chMotor;

	void Awake() {
		nav = GetComponent<NavMeshAgent>();
		col = GetComponent<SphereCollider>();
		anim = GetComponent<Animator>();
		player = GameObject.FindGameObjectWithTag(Tags.player);
		playerMovement = player.GetComponent<FPSControllerInputExtras>();
		chMotor =  player.GetComponent<CharacterMotor>();
		lastSight = resetPosition;
		previousSight = resetPosition;
	}

	void Update() {
		previousSight = lastSight;
	}

	void OnTriggerStay(Collider other) {
		if (other.gameObject == player) {
			playerInSight = false;
			playerInReach = false;
			playerHeard = false;
			float dist = Vector3.Distance(transform.position, other.transform.position);
			//check if AI can hear the player
			if (playerMovement.moved == true && (chMotor.movement.maxForwardSpeed > playerMovement.crchSpeed || dist < 2)) {
				lastSight = player.transform.position;
				playerHeard = true;
			}
			//check if AI can see the player
			Vector3 direction = other.transform.position - transform.position;
			float angle = Vector3.Angle(direction, transform.forward);
			//enemy eyes are arround 1.7
			Vector3 enemyEyes = transform.position + transform.up * 1.7f;
			Vector3 playerEyes = player.transform.position + player.transform.up * 0.2f;//0.4f;

			if (angle < fieldOfViewAngle * 0.5f) {
				RaycastHit hit;
				Debug.DrawRay (enemyEyes, (playerEyes - enemyEyes).normalized * 10, Color.red);

				if (Physics.Raycast(enemyEyes, (playerEyes - enemyEyes).normalized, out hit, col.radius)) {
					if (hit.collider.gameObject == player) {
						sighted = true;
						playerInSight = true;
						lastSight = player.transform.position;
						if (hit.distance < 1.5f && angle < rangeAngle) {
							playerInReach = true;
						}
					}
				}
			}
		}
	}

	void OnTriggerExit(Collider other) {
		if (other.gameObject == player) {
			playerInSight = false;
		}
	}

	public void HearSound(Vector3 pos, float strength) {
		if (Vector3.Distance(pos, transform.position) <= strength) {
			if (lastSight == resetPosition) {
				lastSight = pos;
			}
		}
	}
}
