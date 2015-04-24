using UnityEngine;
using System.Collections;

public class ReflectAsteroids : MonoBehaviour {

	public float maxReflectionVelocity = 2f;

	void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.tag == "Asteroid"){
			if(other.relativeVelocity.magnitude < maxReflectionVelocity){
				other.gameObject.GetComponent<Rigidbody2D>().velocity = other.relativeVelocity;
			}
			else{
				other.gameObject.GetComponent<Rigidbody2D>().velocity = other.relativeVelocity.normalized * maxReflectionVelocity;
			}

		}
		
	}
}
