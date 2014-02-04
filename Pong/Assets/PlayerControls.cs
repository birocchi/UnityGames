using UnityEngine;
using System.Collections;

public class PlayerControls : MonoBehaviour {

	public KeyCode moveUp;
	public KeyCode moveDown;

	public float speed = 10;

	// Update is called once per frame
	void Update () {
		if (Input.GetKey (moveUp)) {
			rigidbody2D.velocity = new Vector3(0,speed,0);
		}
		else if (Input.GetKey(moveDown)){
			rigidbody2D.velocity = new Vector3(0,-speed,0);
		}
		else {
			rigidbody2D.velocity = new Vector3(0,0,0);
		}
	}
}
