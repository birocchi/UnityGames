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
	
	//Player info
	private HealthController health;
	public string playerName;
	public int playerNumber;
	public NetworkPlayer netPlayer;
	private ScoreManager score;

	private AudioSource shotSound;
	private AudioSource propulsorSound;

	void Awake () {
		lastSynchronizationTime = Time.time;
		playerShip = transform.FindChild("Ship");
		propulsor = playerShip.FindChild("Propulsor");
		shotSpawn = playerShip.FindChild("ShotSpawn");
		health = GetComponent<HealthController>();
		shotsPool = GameObject.Find("ShotsPoolManager").GetComponent<ShotsPoolManager>();
		score = GameObject.Find("GameManager").GetComponent<ScoreManager>();
		shotSound = GetComponents<AudioSource>()[0];
		propulsorSound = GetComponents<AudioSource>()[1];
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

	//Receives the player number from the server
	[RPC]
	public void OnReceivePlayerNumberFromServer(int serverPlayerNumber){
		playerNumber = serverPlayerNumber;
		score.networkView.RPC("IncludePlayer", RPCMode.AllBuffered, playerName, networkView.owner, playerNumber);
	}

	//Enable a shot from the pool
	[RPC]
	void EnableShot(string playerID, NetworkViewID shotID, NetworkViewID ownerID){
		GameObject shot = shotsPool.GetObjectByID(shotID);
		if(shot != null){
			shot.transform.position = shotSpawn.position;
			shot.transform.rotation = shotSpawn.rotation;
			shot.name = ShotController.DefaultName + playerID;
			shot.GetComponent<ShotController>().OwnerID = ownerID;
			shot.SetActive(true);
			shotSound.Play();
		}
	}

	void Update(){
		#if !UNITY_ANDROID && !UNITY_IPHONE
		if(networkView.isMine){
			if(Input.GetButton("Fire1")){
				Shoot();
			}
		}
		#endif
		//Update the position of the other players
		if(!networkView.isMine){
			syncTime += Time.deltaTime;
			transform.position = Vector3.Lerp(syncStartPosition, syncEndPosition, syncTime / syncDelay);
			playerShip.rotation = Quaternion.Lerp(syncStartRotation, syncEndRotation, syncTime / syncDelay);
		}

		//Animate the propulsor
		if(verticalMove > 0){
			propulsor.particleSystem.Play();
			if(!propulsorSound.isPlaying){
				propulsorSound.volume = 1f;
				propulsorSound.Play();
			}
		}
		else{
			propulsor.particleSystem.Stop();
			propulsor.particleSystem.Clear();
			if(propulsorSound.isPlaying){
				StartCoroutine(VolumeFade(propulsorSound,0f,0.1f));
			} 
		}
	}

	IEnumerator VolumeFade(AudioSource audioSource, float endVolume, float fadeLength){
		
		float startVolume = audioSource.volume;
		float startTime = Time.time;
		
		while (Time.time < startTime + fadeLength){
			audioSource.volume = startVolume + ((endVolume - startVolume) * ((Time.time - startTime) / fadeLength));
			yield return null;
		}
		
		if (endVolume == 0){
			audioSource.Stop();
		}
	}
	
	void Shoot(){
		if(Time.time > nextFire){
			nextFire = Time.time + fireRate;
			NetworkViewID shotID = shotsPool.GetFreeObject().GetComponent<NetworkView>().viewID;
			NetworkView playerShootingView = networkView.GetComponents<NetworkView>()[1];
			//Debug.Log("NetworkView used to call the EnableShot RPC: " + playerShootingView.viewID);
			playerShootingView.RPC ("EnableShot", RPCMode.AllBuffered, playerNumber.ToString(), shotID, playerShootingView.viewID);
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
						case TouchPhase.Moved:
						case TouchPhase.Stationary:
							verticalMove = 1;
							break;
						case TouchPhase.Ended:
						case TouchPhase.Canceled:
							verticalMove = 0;
							break;
						}
					}
					else{
						Shoot();
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
			string shipOwnerID = playerNumber.ToString();
			string shotOwnerID = other.gameObject.name.Substring(ShotController.DefaultName.Length);

			if(networkView.isMine){
				if(shipOwnerID != shotOwnerID){
					networkView.RPC("GetHurt",RPCMode.AllBuffered,-10);
					networkView.RPC ("GetScore",score.GetNetworkPlayer(int.Parse(shotOwnerID)), 1, int.Parse(shotOwnerID));
					if(health.isDead){
						Network.RemoveRPCs(Network.player);
						Network.Destroy(this.gameObject);
					}
				}
			}
		}
	}

	//Descrease health when hit by a shot
	[RPC]
	public void GetHurt(int amount){
		health.ChangeHealth(amount);
	}

	//Gets the score from a successful fired shot
	[RPC]
	public void GetScore(int value, int playerNumber ){
		score.networkView.RPC("AddScore", RPCMode.AllBuffered, value, playerNumber);
	}

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info){
		Vector3 syncPosition = Vector3.zero;
		Quaternion syncRotation = Quaternion.identity;
		float syncVerticalMove = 0;

		//If is writing on the stream, sends the position, rotation and acceleration;
		if(stream.isWriting){
			syncPosition = transform.position;
			syncRotation = playerShip.rotation;
			syncVerticalMove = verticalMove;
			stream.Serialize(ref syncPosition);
			stream.Serialize(ref syncRotation);
			stream.Serialize(ref syncVerticalMove);
		}
		//If is reading the stream, set the variables used in interpolating the position, rotation and acceleration
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

			//Set the acceleration
			verticalMove = syncVerticalMove;
		}
	}
}

