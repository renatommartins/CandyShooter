using UnityEngine;
using System.Collections;

public class CookieSword : MonoBehaviour {

	public Player parentScript;

	void OnTriggerEnter2D(Collider2D Other){
		if(Other.gameObject.layer == GameSystem.enemyLayer && gameObject.activeSelf){
			Other.GetComponent<Enemy>().TakeDamage(parentScript.cookieDamage);
		}
	}
}
