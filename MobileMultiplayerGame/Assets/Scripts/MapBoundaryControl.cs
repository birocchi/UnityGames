using UnityEngine;
using System.Collections;

public class MapBoundaryControl : MonoBehaviour {

	private float mapHeight;
	private float mapWidth;

	void Awake(){
		BoxCollider2D boundingBox = (BoxCollider2D) collider2D;
		mapHeight = boundingBox.size.y;
		mapWidth = boundingBox.size.x;
	}

	void OnTriggerExit2D(Collider2D other) {
		Vector3 position = other.transform.position;

		if(position.x > mapWidth/2){
			other.transform.position += Vector3.left * mapWidth;
		}

		if(position.x < -mapWidth/2){
			other.transform.position += Vector3.right * mapWidth;
		}

		if(position.y > mapHeight/2){
			other.transform.position += Vector3.down * mapHeight;
		}
		
		if(position.y < -mapHeight/2){
			other.transform.position += Vector3.up * mapHeight;
		}
	}
}
