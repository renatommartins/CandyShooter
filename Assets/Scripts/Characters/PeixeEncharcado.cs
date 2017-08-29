using UnityEngine;
using System.Collections;

/// <summary>
/// Peixe encharcado.
/// Aims at the closest Player and fires three Projectiles.
/// </summary>
public class PeixeEncharcado : Enemy {
	
	public float ySpeed;
	public Transform lookAt;
	public float maxRotationPerSec;
	Vector2 position;

	/// <summary> (Callback) Substitute of Awake for deriving from Enemy. DO NOT CALL!!! </summary>
	public override void EnemyAwake (){}
	/// <summary> (Callback) Substitute of Start for deriving from Enemy. DO NOT CALL!!! </summary>
	public override void EnemyStart (){}
	public override void EnemySetup (){	position = transform.position;	}
	/// <summary> (Callback) Resets the Enemy to the original state. DO NOT CALL!!! </summary>
	public override void EnemyReset (){	position = initialPosition;	}
	public override void EnemyOnEnable (){}
	/// <summary> (Callback) Substitute of OnDisable for deriving from Enemy. DO NOT CALL!!! </summary>
	public override void EnemyOnDisable(){}
	/// <summary> (Callback) Substitute of FixedUpdate for deriving from Enemy. DO NOT CALL!!! </summary>
	public override void EnemyFixedUpdate (){}
	/// <summary> (Callback) Substitute of Update for deriving from Enemy. DO NOT CALL!!! </summary>
	public override void EnemyUpdate (){
		transform.eulerAngles = new Vector3(0,0,Mathf.Rad2Deg*GetAngle(GetClosestPlayer().transform));

		if(projectileCounter > 0){	projectileCounter -= Time.deltaTime;	}
		else {	projectileCounter = 0;	}
		
		if( projectileCounter == 0){
			Fire(projectileDamage, projectileSpeed, Mathf.PI/4);
			projectileCounter = 1/rateOfFirePerSec;
		}
	}
	/// <summary>
	/// Fires a Projectile at the specified damage, speed and angle.
	/// </summary>
	/// <param name="damage">Damage.</param>
	/// <param name="speed">Speed.</param>
	/// <param name="angle">Angle, zero is facing right.</param>
	public void Fire(int damage, float speed, float angle){
		for(int i=0; i<	3; i++){
			Projectile newProjectile = RequestProjectile(0);
			newProjectile.transform.position = transform.position;
			newProjectile.GetComponent<Projectile>().SetupProjectile(transform.position, damage, speed, angle*(i-1)+(Mathf.Deg2Rad*transform.eulerAngles.z), 13);
		}
	}
	/// <summary>
	/// (Callback)
	/// Moves this Enemy.
	/// input is ignored. multiplier alters the speed it goes down the screen.
	/// </summary>
	/// <param name="input">Input. IGNORED!!!</param>
	/// <param name="multiplier">Multiplier.</param>
	public override void Move (Vector2 input, float multiplier){
		position.y -= ySpeed*Time.fixedDeltaTime*multiplier;
		controlRigidbody.MovePosition(position);
	}
	public override void EnemyOnTriggerEnter2D (Collider2D other){}
}
