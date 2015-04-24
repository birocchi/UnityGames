using UnityEngine;
using System.Collections;

public class ShotsPoolManager : MonoBehaviour {

	public int shotsPoolMaxSize = 20;
	public GameObject shotObject;
	private GameObjectPool shotsPool;

	void Awake(){
		shotsPool = new GameObjectPool();
	}

	void Start(){
		if(Network.isServer){
			for(int i = 0; i < shotsPoolMaxSize; i++){
				GameObject shot = (GameObject)Network.Instantiate(shotObject,Vector3.zero,Quaternion.identity,0);
				//Debug.Log(string.Format("Network Instantiation of the shot with {0}",shot.networkView.viewID, shot.networkView.group,shot.networkView.owner));
				//Debug.Log(string.Format("Calling RPC -AddShotToPool- using {0}, Group: {1}, Owner: {2} ", networkView.viewID, networkView.group, networkView.owner));
				GetComponent<NetworkView>().RPC("AddShotToPool", RPCMode.AllBuffered, shot.GetComponent<NetworkView>().viewID);
			}
		}
	}

	public GameObject GetFreeObject(){
		return shotsPool.GetFreeObject();
	}

	public GameObject GetObjectByID(NetworkViewID ID){
		return shotsPool.GetObjectByID(ID);
	}

	[RPC]
	public void AddShotToPool(NetworkViewID shotViewID){
		GameObject shot = NetworkView.Find(shotViewID).gameObject;
		if(shot != null){
			shot.transform.SetParent(this.transform);
			shot.GetComponent<NetworkView>().group = 2;
			shotsPool.Add(shot);
			//Debug.Log(string.Format("Added shot {0} to {1}'s pool! \n{2}, PoolSize: {3}", shot.networkView.viewID, gameObject.name, networkView.viewID, shotsPool.PoolSize));
		}
	}
}
