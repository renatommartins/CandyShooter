using UnityEngine;
using System.Collections;

public class AvoDoCarlosSpoon : MonoBehaviour {

	public AvoDoCarlos parentScript;

	void OnTriggerEnter2D(Collider2D Other){
		if(Other.gameObject.layer == GameSystem.playerLayer && gameObject.activeSelf){
			Other.GetComponent<Player>().TakeDamage(parentScript.spoonDamage);
		}
	}
}
