using UnityEngine;
using System.Collections;

public class AsteroidController : MonoBehaviour {

	//Movement synchronization
	private float lastSynchronizationTime;
	private float syncDelay = 0f;
	private float syncTime = 0f;
	private Vector3 syncStartPosition = Vector3.zero;
	private Vector3 syncEndPosition = Vector3.zero;

	void Awake () {
		lastSynchronizationTime = Time.time;
	}

	void Update(){
		if(!GetComponent<NetworkView>().isMine){
			syncTime += Time.deltaTime;
			transform.position = Vector3.Lerp(syncStartPosition, syncEndPosition, syncTime / syncDelay);
		}
	}

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info){

		Vector3 syncPosition = Vector3.zero;
		
		//If is writing on the stream, sends the position;
		if(stream.isWriting){
			syncPosition = transform.position;
			stream.Serialize(ref syncPosition);
		}
		//If is reading the stream, set the variables used in interpolating the position
		else if(stream.isReading){
			stream.Serialize(ref syncPosition);
			
			//Reset the timer and calculate the delay between the network sent packages
			syncTime = 0f;
			syncDelay = Time.time - lastSynchronizationTime;
			lastSynchronizationTime = Time.time;
			
			//Set the initial position and the destination
			syncStartPosition = transform.position;
			syncEndPosition = syncPosition;
		}
	}
}
