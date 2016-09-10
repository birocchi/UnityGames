using UnityEngine;
using System.Collections;

public class GatlingGunController : MonoBehaviour {

	public float gunForce = 1000;
	private ParticleSystem gatlingGun;
	private ParticleSystem muzzleFlash;
	private AudioSource shotSound;
	private Light shotLight;
	private float timeElapsed = 0;

	// Use this for initialization
	void Start () {
		shotSound = GetComponent<AudioSource>();
		gatlingGun = GetComponent<ParticleSystem>();
		shotLight = GetComponent<Light>();
		muzzleFlash = gatlingGun.transform.FindChild("MuzzleFlash").GetComponent<ParticleSystem>();
		muzzleFlash.emissionRate = gatlingGun.emissionRate;
	}
	
	// Update is called once per frame
	void Update () {

		timeElapsed += Time.deltaTime;


		if (Input.GetButton ("Fire1") ) {

			//Habilita a arma e o flash da arma
			gatlingGun.enableEmission = true;
			muzzleFlash.enableEmission = true;

			//Toca o som de acordo com a frequencia da arma
			if(timeElapsed >= 1/gatlingGun.emissionRate){
				AudioSource.PlayClipAtPoint(shotSound.clip, transform.position, shotSound.volume);
				shotLight.enabled = true;
				timeElapsed = 0;
			}
			
		} else {
			//Desliga a arma
			gatlingGun.enableEmission = false;
			muzzleFlash.enableEmission = false;
			
		}

		//Se atirou com sucesso, desliga a luz
		if(timeElapsed != 0){
			shotLight.enabled = false;
		}
	}

	void OnParticleCollision(GameObject other) {

		Rigidbody body = other.GetComponent<Rigidbody>();

		//Ao colidir com algo, aplica uma força no objeto
		if (body) {
			Vector3 direction = other.transform.position - transform.position;
			direction = direction.normalized;
			body.AddForce(direction * gunForce);

		}
	}
}
