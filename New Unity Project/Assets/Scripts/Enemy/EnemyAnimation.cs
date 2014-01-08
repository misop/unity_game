using UnityEngine;
using System.Collections;

//taken from Unity tutorial
//my adition was stun that does not allow movement
public class EnemyAnimation : MonoBehaviour {
	public float deadZone = 10f;
	public bool stuned = false;

	private NavMeshAgent nav;
	private Animator anim;
	private AnimatorSetup animSetup;
	private HashIDs hash;

	void Awake() {
		nav = GetComponent<NavMeshAgent>();
		anim = GetComponent<Animator>();
		hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();
		animSetup = new AnimatorSetup(anim, hash);

		nav.updatePosition = false;
		nav.updatePosition = true;
		deadZone *= Mathf.Deg2Rad;
	}

	void Update() {
		if (!stuned)
			NavAnimSetup();
	}

	void OnAnimatorMove() {
		nav.velocity = anim.deltaPosition / Time.deltaTime;
		transform.rotation = anim.rootRotation;
	}

	void NavAnimSetup() {
		float speed;
		float angle;

		speed = Vector3.Project(nav.desiredVelocity, transform.forward).magnitude;
		angle = FindAngle(transform.forward, nav.desiredVelocity, transform.up);

		if (Mathf.Abs(angle) < deadZone) {
			transform.LookAt(transform.position + nav.desiredVelocity);
			angle = 0f;
		}
		animSetup.Setup(speed, angle);
	}

	float FindAngle(Vector3 from, Vector3 to, Vector3 up) {
		if (to == Vector3.zero) return 0f;

		float angle = Vector3.Angle(from, to);
		Vector3 normal = Vector3.Cross(from, to);
		angle *= Mathf.Sign(Vector3.Dot(normal, up));
		angle *= Mathf.Deg2Rad;

		return angle;
	}

	public void Face(Vector3 pos) {
		Vector3 to = (pos - transform.position).normalized;
		float angle = FindAngle(transform.forward, to, transform.up) * 0.7f;
		if (Mathf.Abs(angle) < deadZone) {
			angle = 0f;
		}
		animSetup.Setup(0, angle);
	}

	public void Rotate(float angle) {
		angle *= Mathf.Deg2Rad;
		animSetup.Setup(0, angle);
	}

	public void Attack(bool playerInReach) {
		animSetup.SetupAttacking(playerInReach);
	}

	public void Stop() {
		animSetup.Setup(0f, 0f);
	}
}
