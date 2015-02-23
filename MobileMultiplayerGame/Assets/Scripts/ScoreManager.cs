using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreManager : MonoBehaviour {

	public Text[] playersTexts;
	public string[] playersNames;
	private int[] playerScores;

	// Use this for initialization
	void Start () {
		playerScores = new int[] {0,0,0,0};
		for(int i = 0; i < playersTexts.Length; i++){
			playersTexts[i].text = playersNames[i] + ": " + playerScores[i].ToString();
		}
	}

	public void AddScore(int value, int playerNumber){
		if(playerNumber >= 1 && playerNumber <= 4){
			playerScores[playerNumber - 1] += value;
			playersTexts[playerNumber -1].text = playerScores[playerNumber - 1].ToString();
		}
	}

	public void ResetScore(int playerNumber){
		if(playerNumber >= 1 && playerNumber <= 4){
			playerScores[playerNumber - 1] = 0;
			playersTexts[playerNumber -1].text = playerScores[playerNumber - 1].ToString();
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
}
