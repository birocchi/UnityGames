using UnityEngine;
using System.Collections;

public class DetectorController : MonoBehaviour {

	public Transform target;
	public float showRadius;

	private Vector3 pivot;

	void Awake(){
		pivot = transform.parent.position;
	}

	void Update(){
		Vector3 relativePosition = target.position - pivot;
		Quaternion rotation = Quaternion.LookRotation(relativePosition,Vector3.forward);
		transform.RotateAround(pivot, Vector3.forward, 20* Time.deltaTime);
	}
}
