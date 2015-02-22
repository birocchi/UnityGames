using UnityEngine;
using System.Collections;

[RequireComponent (typeof (NetworkView))]
public class NetworkLevelLoader : MonoBehaviour {

	//Holds a reference to itself, to avoid duplications
	private static NetworkLevelLoader instance;

	public string[] supportedNetworkLevels = {"Stage1"};
	public string disconnectedLevel = "MainLobby";

	private int lastLevelPrefix = 0;
	
	void Awake ()
	{
		//if we don't have an [instance] set yet
		if(!instance) instance = this ;
		//otherwise, if we do, kill this thing
		else Destroy(this.gameObject) ;

		// Network level loading is done in a separate channel.
		DontDestroyOnLoad(this);
		networkView.group = 1;
	}

	void OnServerInitialized(){
		Network.RemoveRPCsInGroup(0);
		Network.RemoveRPCsInGroup(1);
		networkView.RPC( "LoadLevel", RPCMode.AllBuffered, supportedNetworkLevels[0], lastLevelPrefix + 1);
	}

	//RPC that tells everyone to load the specified level
	[RPC]
	void LoadLevel (string level, int levelPrefix){
		lastLevelPrefix = levelPrefix;
		
		// There is no reason to send any more data over the network on the default channel,
		// because we are about to load the level, thus all those objects will get deleted anyway
		Network.SetSendingEnabled(0, false);    
		
		// We need to stop receiving because first the level must be loaded first.
		// Once the level is loaded, rpc's and other state update attached to objects in the level are allowed to fire
		Network.isMessageQueueRunning = false;
		
		// All network views loaded from a level will get a prefix into their NetworkViewID.
		// This will prevent old updates from clients leaking into a newly created scene.
		Network.SetLevelPrefix(levelPrefix);
		Debug.Log("Loading a Network Level...");
		Application.LoadLevel(level);
	}

	//Called after the level is completely loaded
	void OnLevelWasLoaded(int level) {
		//Send the message only if it is not the main lobby
		if(level != 0){
			Debug.Log("Network Level loaded!");
			
			// Allow receiving data again
			Network.isMessageQueueRunning = true;
			// Now the level has been loaded and we can start sending out data to clients
			Network.SetSendingEnabled(0, true);

			Debug.Log("Sending message to other game objects...");

			//Tell the other GameObjects that the level finished loading
			foreach (GameObject go in FindObjectsOfType<GameObject>())
				go.SendMessage("OnNetworkLoadedLevel", SendMessageOptions.DontRequireReceiver); 
		}
	}

	void OnDisconnectedFromServer (){
		Application.LoadLevel(disconnectedLevel);
	}

}
