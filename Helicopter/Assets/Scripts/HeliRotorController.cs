using System;
using UnityEngine;

public class HeliRotorController : MonoBehaviour {

	//Eixo da rotacao
	public enum Axis {
		X, Y, Z
	}
	public Axis RotateAxis;
    
	//Velocidade do rotor, limitado entre 0 e 3000
    public float RotorSpeed
    {
		get { return rotorSpeed; }
		set { rotorSpeed = Mathf.Clamp(value,0,3000); }
    }

	private float rotorSpeed;
    private float rotateDegree;
    private Vector3 originalRotation;

    void Start () {
        originalRotation = transform.localEulerAngles;
	}

	void Update () {

        rotateDegree += RotorSpeed * Time.deltaTime;

		//Limita o angulo de rotacao a um valor entre 0 e 359
	    rotateDegree = rotateDegree % 360;

		switch (RotateAxis)	{
		    case Axis.Y:
		        transform.localRotation = Quaternion.Euler(originalRotation.x, rotateDegree, originalRotation.z);
		        break;
		    case Axis.Z:
		        transform.localRotation = Quaternion.Euler(originalRotation.x, originalRotation.y, rotateDegree);
		        break;
		    default:
		        transform.localRotation = Quaternion.Euler(rotateDegree, originalRotation.y, originalRotation.z);
		        break;
		}
	}
}
