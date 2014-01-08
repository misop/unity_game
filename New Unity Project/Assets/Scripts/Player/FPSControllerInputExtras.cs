using UnityEngine;
using System.Collections;

//extends FPS controller
//main code from internet
//mine adition was stamina requirement for sprint and setting crouch height externaly (as it caused problems in the original script)
public class FPSControllerInputExtras : MonoBehaviour {
	public float walkSpeed = 7; // regular speed
	public float crchSpeed = 3; // crouching speed
	public float runSpeed = 20; // run speed
	public float height = 0.8f;
	public float crouchedHeight = 0.4f;
	public bool moved = false;
	public float backwardSpeedReduction = 0.66f;

	private CharacterMotor chMotor;
	private Transform tr;
	private float dist; // distance to ground
	private Vector3 lastPos;
	private PlayerBars playerBars;
	
	// Use this for initialization
	void Start () {
		chMotor =  GetComponent<CharacterMotor>();
		playerBars = GameObject.FindGameObjectWithTag(Tags.player).GetComponent<PlayerBars>();
		tr = transform;
		//CharacterController ch = GetComponent<CharacterController>();
		dist = height/2; // calculate distance to ground
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		float dist = Vector3.Distance(transform.position, lastPos);
		lastPos = transform.position;
		if (Time.deltaTime > 0){ // avoid errors when game paused
			float mspeed = dist / Time.deltaTime; // calculate speed
			if (mspeed > crchSpeed*0.5f){
				moved = true;
			} else {
				moved = false;
			}
		}

		float vScale = height;//1.0f;
		float speed = walkSpeed;
		
		float h = height;

		if (Input.GetKey("left shift") && chMotor.grounded )
		{
			if (playerBars.TakeStamina(Time.fixedDeltaTime)) {
				speed = runSpeed;
			}
		}

		if (Input.GetKey("s")) {
			speed *= backwardSpeedReduction;
		}
		
		if (Input.GetKey(KeyCode.LeftControl))
		{ // press C to crouch
			vScale = crouchedHeight;//0.5f;
			//h = 0.5f * height;
			speed = crchSpeed; // slow down when crouching
		}
		
		chMotor.movement.maxForwardSpeed = speed; // set max speed
		float ultScale = tr.localScale.y; // crouch/stand up smoothly 
		
		Vector3 tmpScale = tr.localScale;
		Vector3 tmpPosition = tr.position;
		
		tmpScale.y = Mathf.Lerp(tr.localScale.y, vScale, 5 * Time.deltaTime);
		tr.localScale = tmpScale;
		
		tmpPosition.y += dist * (tr.localScale.y - ultScale); // fix vertical position  
		tr.position = tmpPosition;
	}
}