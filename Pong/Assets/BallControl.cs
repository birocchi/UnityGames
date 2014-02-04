using UnityEngine;
using System.Collections;

public class BallControl : MonoBehaviour {

	// Use this for initialization
	void Start () {
		float randomNumber = Random.Range(0.0f,1.0f);
		if (randomNumber <= 0.5f) {
			Debug.Log("Shoot left");
			rigidbody2D.AddForce(new Vector2(80,10));
		}
		else {
			Debug.Log("Shoot right");
			rigidbody2D.AddForce(new Vector2(-80,-10));
		}
	}
	
	void OnCollisionEnter2D(Collision2D collisionInfo){
		if(collisionInfo.collider.tag == "Player"){
			rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, rigidbody2D.velocity.y + collisionInfo.collider.rigidbody2D.velocity.y/3);
		}
	}
}
