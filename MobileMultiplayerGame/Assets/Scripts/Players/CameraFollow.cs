using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	public Transform followedObject;
	public Vector2 minLimit = new Vector2(-50,-50);
	public Vector2 maxLimit = new Vector2(50,50);
	
	void FixedUpdate () {
		//Follow the object
		if(followedObject != null){
			transform.position = new Vector3(Mathf.Clamp(followedObject.position.x, minLimit.x, maxLimit.x),
			                                 Mathf.Clamp(followedObject.position.y, minLimit.y, maxLimit.y),
			                                 transform.position.z);
		}
	}


}
