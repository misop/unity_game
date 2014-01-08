using UnityEngine;
using System.Collections;

//if object is within range and mouse is over it it will shine
public class SelectableObject : MonoBehaviour {
	public float range = 10f;
	public bool isActive = true;

	private bool isSelected = false;
	private Transform player;

	void Awake() {
		player = GameObject.FindGameObjectWithTag(Tags.player).transform;
	}

	void OnMouseOver() {
		if (isSelected == true) return;

		if (isActive && Vector3.Distance(player.position, transform.position) <= range) {
			renderer.material.shader = Shader.Find("Self-Illumin/Diffuse");
		} else {
			renderer.material.shader = Shader.Find("Diffuse");
		}
	}
	
	void OnMouseExit(){
		if (isSelected == true) return;

		renderer.material.shader = Shader.Find("Diffuse");
	}

	public void Select() {
		isSelected = true;
		renderer.material.shader = Shader.Find("Self-Illumin/Diffuse");
	}
	public void Deselect() {
		isSelected = false;
		renderer.material.shader = Shader.Find("Diffuse");
	}
}
