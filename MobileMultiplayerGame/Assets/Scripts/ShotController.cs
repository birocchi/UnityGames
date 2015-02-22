using UnityEngine;
using System.Collections;

public class ShotController : MonoBehaviour {

	public float speed;
	public float lifeTime;
	
	void Start () {
		rigidbody2D.velocity = transform.up * speed;
		StartCoroutine(DestroyObject(lifeTime));
	}

	void OnCollisionEnter2D(Collision2D other){
		StartCoroutine(DestroyObject(0));
	}

	IEnumerator DestroyObject(float time){
		Debug.Log("Destroying the object: " + gameObject.name + " after " + time + "s");
		yield return new WaitForSeconds(time);
		if(Network.isServer){
			Network.RemoveRPCs(networkView.viewID);
			Network.Destroy(this.gameObject);
		}
		Destroy(this.gameObject);
	}
}
