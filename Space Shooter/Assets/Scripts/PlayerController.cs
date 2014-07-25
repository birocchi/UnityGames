using UnityEngine;
using System.Collections;

[System.Serializable]
public class Boundary{
	public float xMin, xMax, zMin, zMax;
}

public class PlayerController : MonoBehaviour {

	public float speed;
	public float tilt;
	public Boundary boundary;

	public GameObject shot;
	public Transform shotSpawn;
	public float fireRate;
	private float nextFire;

	public Vector3 touchOffset;

	void Update(){
		if(Input.GetButton("Fire1")){
			Shoot();
		}
	}

	void FixedUpdate () {

		//Touches
		if(Input.touchCount > 0){
			foreach(Touch touch in Input.touches){
				Vector3 worldPoint = Camera.main.ScreenToWorldPoint(touch.position) + touchOffset;
				Vector3 touchToPlayer = worldPoint - transform.position;
				rigidbody.velocity = new Vector3(Mathf.Clamp (touchToPlayer.x,-1,1),0,Mathf.Clamp (touchToPlayer.z,-1,1)) * speed;
				Shoot ();
			}
		}
		//Mouse
		else if(Input.GetMouseButton(0)){
			Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition) + touchOffset;
			Vector3 touchToPlayer = worldPoint - transform.position;
			rigidbody.velocity = new Vector3(Mathf.Clamp (touchToPlayer.x,-1,1),0,Mathf.Clamp (touchToPlayer.z,-1,1)) * speed;
			Shoot ();
		}
		//Keyboard
		else {
			float moveHorizontal = Input.GetAxis("Horizontal");
			float moveVertical = Input.GetAxis("Vertical");

			rigidbody.velocity = new Vector3(moveHorizontal, 0, moveVertical) * speed;
		}

		rigidbody.position = new Vector3
		(
			Mathf.Clamp(rigidbody.position.x,boundary.xMin,boundary.xMax),
			0,
			Mathf.Clamp(rigidbody.position.z,boundary.zMin,boundary.zMax)
		);

		rigidbody.rotation = Quaternion.Euler(0, 0, rigidbody.velocity.x * -tilt);
	}

	void Shoot(){
		if(Time.time > nextFire){
			nextFire = Time.time + fireRate;
			Instantiate(shot,shotSpawn.position,shotSpawn.rotation);
			audio.Play();
		}
	}
}
