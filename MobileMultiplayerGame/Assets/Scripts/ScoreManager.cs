using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreManager : MonoBehaviour {

	public Text[] playersTexts;
	private string[] playersNames = {"","","",""};
	private int[] playerScores = {0,0,0,0};
	private NetworkPlayer[] netPlayers = new NetworkPlayer[4];

	public int IncludeServerPlayer(string playerName, NetworkPlayer netPlayer){
		//Debug.Log("Including the player " + playerName + " to the score system...");
		
		for(int playerNumber = 1; playerNumber <= playersNames.Length; playerNumber++){
			if(playersNames[playerNumber-1] == ""){
				return playerNumber;
			}
		}
		return 0;
	}

	[RPC]
	public void IncludeNewPlayerOnServer(string playerName, NetworkViewID playerViewID, NetworkMessageInfo info){
		//Debug.Log(string.Format("RPC -> Sender: {0}, NetView Owner: {1}, TimeStamp: {2}, ViewID: {3}",info.sender,info.networkView.owner,info.timestamp,info.networkView.viewID));
		//Debug.Log("Including the player " + playerName + " to the score system...");

		for(int playerNumber = 1; playerNumber <= playersNames.Length; playerNumber++){
			if(playersNames[playerNumber-1] == ""){
				NetworkView.Find(playerViewID).RPC ("OnReceivePlayerNumberFromServer", info.sender, playerNumber);
				return;
			}
		}
	}

	[RPC]
	public void IncludePlayer(string playerName, NetworkPlayer netPlayer, int playerNumber){
		//Debug.Log("Including the player " + playerName + " to the position " + playerNumber + " in the score system...");
		if(playerNumber >= 1 && playerNumber <= 4){
			playersNames[playerNumber - 1] = playerName;
			playersTexts[playerNumber - 1].text = playerName + ": 0";
			playerScores[playerNumber - 1] = 0;
			netPlayers[playerNumber - 1] = netPlayer;
			return;
		}
	}

	[RPC]
	public void ExcludePlayer(NetworkPlayer netPlayer){
		for(int playerNumber = 0; playerNumber < netPlayers.Length; playerNumber++){
			if(netPlayers[playerNumber] == netPlayer){
				playersNames[playerNumber] = "";
				playersTexts[playerNumber].text = "<No Player>";
				playerScores[playerNumber] = 0;
			}
		}
	}

	[RPC]
	public void AddScore(int value, int playerNumber){
		if(playerNumber >= 1 && playerNumber <= 4){
			playerScores[playerNumber - 1] += value;
			playersTexts[playerNumber -1].text = playersNames[playerNumber - 1] + ": " + playerScores[playerNumber - 1].ToString();
		}
	}

	public void ResetScore(int playerNumber){
		if(playerNumber >= 1 && playerNumber <= 4){
			playerScores[playerNumber - 1] = 0;
			playersTexts[playerNumber -1].text = playersNames[playerNumber - 1] + ": 0";
		}
	}

	public int GetPlayerScore(int playerNumber){
		if(playerNumber >= 1 && playerNumber <= 4){
			return playerScores[playerNumber -1];
		}
		else{
			return -1;
		}
	}

	public string GetPlayerName(int playerNumber){
		if(playerNumber >= 1 && playerNumber <= 4){
			return playersNames[playerNumber -1];
		}
		else{
			return string.Empty;
		}
	}

	public int GetPlayerNumber(string playerName){
		for(int playerNumber = 1; playerNumber <= playersNames.Length; playerNumber++){
			if(playersNames[playerNumber - 1] == playerName){
				return playerNumber;
			}
		}
		return 0;
	}
}
