using UnityEngine;
using System.Collections;

public class AsteroidGenerator : MonoBehaviour {

	public GameObject[] hazards;
	public Vector2 spawnValues;
	public int hazardCount;

	private Transform generatedAsteroids;

	void Start () {
		generatedAsteroids = transform.FindChild("GeneratedAsteroids");

		if(Network.isServer){
			for(int i = 0; i < hazardCount; i++){
				GameObject hazard = hazards[Random.Range(0,hazards.Length)];
				
				Vector3 spawnPosition = new Vector3(Random.Range(-spawnValues.x,spawnValues.x), Random.Range(-spawnValues.y,spawnValues.y));
				Quaternion spawnRotation = Quaternion.identity;
				GameObject instance = (GameObject) Network.Instantiate(hazard,spawnPosition,spawnRotation, 0);
				GetComponent<NetworkView>().RPC("SetAsteroidAttribute",RPCMode.AllBuffered, instance.GetComponent<NetworkView>().viewID);
			}
		}
	}

	[RPC]
	void SetAsteroidAttribute(NetworkViewID networkID){
		GameObject instance = NetworkView.Find(networkID).gameObject;
		instance.transform.SetParent(generatedAsteroids);
		instance.GetComponent<Rigidbody2D>().velocity = new Vector3(Random.Range(-1f,1f),Random.Range(-1f,1f),Random.Range(-1f,1f));
		instance.GetComponent<Rigidbody2D>().AddTorque(Random.Range(-5f * instance.GetComponent<Rigidbody2D>().mass, 5f * instance.GetComponent<Rigidbody2D>().mass));
	}

}
