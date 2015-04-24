using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LobbyUIManager : MonoBehaviour {

	public GameObject hostListItemPrefab;

	private GameObject hostListPanel;
	private GameObject controlsPanel;
	private InputField gameNameInputField;
	private InputField playerNameInputField;

	void Awake(){
		hostListPanel = GameObject.Find("HostListPanel");
		controlsPanel = GameObject.Find("ControlsPanel");
		gameNameInputField = GameObject.Find("GameNameInput").GetComponent<InputField>();
		playerNameInputField = GameObject.Find("PlayerNameInput").GetComponent<InputField>();
	}

	public string GetGameName(){
		if(gameNameInputField != null && gameNameInputField.text != string.Empty) 
			return gameNameInputField.text;
		else
			return "NoGameName";
	}

	public string GetPlayerName(){
		if(playerNameInputField != null && playerNameInputField.text != string.Empty) 
			return playerNameInputField.text;
		else
			return "NoPlayerName";
	}

	//Shows the list of available hosts
	public void ShowHostList(string gameTypeName){
		GetComponent<AudioSource>().Play();

		//Shows the host list panel
		hostListPanel.GetComponent<Image>().enabled = true;
		
		//Clear the old host list items
		MasterServer.ClearHostList();
		GameObject[] hostListItems = GameObject.FindGameObjectsWithTag("HostListItem");
		if(hostListItems != null && hostListItems.Length != 0){
			foreach(GameObject item in hostListItems){
				Destroy(item.gameObject);
			}
		}
		
		//Request an updated host list to the Master Server
		MasterServer.RequestHostList(gameTypeName);
	}

	//Fills the host list with a given hostdata array
	public void FillHostList(HostData[] hostData){
		for(int i = 0; i < hostData.Length; i++){
			GameObject instance = (GameObject) Instantiate(hostListItemPrefab);
			instance.transform.SetParent(hostListPanel.transform, false);
			instance.GetComponent<HostListItem>().hostData = hostData[i];
			instance.GetComponent<HostListItem>().SetVisualObjects();
			instance.transform.position = new Vector2(instance.transform.position.x, instance.transform.position.y - i * 30);
		}
	}

	//Disable the Lobby UI
	public void HideLobbyUI(){
		controlsPanel.SetActive(false);
		hostListPanel.SetActive(false);
	}

	//Called on any event received from the master server
	void OnMasterServerEvent(MasterServerEvent msEvent) {
		HostData[] hostData;

		if (msEvent == MasterServerEvent.RegistrationSucceeded){
			HideLobbyUI();
		}
		
		if (msEvent == MasterServerEvent.HostListReceived){
			hostData = MasterServer.PollHostList();
			//Debug.Log("Host list received: " + hostData.Length + " Host(s) found.");
			
			if(hostData != null && hostData.Length > 0){
				FillHostList(hostData);
			}
		}
	}	
}
