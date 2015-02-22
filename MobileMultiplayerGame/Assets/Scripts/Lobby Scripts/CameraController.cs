using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	private float rotationX;
	private float rotationY;
	private float rotationZ;
	
	private float debounce = 0.05f;
	private bool supportsGyro;
	private Gyroscope gyro;
	
	private float validationTime;
	
	void Awake (){
		supportsGyro = SystemInfo.supportsGyroscope;
	}
	
	void Start () {
		if(supportsGyro){
			gyro = Input.gyro;
			gyro.enabled = true;
			gyro.updateInterval = 1f/60f;
		}
	}
	
	void Update () {
		if (supportsGyro){
			Vector3 rotation = gyro.rotationRateUnbiased;
			
			float time = Time.timeSinceLevelLoad;
			float period = time - validationTime;
			validationTime = time;

			if(rotation.magnitude > debounce){
				rotation = ((rotation * period) * 180)/Mathf.PI;
			}
			else{
				rotation = Vector3.zero;
			}

			rotationX = Mathf.Abs(rotation.x) > debounce ? ((rotation.x * period) * 180)/Mathf.PI : 0;
			rotationY = Mathf.Abs(rotation.y) > debounce ? ((rotation.y * period) * 180)/Mathf.PI : 0;
			rotationZ = Mathf.Abs(rotation.z) > 3 * debounce ? ((rotation.z * period) * 180)/Mathf.PI : 0;

			transform.Rotate(-rotationX, -rotationY, rotationZ);
		}
	}
}
