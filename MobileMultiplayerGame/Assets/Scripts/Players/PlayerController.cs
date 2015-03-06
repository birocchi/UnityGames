using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	//Controlling
	public float acceleration;
	public float maxSpeed;
	public float turningSpeed;
	private Transform playerShip;
	private Transform propulsor;
	
	//Shooting
	public float fireRate;
	private Transform shotSpawn;
	private float nextFire;
	private ShotsPoolManager shotsPool;

	//Movement synchronization
	private float lastSynchronizationTime;
	private float syncDelay = 0f;
	private float syncTime = 0f;
	private Vector3 syncStartPosition = Vector3.zero;
	private Vector3 syncEndPosition = Vector3.zero;
	private Quaternion syncStartRotation = Quaternion.identity;
	private Quaternion syncEndRotation = Quaternion.identity;
	private float verticalMove = 0f;

	#if UNITY_ANDROID || UNITY_IPHONE
	//Mobile controller
	private Vector2 initialTouchPosition;
	private float touchDistanceTolerance = 10f;
	#endif
	
	//Player info
	private HealthController health;
	public string playerName;
	public int playerNumber;
	public NetworkPlayer netPlayer;
	private ScoreManager score;

	void Awake () {
		lastSynchronizationTime = Time.time;
		playerShip = transform.FindChild("Ship");
		propulsor = playerShip.FindChild("Propulsor");
		shotSpawn = playerShip.FindChild("ShotSpawn");
		health = GetComponent<HealthController>();
		shotsPool = GameObject.Find("ShotsPoolManager").GetComponent<ShotsPoolManager>();
		score = GameObject.Find("GameManager").GetComponent<ScoreManager>();
	}

	void Start(){
		if(networkView.isMine){
			if(Network.isServer){
				playerNumber = score.IncludeServerPlayer(playerName, networkView.owner);
				score.networkView.RPC("IncludePlayer", RPCMode.AllBuffered, playerName, networkView.owner, playerNumber);
			}
			else {
				score.networkView.RPC("IncludeNewPlayerOnServer",RPCMode.Server, playerName, networkView.viewID);
			}
		}
	}

	[RPC]
	public void OnReceivePlayerNumberFromServer(int serverPlayerNumber){
		playerNumber = serverPlayerNumber;
		score.networkView.RPC("IncludePlayer", RPCMode.AllBuffered, playerName, networkView.owner, playerNumber);
	}

	[RPC]
	void EnableShot(string playerID, NetworkViewID shotID, NetworkViewID ownerID){
		GameObject shot = shotsPool.GetObjectByID(shotID);
		if(shot != null){
			shot.transform.position = shotSpawn.position;
			shot.transform.rotation = shotSpawn.rotation;
			shot.name = ShotController.DefaultName + playerID;
			shot.GetComponent<ShotController>().OwnerID = ownerID;
			shot.SetActive(true);
			//Debug.Log("Enabled the shot with viewID = " + shotID);
		}
	}

	void Update(){
		#if !UNITY_ANDROID && !UNITY_IPHONE
		if(networkView.isMine){
			if(Input.GetButton("Fire1")){
				if(Time.time > nextFire){
					nextFire = Time.time + fireRate;

					NetworkViewID shotID = shotsPool.GetFreeObject().GetComponent<NetworkView>().viewID;
					NetworkView playerShootingView = networkView.GetComponents<NetworkView>()[1];
					//Debug.Log("NetworkView used to call the EnableShot RPC: " + playerShootingView.viewID);
					playerShootingView.RPC ("EnableShot", RPCMode.AllBuffered, playerNumber.ToString(), shotID, playerShootingView.viewID);
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

		if(verticalMove > 0)
			propulsor.particleSystem.Play();
		else{
			propulsor.particleSystem.Stop();
			propulsor.particleSystem.Clear();
		}
	}

	void FixedUpdate () {
		//Do nothing if the player was not created by me
		if(networkView.isMine){

			verticalMove = Input.GetAxis("Vertical");

			#if UNITY_ANDROID || UNITY_IPHONE
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
							networkView.group = 2;
							Debug.Log(string.Format("Calling RPC -InstantiateShot- using Group: {0}, Owner: {1} ", networkView.group, networkView.owner));
							networkView.RPC ("InstantiateShot", RPCMode.AllBuffered, networkView.viewID.ToString(), shotsPool.GetFreeObject().networkView.viewID);
							networkView.group = 0;
						}
					}
				}
			}
			#endif

			if(verticalMove > 0){
				rigidbody2D.AddForce(playerShip.up * verticalMove * acceleration);
			}

			if(rigidbody2D.velocity.magnitude > maxSpeed){
				rigidbody2D.velocity = rigidbody2D.velocity.normalized * maxSpeed;
			}

			float horizontalRotation = -Input.GetAxis("Horizontal");

			#if UNITY_ANDROID || UNITY_IPHONE
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
			string shipOwnerID = gameObject.networkView.viewID.ToString();
			string shotOwnerID = other.gameObject.name.Substring(ShotController.DefaultName.Length);

			Debug.Log(string.Format("Shot Collision! Ship Owner ID: {0}, Shot Owner ID: {1}",shipOwnerID,shotOwnerID));

			if(shipOwnerID != shotOwnerID){
				Debug.Log(string.Format("Damage applied to {0}!",gameObject.name));

				if(Network.isServer){
					//Debug.Log(string.Format("Calling RPC -ChangeHealth- using {0}, Group: {1}, Owner: {2} ", networkView.viewID, networkView.group, networkView.owner));
					//Debug.Log(string.Format("Calling RPC -ChangeHealth- through {0}, Group: {1}, Owner: {2} ", health.networkView.viewID, health.networkView.group, health.networkView.owner));
					health.networkView.RPC("ChangeHealth",RPCMode.AllBuffered,-10);
					score.networkView.RPC ("AddScore",RPCMode.AllBuffered, 1, int.Parse(shotOwnerID));
					if(health.isDead){
						//Debug.Log("Removing all RPCs called by " + networkView.viewID + " in group " + networkView.group);
						Network.RemoveRPCs(Network.player);
						Network.Destroy(this.gameObject);
					}
				}
			}
		}
	}

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info){
		Vector3 syncPosition = Vector3.zero;
		Quaternion syncRotation = Quaternion.identity;
		float syncVerticalMove = 0;

		//If is writing on the stream, sends the position;
		if(stream.isWriting){
			syncPosition = transform.position;
			syncRotation = playerShip.rotation;
			syncVerticalMove = verticalMove;
			stream.Serialize(ref syncPosition);
			stream.Serialize(ref syncRotation);
			stream.Serialize(ref syncVerticalMove);
		}
		//If is reading the stream, set the variables used in interpolating the position
		else if(stream.isReading){
			stream.Serialize(ref syncPosition);
			stream.Serialize(ref syncRotation);
			stream.Serialize(ref syncVerticalMove);

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

			verticalMove = syncVerticalMove;
		}
	}
}

