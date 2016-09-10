using UnityEngine;

public class FollowTargetCamera : MonoBehaviour {

    public Transform Target;
    public float positionFolowForce = 5f;
    public float rotationFolowForce = 5f;

    void FixedUpdate() {

		//Aplica a rotacao atual do alvo no vetor frontal
        Vector3 direction = Target.rotation * Vector3.forward;

		//Ignora o componente Y da direcao
		direction.y = 0f;

		//Move suavemente para o alvo
        transform.position = Vector3.Lerp(transform.position, Target.position, positionFolowForce * Time.deltaTime);

		//Rotaciona suavemente para apontar para a frente do alvo
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction.normalized), rotationFolowForce * Time.deltaTime);
	}
}

