using UnityEngine;
using System.Collections;

public class HealthController : MonoBehaviour {

	public Transform healthIndicator;
	public int maxHealth = 100;
	[Range(0, 100)]
	public int currentHealthPercent;

	private int currentHealth;

	public bool isDead{get; set;}


	void Awake(){
		currentHealth = (currentHealthPercent * maxHealth)/100;
		healthIndicator.localScale = new Vector3(currentHealthPercent/100f, 1f, 1f);
		isDead = false;
	}

	[RPC]
	public void ChangeHealth(int amount){
		currentHealth += amount;

		if(currentHealth > maxHealth){
			currentHealth = maxHealth;
		}
		else if (currentHealth <= 0){
			currentHealth = 0;
			isDead = true;
		}

		currentHealthPercent = (currentHealth * 100)/maxHealth;

		healthIndicator.localScale = new Vector3(currentHealthPercent/100f, 1, 1);

		if(isDead){
			DestroyPlayer();
		}
	}

	void DestroyPlayer(){
		if(Network.isServer){
			Network.RemoveRPCs(networkView.owner);
			Network.Destroy(this.gameObject);
		}
	}

}
