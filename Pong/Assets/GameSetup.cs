using UnityEngine;
using System.Collections;

public class GameSetup : MonoBehaviour {

	public Camera mainCam;

	public BoxCollider2D topWall;
	public BoxCollider2D bottomWall;
	public BoxCollider2D leftWall;
	public BoxCollider2D rightWall;

	public Transform player1;
	public Transform player2;

	// Use this for initialization
	//void Start () {
	//
	//}
	
	// Update is called once per frame
	void Update () {
		//Move each wall to its edge location
		topWall.size = new Vector2 (mainCam.ScreenToWorldPoint (new Vector3 (2f * Screen.width, 0, 0)).x, 1f);
		topWall.center = new Vector2 (0, mainCam.ScreenToWorldPoint (new Vector3 (0, Screen.height, 0)).y + 0.5f);

		bottomWall.size = new Vector2 (mainCam.ScreenToWorldPoint (new Vector3 (2f * Screen.width, 0, 0)).x, 1f);
		bottomWall.center = new Vector2 (0, mainCam.ScreenToWorldPoint (new Vector3 (0, 0, 0)).y - 0.5f);

		leftWall.size = new Vector2 (1f, mainCam.ScreenToWorldPoint (new Vector3 (0, 2f * Screen.height, 0)).y);
		leftWall.center = new Vector2 (mainCam.ScreenToWorldPoint (new Vector3 (0, 0, 0)).x - 0.5f, 0);

		rightWall.size = new Vector2 (1f, mainCam.ScreenToWorldPoint (new Vector3 (0, 2f * Screen.height, 0)).y);
		rightWall.center = new Vector2 (mainCam.ScreenToWorldPoint (new Vector3 (Screen.width, 0, 0)).x + 0.5f, 0);

		player1.position = new Vector2 (mainCam.ScreenToWorldPoint (new Vector3 (75f, 0, 0)).x, player1.position.y);
		player2.position = new Vector2 (mainCam.ScreenToWorldPoint (new Vector3 (Screen.width - 75f, 0, 0)).x, player2.position.y);
	}
}
