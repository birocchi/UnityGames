using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Magnet : MonoBehaviour {

	List<HingeJoint> juntas = new List<HingeJoint>();

	float originalMass = 1;

	void Update () {

		//Solta as caixas grudadas no ima
		if(Input.GetButton("Fire2")){
			foreach(HingeJoint junta in juntas){
				junta.gameObject.GetComponent<Rigidbody>().mass = originalMass;
				Component.Destroy(junta);
			}
			juntas.Clear();
		}
	}

	void OnCollisionEnter(Collision coll) {

		if (coll.gameObject.tag == "Crate" && coll.gameObject.GetComponent<HingeJoint>() == null) {

			//Deixa a caixa mais leve para nao bugar a corda
			Rigidbody crateBody = coll.gameObject.GetComponent<Rigidbody>();
			originalMass = crateBody.mass;
			crateBody.mass = 0.0000001f;

			//Se o ima colidiu com uma caixa, cria um hinge joint na caixa para gruda-la a o ima
			HingeJoint junta = coll.gameObject.AddComponent<HingeJoint>();
			junta.connectedBody = GetComponent<Rigidbody>();
			junta.useMotor = true;
			junta.anchor = coll.transform.InverseTransformPoint(coll.contacts[0].point);
			junta.connectedAnchor = new Vector3(0,-0.15f,0);
			juntas.Add(junta);
		}
	}
}
