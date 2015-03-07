using UnityEngine;
using System.Collections;

public class CreateGameButton : MonoBehaviour {

	public void StartServer () {
		GameObject.Find("NetworkLevelLoader").GetComponent<AudioSource>().Play();
		GameObject.Find("NetworkManager").GetComponent<NetworkManager>().StartServer();
	}
}