using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public GameObject[] hazards;
	public Vector3 spawnValues;
	public int hazardCount;
	public float spawnWait;
	public float startWait;
	public float waveWait;

	public GUIText scoreText;
	public GUIText restartText;
	public GUIText gameOverText;

	private bool gameOver;
	private bool restart;
	private int score;

	void Start () {
		gameOver = false;
		restart = false;
		restartText.text = "";
		gameOverText.text = "";
		score = 0;
		UpdateScore();
		StartCoroutine(SpawnWaves());
	}

	void Update (){
		if(restart){
			if(Input.GetKeyDown(KeyCode.R) || Input.touchCount > 0){
				Application.LoadLevel(Application.loadedLevel);
			}
		}

		if(Input.GetKey(KeyCode.Escape)){
			Application.Quit();
		}
	}

	IEnumerator SpawnWaves(){
		yield return new WaitForSeconds(startWait);

		while(true){
			for(int i = 0; i < hazardCount; i++){
				GameObject hazard = hazards[Random.Range(0,hazards.Length)];
				Vector3 spawnPosition = new Vector3(Random.Range(-spawnValues.x,spawnValues.x), spawnValues.y, spawnValues.z);
				Quaternion spawnRotation = Quaternion.identity;
				Instantiate(hazard,spawnPosition,spawnRotation);
				yield return new WaitForSeconds(spawnWait);
			}
			yield return new WaitForSeconds(waveWait);

			if(gameOver){
				restartText.text = "Press 'R' or Touch Screen for Restart";
				restart = true;
				break;
			}
		}
	}

	public void AddScore(int newScoreValue){
		score += newScoreValue;
		UpdateScore();
	}

	void UpdateScore(){
		scoreText.text = "Score: " + score;
	}

	public void GameOver(){
		gameOverText.text = "CHUPA WELLINGTON!";
		gameOver = true;
	}
}
