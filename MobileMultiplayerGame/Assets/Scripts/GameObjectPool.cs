using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameObjectPool {

	private List<GameObject> pool;
	public int PoolSize{ get{return pool.Count;} }

	public GameObjectPool() {
		pool = new List<GameObject>();
	}

	public void Add(GameObject obj){
		obj.SetActive(false);
		pool.Add(obj);
	}

	public GameObject GetFreeObject(){
		foreach(GameObject go in pool){
			if(!go.activeSelf){
				return go;
			}
		}
		return null;
	}


}
