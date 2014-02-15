using UnityEngine;
using System.Collections;

public class BallControl : MonoBehaviour {

	Vector2 ballSpeed = new Vector2 (80, 10);

	IEnumerator Start () {
		Debug.Log ("Waiting to start the game");
		yield return new WaitForSeconds(3);
		Debug.Log ("Waited 3 seconds! Game Start!");
		GoBall ();
	}
	
	void OnCollisionEnter2D(Collision2D collisionInfo){
		if(collisionInfo.collider.tag == "Player"){
			rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, rigidbody2D.velocity.y + collisionInfo.collider.rigidbody2D.velocity.y/3);
		}
	}

	IEnumerator ResetBall(){
		rigidbody2D.velocity = new Vector2 (0, 0);
		transform.position = new Vector2 (0, 3);

		Debug.Log ("Reseting the game");
		yield return new WaitForSeconds(1);
		Debug.Log ("Game Reseted! Game Start!");
		GoBall ();

	}

	void GoBall () {
		float randomNumber = Random.Range(0.0f,1.0f);

		if (randomNumber <= 0.5f) {
			Debug.Log("Shoot left");
			rigidbody2D.AddForce(ballSpeed);
		}
		else {
			Debug.Log("Shoot right");
			rigidbody2D.AddForce(-ballSpeed);
		}
	}
}
