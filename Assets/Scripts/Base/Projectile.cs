using UnityEngine;
using System.Collections;

/// <summary>
/// Projectile. Class used for projectile prefabs.
/// </summary>
public class Projectile : MonoBehaviour {

	[HideInInspector]
	public Rigidbody2D controlRigidbody;
	public int damage;
	public float speed;
	public float angle;

	void Awake(){
		controlRigidbody = GetComponent<Rigidbody2D>();
		ProjectileAwake();
	}

	public virtual void ProjectileAwake(){}

	/// <summary>
	/// Setups the projectile. Used every time the projectile is fired.
	/// </summary>
	/// <param name="position">Position.</param>
	/// <param name="damage">Damage.</param>
	/// <param name="speed">Speed.</param>
	/// <param name="angle">Angle, zero is facing right.</param>
	/// <param name="layer">Layer.</param>
	public virtual void SetupProjectile(Vector3 position, int damage, float speed, float angle, int layer, params float[] extraParameters){
		this.damage = damage;
		this.speed = speed;
		this.angle = angle;
		gameObject.layer = layer;

		this.transform.position = position;
		Vector3 rotation = this.transform.eulerAngles;
		rotation.z = angle*Mathf.Rad2Deg;
		this.transform.eulerAngles = rotation;

		controlRigidbody.velocity = new Vector2(Mathf.Cos(angle)*speed,Mathf.Sin(angle)*speed);
	}

	/// <summary>
	/// Raises the trigger enter2d event.
	/// Handles the projectile collisions, dealing damage or returning the projectile to the pool.
	/// </summary>
	/// <param name="other">Other.</param>
	void OnTriggerEnter2D(Collider2D other){
		// Checks for collision with the offscreen limit, returns the projectile to the pool.
		if( other.gameObject.layer == GameSystem.offscreenLimitLayer){
			gameObject.SetActive(false);
		}
		// As a Enemy Projectile checks for collision with a Player and deals damage.
		if( other.gameObject.layer == GameSystem.playerLayer && gameObject.layer == GameSystem.enemyProjectileLayer){
			gameObject.SetActive(false);
			other.GetComponent<Player>().TakeDamage(damage);
		}
		// As a Player Projectile checks for collision with a Enemy and deals damage.
		if( other.gameObject.layer == GameSystem.enemyLayer && gameObject.layer == GameSystem.playerProjectileLayer){
			gameObject.SetActive(false);
			other.GetComponent<Enemy>().TakeDamage(damage);
		}
	}
}
