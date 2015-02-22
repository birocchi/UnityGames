using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HostListItem : MonoBehaviour {

	[SerializeField]
	public HostData hostData;

	public void JoinGame(){
		GameObject.Find("NetworkManager").GetComponent<NetworkManager>().ConnectToHost(hostData);
	}

	public void SetVisualObjects(){
		if(hostData != null){
			transform.FindChild("ServerName").GetComponent<Text>().text = hostData.gameName;
			transform.FindChild("PlayersLimit").GetComponent<Text>().text = "Players: " + hostData.connectedPlayers + "/" + hostData.playerLimit;
		}
	}
}
