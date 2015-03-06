using UnityEngine;
using System.Collections;

public class ShotController : MonoBehaviour {

	public float speed;
	public float lifeTime;

	public static string DefaultName{ get; set; }

	public NetworkViewID OwnerID { get; set; }

	void Awake(){
		DefaultName = gameObject.name;
	}

	void OnEnable(){
		rigidbody2D.velocity = transform.up * speed;
		StartCoroutine(DestroyObject(lifeTime));
		Debug.Log("Shot " + gameObject.name + " enabled! Lifetime: " + lifeTime + "s");
	}

	void OnCollisionEnter2D(Collision2D other){
		StartCoroutine(DestroyObject(0));
	}

	IEnumerator DestroyObject(float time){
		yield return new WaitForSeconds(time);
		if(Network.isServer){
			networkView.RPC ("DisableShot", RPCMode.All);
			Network.RemoveRPCs(OwnerID);
		}
		Debug.Log(string.Format("Server removed the RPCs from the owner {0}", OwnerID));
	}

	[RPC]
	public void DisableShot(){
		gameObject.SetActive(false);
		gameObject.name = DefaultName;
		Debug.Log(string.Format("Disabled the shot with viewID = {0}", networkView.viewID));
	}
}
