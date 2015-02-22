using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	//Controlling
	public float acceleration;
	public float maxSpeed;
	public float turningSpeed;
	private Transform playerShip;
	private Transform propulsor;
	private bool isPropulsorOn;
	
	//Shooting
	public GameObject shotObject;
	public float fireRate;
	private Transform shotSpawn;
	private float nextFire;

	//Movement synchronization
	private float lastSynchronizationTime;
	private float syncDelay = 0f;
	private float syncTime = 0f;
	private Vector3 syncStartPosition = Vector3.zero;
	private Vector3 syncEndPosition = Vector3.zero;
	private Quaternion syncStartRotation = Quaternion.identity;
	private Quaternion syncEndRotation = Quaternion.identity;

	#if UNITY_ANDROID
	//Mobile controller
	private Vector2 initialTouchPosition;
	private float touchDistanceTolerance = 10f;
	#endif

	//Player Health
	private HealthController health;
	

	void Awake () {
		lastSynchronizationTime = Time.time;
		playerShip = transform.FindChild("Ship");
		propulsor = playerShip.FindChild("Propulsor");
		shotSpawn = playerShip.FindChild("ShotSpawn");
		health = GetComponent<HealthController>();
	}

	void Update(){
		#if !UNITY_ANDROID
		if(networkView.isMine){
			if(Input.GetButton("Fire1")){
				if(Time.time > nextFire){
					nextFire = Time.time + fireRate;
					Network.Instantiate(shotObject, shotSpawn.position, shotSpawn.rotation, 0 );
				}
			}
		}
		#endif
		//Update the position of the other players
		if(!networkView.isMine){
			syncTime += Time.deltaTime;
			transform.position = Vector3.Lerp(syncStartPosition, syncEndPosition, syncTime / syncDelay);
			playerShip.rotation = Quaternion.Lerp(syncStartRotation, syncEndRotation, syncTime / syncDelay);
		}
	}

	void FixedUpdate () {
		//Do nothing if the player was not created by me
		if(networkView.isMine){

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

			//isPropulsorOn = verticalMove > 0 ? true: false;
			//if( isPropulsorOn )
			if(verticalMove > 0)
				propulsor.particleSystem.Play();
			else{
				propulsor.particleSystem.Stop();
				propulsor.particleSystem.Clear();
			}

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

	}

	void OnCollisionEnter2D(Collision2D other){
		if(other.gameObject.tag.Equals("Shot")){
			Debug.Log(string.Format("My Ship: {0}, My Shot: {1}",gameObject.networkView.isMine,other.gameObject.networkView.isMine));

			if((gameObject.networkView.isMine && !other.gameObject.networkView.isMine) ||
			   (!gameObject.networkView.isMine && other.gameObject.networkView.isMine) ||
			   (!gameObject.networkView.isMine && !other.gameObject.networkView.isMine && gameObject.networkView.owner != other.gameObject.networkView.owner)){
				Debug.Log(string.Format("Damage applied to {0}!",gameObject.name));

				if(Network.isServer){
					health.networkView.RPC("ChangeHealth",RPCMode.AllBuffered,-10);
				}
			}
		}
	}

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info){
		Vector3 syncPosition = Vector3.zero;
		Quaternion syncRotation = Quaternion.identity;

		//If is writing on the stream, sends the position;
		if(stream.isWriting){
			syncPosition = transform.position;
			syncRotation = playerShip.rotation;
			stream.Serialize(ref syncPosition);
			stream.Serialize(ref syncRotation);
		}
		//If is reading the stream, set the variables used in interpolating the position
		else if(stream.isReading){
			stream.Serialize(ref syncPosition);
			stream.Serialize(ref syncRotation);

			//Reset the timer and calculate the delay between the network sent packages
			syncTime = 0f;
			syncDelay = Time.time - lastSynchronizationTime;
			lastSynchronizationTime = Time.time;

			//Set the initial position and the destination
			syncStartPosition = transform.position;
			syncEndPosition = syncPosition;

			//Set the initial rotation and the destination
			syncStartRotation = playerShip.rotation;
			syncEndRotation = syncRotation;
		}
	}

}

