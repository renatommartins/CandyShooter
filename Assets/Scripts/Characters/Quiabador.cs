using UnityEngine;
using System.Collections;

/// <summary>
/// Quiabador.
/// Moves down the screen in a zigzag and shoots eight projectiles in a circle around itself.
/// </summary>
public class Quiabador : Enemy {

	public AnimationCurve movement;
	public float radius;
	public float ySpeed;
	public float loopMultiplier;
	Vector2 position;
	float positionCounter;

	/// <summary> (Callback) Substitute of Awake for deriving from Enemy. DO NOT CALL!!! </summary>
	public override void EnemyAwake (){}
	/// <summary> (Callback) Substitute of Start for deriving from Enemy. DO NOT CALL!!! </summary>
	public override void EnemyStart (){	position = transform.position;	}
	public override void EnemySetup (){}
	/// <summary> (Callback) Resets the Enemy to the original state. DO NOT CALL!!! </summary>
	public override void EnemyReset ()	{
		position = initialPosition;
		positionCounter = 0;
	}
	public override void EnemyOnEnable (){}
	/// <summary> (Callback) Substitute of OnDisable for deriving from Enemy. DO NOT CALL!!! </summary>
	public override void EnemyOnDisable(){}
	/// <summary> (Callback) Substitute of FixedUpdate for deriving from Enemy. DO NOT CALL!!! </summary>
	public override void EnemyFixedUpdate (){}
	/// <summary> (Callback) Substitute of Update for deriving from Enemy. DO NOT CALL!!! </summary>
	public override void EnemyUpdate (){
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
		for(int i=1; i<=8; i++){
			Projectile newProjectile = RequestProjectile(0);
			newProjectile.transform.position = transform.position;
			newProjectile.GetComponent<Projectile>().SetupProjectile(transform.position, damage, speed, angle*i, 13);
		}
	}
	/// <summary>
	/// (Callback)
	/// Moves the character in a zigzag, input is ignored.
	/// multiplier makes the movement side to side faster. DO NOT CALL!!!
	/// </summary>
	/// <param name="input">Input. IGNORED!!! </param>
	/// <param name="multiplier">Multiplier.</param>
	public override void Move (Vector2 input, float multiplier){
		controlRigidbody.MovePosition(new Vector2(radius*movement.Evaluate(positionCounter)*Mathf.Sign(multiplier)+position.x,position.y));
		positionCounter += loopMultiplier*Time.fixedDeltaTime*Mathf.Abs(multiplier);
		if(positionCounter > 1){
			positionCounter = 0;
		}
		position.y -= ySpeed*Time.fixedDeltaTime;
	}

	public override void EnemyOnTriggerEnter2D (Collider2D other){}
}
