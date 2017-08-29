using UnityEngine;
using System.Collections;

/// <summary>
/// Enemy. Base Class to derive all Enemy Characters in the game.
/// </summary>
public abstract class Enemy : Character, IDamageable {
	
	public int touchDamage;
	public int defeatScore;
	[HideInInspector]
	public Vector2 initialPosition;
	public Collider2D offscreenCollider;

	public OriginalValues originalValues = new OriginalValues();

	[System.Serializable]
	// Class for restoring the enemy initial values when pooling
	public class OriginalValues{
		public int hitpoints;
		public int projectileDamage;
		public float projectileSpeed;
		public float rateOfFirePerSec;
		public int touchDamage;
		public int defeatScore;
		public float moveMultiplier;
	}

	/// <summary>
	/// Substitute of Awake for deriving from Character.
	/// Sets up the enemy reset for reuse in pooling.
	/// </summary>
	public override void CharacterAwake(){
		initialPosition = transform.position;
		originalValues.hitpoints = hitpoints;
		originalValues.projectileDamage = projectileDamage;
		originalValues.projectileSpeed = projectileSpeed;
		originalValues.rateOfFirePerSec = rateOfFirePerSec;
		originalValues.touchDamage = touchDamage;
		originalValues.defeatScore = defeatScore;
		originalValues.moveMultiplier = moveMultiplier;

		EnemyAwake();
	}

	/// <summary> (Callback) Substitute of Start for deriving from Character. DO NOT CALL!!! </summary>
	public override void CharacterStart () {	EnemyStart();	}

	/// <summary>
	/// Resets the character to the prefab values.
	/// </summary>
	public void CharacterReset(){
		hitpoints = originalValues.hitpoints;
		projectileDamage = originalValues.projectileDamage;
		projectileSpeed = originalValues.projectileSpeed;
		rateOfFirePerSec = originalValues.rateOfFirePerSec;
		touchDamage = originalValues.touchDamage;
		defeatScore = originalValues.defeatScore;
		moveMultiplier = originalValues.moveMultiplier;

		transform.position = initialPosition;
		projectileCounter = 1/rateOfFirePerSec;

		EnemyReset();
	}

	/// <summary> (Callback) Substitute of FixedUpdate for deriving from Character. DO NOT CALL!!! </summary>
	public override void CharacterFixedUpdate (){	EnemyFixedUpdate();	}

	/// <summary> (Callback) Substitute of Awake for deriving from Character. DO NOT CALL!!! </summary>
	public override void CharacterUpdate (){	EnemyUpdate();	}

	/// <summary>
	/// Takes the damage. returns to the pool and adds score if defeated.
	/// </summary>
	/// <param name="damage">Damage.</param>
	public override void TakeDamage(int damage){
		hitpoints -= damage;
		if(hitpoints <= 0){
			gameObject.SetActive(false);
			GameSystem.Instance.AddScore(defeatScore);
		}
	}

	/// <summary>
	/// (Callback)
	/// Raises the collision enter2d event.
	/// Checks if the enemy has reached the offscreen limit or has touched the player.
	/// </summary>
	/// <param name="collision">Collision.</param>
	void OnTriggerEnter2D(Collider2D collision){
		if( collision.gameObject.layer == GameSystem.offscreenLimitLayer && offscreenCollider.IsTouching(collision) ){
			gameObject.SetActive(false);
		}

		if( collision.gameObject.layer == GameSystem.playerLayer && offscreenCollider.IsTouching(collision) ){
			if(touchDamage > 0){	collision.gameObject.GetComponent<Player>().TakeDamage(touchDamage);	}
		}
		EnemyOnTriggerEnter2D (collision);
	}
	/// <summary>
	/// Raises the disable event.
	/// </summary>
	void OnDisable(){	CharacterReset();	EnemyOnDisable();	}

	void OnEnable(){	EnemyOnEnable();	}

	public Player GetClosestPlayer(){
		Player closestPlayer = null;
		float distance = Mathf.Infinity, newDistance = -1;
		foreach(Player player in GameSystem.Instance.players){
			newDistance = 	Vector3.Distance(transform.position,player.transform.position);
			if(newDistance < distance){	
				closestPlayer = player;
				distance = newDistance;
			}
		}
		return closestPlayer;
	}

	public float GetAngle(Transform otherTransform){
		float tangent = (otherTransform.position.y-transform.position.y)/(otherTransform.position.x-transform.position.x);
		float arcTan = Mathf.Atan(tangent);
		if(otherTransform.position.x < transform.position.x){	arcTan += Mathf.PI;	}
		return arcTan;
	}

	/// <summary> (Callback) Substitute of Awake for deriving from Enemy. DO NOT CALL!!! </summary>
	public abstract void EnemyAwake();
	/// <summary> (Callback) Substitute of Start for deriving from Enemy. DO NOT CALL!!! </summary>
	public abstract void EnemyStart();
	/// <summary> (Callback) Substitute of FixedUpdate for deriving from Enemy. DO NOT CALL!!! </summary>
	public abstract void EnemyFixedUpdate();
	/// <summary> (Callback) Substitute of Update for deriving from Enemy. DO NOT CALL!!! </summary>
	public abstract void EnemyUpdate();
	/// <summary> (Callback) Resets the Enemy to the original state. DO NOT CALL!!! </summary>
	public abstract void EnemyReset();
	/// <summary> (Callback) Substitute of OnDisable for deriving from Enemy. DO NOT CALL!!! </summary>
	public abstract void EnemyOnEnable();
	/// <summary> (Callback) Substitute of OnDisable for deriving from Enemy. DO NOT CALL!!! </summary>
	public abstract void EnemyOnDisable();
	/// <summary> (Callback) Substitute of OnTriggerEnter2D for deriving from Enemy. DO NOT CALL!!! </summary>
	/// <param name="other">Other.</param>
	public abstract void EnemyOnTriggerEnter2D(Collider2D other);

	public abstract void EnemySetup();
}
