using UnityEngine;
using System.Collections;

public class BallControl : MonoBehaviour {

	Vector2 ballSpeed;
	bool isReseting = false;

	IEnumerator Start () {
		Debug.Log ("Waiting to start the game");
		yield return new WaitForSeconds(3);
		Debug.Log ("Waited 3 seconds! Game Start!");
		GoBall ();
	}
	
	void OnCollisionEnter2D(Collision2D collisionInfo){
		if(collisionInfo.collider.tag == "Player"){
			rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, rigidbody2D.velocity.y + collisionInfo.collider.rigidbody2D.velocity.y/3);
			audio.pitch = Random.Range(0.8f,1.2f);
			audio.Play();
		}
	}

	IEnumerator ResetBall(){
		if(!isReseting){
			isReseting = true;
			rigidbody2D.velocity = new Vector2 (0, 0);
			transform.position = new Vector2 (0, 3);

			Debug.Log ("Reseting the game");
			yield return new WaitForSeconds(1);
			Debug.Log ("Game Reseted! Game Start!");
			GoBall ();
			isReseting = false;
		}
	}

	void GoBall () {
		float randomXdirection = Random.Range(0.0f,1.0f);
		float randomYdirection = Random.Range(0.0f,1.0f);

		if (randomYdirection <= 0.5f) {
			Debug.Log("Shoot Down");
			ballSpeed = new Vector2 (80, -10);
		}
		else {
			Debug.Log("Shoot Up");
			ballSpeed = new Vector2 (80, 10);
		}

		if (randomXdirection <= 0.5f) {
			Debug.Log("Shoot left");
			rigidbody2D.AddForce(ballSpeed);
		}
		else {
			Debug.Log("Shoot right");
			rigidbody2D.AddForce(-ballSpeed);
		}
	}
}
