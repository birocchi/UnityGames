using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour {

	//Holds a reference to itself, to avoid duplications
	private static NetworkManager instance;

#region Variables
	public GameObject[] playerPrefabs;
	public int numberOfClients = 3;
	public string gameTypeName = "SENAC Network Test Project";
	public int gamePort = 25000;

	private HostData[] hostData;
	private LobbyUIManager uiManager;
#endregion

#region Public Methods
	//Starts a new server
	public void StartServer () {
		NetworkConnectionError netError = Network.InitializeServer(Mathf.Abs(numberOfClients), gamePort, false);

		if(netError == NetworkConnectionError.NoError){
			Debug.Log("Server initialized in port: " + gamePort);
			return;
		}
		else if(netError == NetworkConnectionError.CreateSocketOrThreadFailure){
			gamePort++;
			Debug.Log("Port already in use! Trying the port: " + gamePort);
			StartServer();
		}
	}

	//Join a game using the host data
	public void ConnectToHost(HostData host){
		if(host != null){
			Network.Connect(host);
		}
	}
#endregion

#region Private Methods
	//Called when the object is created
	void Awake(){
		//if we don't have an [instance] set yet
		if(!instance) instance = this ;
		//otherwise, if we do, kill this thing
		else Destroy(this.gameObject) ;

		DontDestroyOnLoad(this);
	}

	//Spawns the network player
	private void SpawnPlayer(){
		Debug.Log("Spawning Player...");
		GameObject instance = (GameObject) Network.Instantiate(playerPrefabs[Random.Range(0,playerPrefabs.Length)], Vector3.zero, Quaternion.identity, 0);
		Camera.main.GetComponent<CameraFollow>().followedObject = instance.transform;
	}
#endregion

#region Network Callbacks
	//Called when connected to a server
	void OnConnectedToServer() {
		Debug.Log("Connected to the server!");
	}

	//Called when a server is initialized
	void OnServerInitialized(){
		Debug.Log("Server initialized, now registering...");
		uiManager = GameObject.Find("UIManager").GetComponent<LobbyUIManager>();
		MasterServer.RegisterHost(gameTypeName, uiManager.GetGameName(), "Network Game Room");
	}

	void OnPlayerDisconnected(NetworkPlayer netPlayer){
		Debug.Log("Player disconnected from: " + netPlayer.ipAddress + ":" + netPlayer.port);
		Network.RemoveRPCs(netPlayer);
		Network.DestroyPlayerObjects(netPlayer);
	}

	void OnNetworkLoadedLevel(){
		SpawnPlayer();
	}
#endregion

#region Unity Callbacks
	void OnApplicationQuit(){
		if(Network.isServer){
			Network.Disconnect(200);
			MasterServer.UnregisterHost();
		}

		if(Network.isClient){
			Network.Disconnect(200);
		}
	}
#endregion
}
