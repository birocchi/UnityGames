using UnityEngine;
using System.Collections;

public class ShotController : MonoBehaviour {

	public float speed;
	public float lifeTime;

	void OnEnable(){
		rigidbody2D.velocity = transform.up * speed;
		//StartCoroutine(DestroyObject(lifeTime));
	}

	void OnCollisionEnter2D(Collision2D other){
		StartCoroutine(DestroyObject(0));
	}

	IEnumerator DestroyObject(float time){
		Debug.Log("Destroying the object: " + gameObject.name + " after " + time + "s");
		yield return new WaitForSeconds(time);
		if(Network.isServer){
			Debug.Log(string.Format("Calling RPC -DisableShot- using Group: {1}, Owner: {2} ", networkView.viewID, networkView.group, networkView.owner));
			networkView.RPC ("DisableShot", RPCMode.AllBuffered, networkView.viewID);
			Debug.Log(string.Format("Removing RPCs from the owner {0} called in group 2", networkView.owner));
			Network.RemoveRPCs(networkView.owner, 2);
		}
	}

	[RPC]
	public void DisableShot(NetworkViewID shotViewID){
		GameObject shot = NetworkView.Find(shotViewID).gameObject;
		if(shot != null){
			shot.SetActive(false);
			Debug.Log(string.Format("Disabled the shot with viewID = {0}", shotViewID));
		}
	}
}
