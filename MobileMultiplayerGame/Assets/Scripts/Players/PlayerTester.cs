using UnityEngine;
using System.Collections;

public class PlayerTester : MonoBehaviour {
	
	public float acceleration;
	public float maxSpeed;
	public float turningSpeed;
	public GameObject shotObject;
	public float fireRate;
	public float touchDistanceTolerance = 10f;
	
	private Transform shotSpawn;
	private Transform playerShip;
	private float nextFire;
	
	//Mobile controller
	private Vector2 initialTouchPosition;
	
	void Awake () {
		playerShip = transform.FindChild("Ship");
		shotSpawn = playerShip.FindChild("ShotSpawn");
	}
	
	void Update(){
		#if !UNITY_ANDROID
		if(Input.GetButton("Fire1")){
			if(Time.time > nextFire){
				nextFire = Time.time + fireRate;
				Instantiate(shotObject, shotSpawn.position, shotSpawn.rotation);
			}
		}
		#endif
	}
	
	void FixedUpdate () {
		float verticalMove = Input.GetAxis("Vertical");
		
		#if UNITY_ANDROID
		if(Input.touchCount > 0){
			foreach(Touch touch in Input.touches){
				if(touch.position.x < Screen.width/2){
					switch(touch.phase){
					case TouchPhase.Began:
						initialTouchPosition = touch.position;
						break;
					case TouchPhase.Moved:
					case TouchPhase.Stationary:
						if(touch.position.y - initialTouchPosition.y > touchDistanceTolerance ){
							verticalMove = 1;
						}
						else if(touch.position.y - initialTouchPosition.y < -touchDistanceTolerance){
							verticalMove = -1;
						}
						else{
							verticalMove = 0;
						}
						break;
					}
				}
				else{
					if(Time.time > nextFire){
						nextFire = Time.time + fireRate;
						Network.Instantiate(shotObject, shotSpawn.position, shotSpawn.rotation, 0 );
					}
				}
			}
		}
		#endif
		
		rigidbody2D.AddForce(playerShip.up * verticalMove * acceleration);
		
		if(rigidbody2D.velocity.magnitude > maxSpeed){
			rigidbody2D.velocity = rigidbody2D.velocity.normalized * maxSpeed;
		}
		
		float horizontalRotation = -Input.GetAxis("Horizontal");
		
		#if UNITY_ANDROID
		Vector3 accelerometer = Input.acceleration;
		
		if(accelerometer.sqrMagnitude > 0.5f){
			if(accelerometer.x > 0.1f){
				horizontalRotation = -1;
			}
			else if(accelerometer.x < -0.1f){
				horizontalRotation = 1;
			}
			else{
				horizontalRotation = 0;
			}
		}
		#endif
		
		playerShip.Rotate(0, 0, horizontalRotation * turningSpeed);
		
	}
	
	void OnCollisionEnter2D(Collision2D other){
		if(other.gameObject.tag.Equals("Shot") && !other.gameObject.networkView.isMine){
			//temporary
			Destroy(this.gameObject, 0.5f);
		}
	}
	
}

