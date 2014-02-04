using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public static int playerScore01 = 0;
	public static int playerScore02 = 0;

	public GUISkin theSkin;

	public static void Score (string WallName) {
		if (WallName == "rightWall") {
			playerScore01 += 1;
		}
		else {
			playerScore02 += 1;
		}
		Debug.Log ("Player 1 score is " + playerScore01);
		Debug.Log ("Player 2 score is " + playerScore02);
	}

	void OnGUI (){
		GUI.skin = theSkin;
		GUI.Label (new Rect (Screen.width / 2 - 150, 20, 100, 100), "" + playerScore01);
		GUI.Label (new Rect (Screen.width / 2 + 150, 20, 100, 100), "" + playerScore02);
	}
}
