using UnityEngine;
using System.Collections;

public class LookAtTargetController : MonoBehaviour {

    public Transform Target;
    public float damping = 6.0f;
	
    void LateUpdate() {

		//Pega a rotacao necessaria para alinhar a camera com o alvo
		Quaternion rotation = Quaternion.LookRotation(Target.position - transform.position);

		//Rotaciona de maneira suave usando interpolacao
		transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
    }
}
