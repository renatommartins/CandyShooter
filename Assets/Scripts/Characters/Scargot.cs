using UnityEngine;
using System.Collections;

public class Scargot : Enemy {

	public float ySpeed;
	Vector2 position;

	public Transform meshTransform;
	public Transform FireTransform;
	public float angularSpeed;
	Vector3 currentAngle;
	float projectileAngle;

	public override void EnemyAwake (){}
	public override void EnemyFixedUpdate (){}
	public override void EnemyOnEnable (){}
	public override void EnemyOnDisable (){}
	public override void EnemyOnTriggerEnter2D (Collider2D other){}
	public override void EnemyReset (){
		currentAngle = Vector3.zero;
		projectileAngle = 0;
		position = initialPosition;
	}
	public override void EnemySetup (){
		if(moveMultiplier < 0){	currentAngle.y = 180;	}
		else{	currentAngle.y = 0;	}

		position = transform.position;
	}
	public override void EnemyStart (){}
	public override void EnemyUpdate (){
		projectileAngle += angularSpeed*Time.deltaTime*moveMultiplier;
		if(projectileAngle >= 360){	projectileAngle -= 360;	}

		currentAngle.z += angularSpeed*Time.deltaTime*Mathf.Abs(moveMultiplier);
		if(currentAngle.z >= 360){	currentAngle.z -= 360;	}
		meshTransform.eulerAngles = currentAngle;

		if(projectileCounter > 0){	projectileCounter -= Time.deltaTime;	}
		else {	projectileCounter = 0;	}
		
		if( projectileCounter == 0){
			Fire(projectileDamage, projectileSpeed);
			projectileCounter = 1/rateOfFirePerSec;
		}
	}

	public void Fire(int damage, float speed){
		Projectile newProjectile = RequestProjectile(0);
		newProjectile.SetupProjectile(FireTransform.position,damage,speed,(projectileAngle-90)*Mathf.Deg2Rad,GameSystem.enemyProjectileLayer);
	}

	public override void Move (Vector2 input, float multiplier){
		position.y -= ySpeed*Time.fixedDeltaTime*Mathf.Abs(multiplier);
		controlRigidbody.MovePosition(position);
	}
}
