using UnityEngine;
using System.Collections;

public class EnemyWeaponController : MonoBehaviour {

	public GameObject shot;
	public Transform shotSpawn;
	public float delay;
	public float fireRate;

	void Start () {
		InvokeRepeating("EnemyFire",delay,fireRate);
	}

	void EnemyFire(){
		Instantiate(shot,shotSpawn.position,shotSpawn.rotation);
		audio.Play();
	}
}
