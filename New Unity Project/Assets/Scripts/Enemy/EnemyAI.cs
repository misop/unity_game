using UnityEngine;
using System.Collections;
using System.Collections.Generic;

struct SortPos {
	public float dist;
	public Vector3 pos;
}

class StructComparer : IComparer<SortPos>
{
	public int Compare(SortPos x, SortPos y)
	{
		return x.dist.CompareTo(y.dist);
	}
}
//base skeleton from Unity tutorial but upgraded and improved
public class EnemyAI : MonoBehaviour {
	public float patrolSpeed = 2f;
	public float chaseSpeed = 4f;
	public float hidingSpeed = 5f;
	public float chaseWaitTime = 5f;
	public float searchWaitTime = 3f;
	public float patrolWaitTime = 1f;
	public float hideWaitTime = 8f;
	public float searchCornerDistance = 10f;
	public int searchCornerCount = 3;
	public bool attackAndChase = true;
	public Transform[] patrolWayPoints;
	public Transform[] searchCorners;
	public Transform[] hidePlaces;

	private EnemySight sight;
	private EnemyAnimation enemyAnimations;
	private NavMeshAgent nav;
	private float chaseTimer = 0f;
	private float searchTimer = 0f;
	private float patrolTimer = 0f;
	private float hideTimer = 0f;
	private int wayPointIndex = 0;
	private Queue<Vector3> toSearch;
	private IComparer<SortPos> spc;
	private int firstLeft = 0;
	private float stunDuration;
	private Vector3 hidePlace;

	void Awake() {
		sight = GetComponent<EnemySight>();
		enemyAnimations = GetComponent<EnemyAnimation>();
		nav = GetComponent<NavMeshAgent>();
		toSearch = new Queue<Vector3>();
		spc = new StructComparer();
		nav.destination = patrolWayPoints[0].position;
		stunDuration = 0f;
		hidePlace = sight.resetPosition;
	}

	void Update() {
		enemyAnimations.Attack(false);
		if (stunDuration > 0f) {
			Stunned();
			return;
		} else {
			enemyAnimations.stuned = false;
		}
		//esentialy a decision tree enemy will pick only one action
		if (hidePlace != sight.resetPosition) {
			Hiding();
			patrolWaitTime = 0f;
		} else if (sight.playerInReach) {
			Attacking();
			if (attackAndChase) Chasing();
		} else if (sight.lastSight != sight.resetPosition) {
			Chasing();
		} else if (toSearch.Count > 0) {
			Searching();
			patrolTimer = 0f;
		} else {
			Patrolling();
			chaseTimer = 0f;
		}
	}

	void Stunned() {
		nav.speed = 0f;
		stunDuration -= Time.deltaTime;
		enemyAnimations.Stop();
	}
	//atatcks the player
	void Attacking() {
		//enemyAnimations.Face(sight.lastSight);
		enemyAnimations.Attack(sight.playerInReach);
	}
	//goes to a hinding spot and waits some time
	void Hiding() {
		nav.speed = hidingSpeed;

		if (nav.remainingDistance < nav.stoppingDistance) {
			hideTimer += Time.deltaTime;

			if (hideTimer >= hideWaitTime) {
				sight.lastSight = sight.resetPosition;
				sight.sighted = false;
				hidePlace = sight.resetPosition;
				hideTimer = 0f;
			}
		} else {
			hideTimer = 0f;
		}
		nav.destination = hidePlace;
	}
	//quickly moves to the last known location of the player
	void Chasing () {
		// Create a vector from the enemy to the last sighting of the player.
		Vector3 sightingDeltaPos = sight.lastSight - transform.position;
		
		// If the the last personal sighting of the player is not close...
		if(sightingDeltaPos.sqrMagnitude > 1.5f)
			// ... set the destination for the NavMeshAgent to the last personal sighting of the player.
			nav.destination = sight.lastSight;
		
		// Set the appropriate speed for the NavMeshAgent.
		nav.speed = chaseSpeed;
		
		// If near the last personal sighting...
		if(nav.remainingDistance < nav.stoppingDistance)
		{
			enemyAnimations.Face(sight.lastSight);
			// ... increment the timer.
			chaseTimer += Time.deltaTime;
			
			// If the timer exceeds the wait time...
			if(chaseTimer >= chaseWaitTime)
			{
				// ... reset last global sighting, the last personal sighting and the timer.
				sight.lastSight = sight.resetPosition;
				FindClosestCorners();
				//nav.destination = toSearch.Peek();
				//chaseTimer = 0f;
			}
		}
		else {
			chaseTimer = 0f;
		}
	}
	//search nearby key waypoints in order to locate the player
	void Searching() {
		nav.speed = chaseSpeed;
		
		if (nav.remainingDistance < nav.stoppingDistance || chaseTimer > 0f) {
			LookArround(60f, chaseTimer, searchWaitTime, 0.075f);
			chaseTimer += Time.deltaTime;

			if (chaseTimer > searchWaitTime) {
				toSearch.Dequeue();
				if (toSearch.Count > 0)
					nav.destination = toSearch.Peek();
				chaseTimer = 0f;
			}
		} else {
			chaseTimer = 0f;
			nav.destination = toSearch.Peek();
		}
	}
	//patrolls along a given route from waypoints
	void Patrolling() {
		nav.speed = patrolSpeed;

		if (nav.remainingDistance < nav.stoppingDistance || patrolTimer > 0f) {
			LookArround(60f, patrolTimer, patrolWaitTime);
			patrolTimer += Time.deltaTime;

			if (patrolTimer >= patrolWaitTime) {
				if (wayPointIndex == patrolWayPoints.Length - 1)
					wayPointIndex = 0;
				else
					wayPointIndex++;

				patrolTimer = 0f;
				nav.destination = patrolWayPoints[wayPointIndex].position;
			}
		} else {
			patrolTimer = 0f;
			nav.destination = patrolWayPoints[wayPointIndex].position;
		}

	}
	//find closes key waypoints for searching
	public void FindClosestCorners() {
		FindClosestCorners(transform.position, true);
	}
	//fins up to a number of waypoints or all waypoints taht are close enough
	public void FindClosestCorners(Vector3 pos, bool clear = true) {
		//sort waypoints by distance
		if (clear) toSearch.Clear();
		List<SortPos> close = new List<SortPos>();
		for (int i = 0; i < searchCorners.Length; i++) {
			SortPos sp;
			sp.dist = Vector3.Distance(pos, searchCorners[i].position) + (Random.value - 0.5f) * 2f;
			sp.pos = searchCorners[i].position;
			close.Add(sp);
		}
		close.Sort(spc);
		//get enough close ones and add to queue
		for (int i = 0; i < close.Count; i++) {
			if (toSearch.Count < searchCornerCount || close[i].dist < searchCornerDistance) {
				toSearch.Enqueue(close[i].pos);
			} else {
				break;
			}
		}
		nav.destination = toSearch.Peek();
		chaseTimer = 0f;
	}
	//looking arround either from left to right or behind
	void LookArround(float angle, float timer, float turnLeftTime, float turnChance = 0.05f) {
		if (timer == 0f) {
			float rand = Random.value;
			if (rand < turnChance || rand > 1f - turnChance) {
				firstLeft = rand < 0.5 ? 2 : 3;
			} else {
				firstLeft = rand > 0.5 ? 0 : 1;
			}
		}
		//we are looking behind
		if (firstLeft > 1) {
			turnLeftTime /= 2f;
			if (timer < turnLeftTime)
				enemyAnimations.Rotate(firstLeft == 2 ? -120f : 120f);
			else
				enemyAnimations.Rotate(firstLeft == 2 ? 120f : -120f);
			return;
		}
		turnLeftTime /= 3f;
		//look arround
		if (timer < turnLeftTime)
			enemyAnimations.Rotate(firstLeft == 0 ? -angle : angle);
		else
			enemyAnimations.Rotate(firstLeft == 0 ? angle : -angle);
	}
	//stuns the enemy, normaly the enemy would be "stuned" by a apropiate animation but sience they are missing...
	public void Stun(float duration) {
		stunDuration = duration;
		enemyAnimations.stuned = true;
		enemyAnimations.Stop();
	}
	//checks if enemy can hear the sound (also doeas some stuff with enemy so its not in senses)
	public bool HearSound(Vector3 pos, float strength) {
		if (Vector3.Distance(pos, transform.position) <= strength) {
			if (sight.lastSight == sight.resetPosition) {
				float chance = 0.1f;
				if (toSearch.Count > 0) chance = Mathf.Min(((float)toSearch.Count)/2f, 1f);
				toSearch.Clear();
				toSearch.Enqueue(pos);
				nav.destination = toSearch.Peek();
				chaseTimer = 0f;
				if (Random.value > 1f - chance) FindClosestCorners(pos, false);
				//return true;
			}
			return true;
		}
		return false;
	}
	//enemy picks a random place to hide
	public void Hide() {
		int idx = Mathf.RoundToInt(Random.value * (float)hidePlaces.Length);
		if (idx < 0) idx = 0;
		if (idx >= hidePlaces.Length) idx = hidePlaces.Length - 1;
		hidePlace = hidePlaces[idx].position;
		hideTimer = 0f;
	}
}
