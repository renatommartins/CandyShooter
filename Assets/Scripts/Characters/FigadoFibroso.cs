using UnityEngine;
using System.Collections;

public class FigadoFibroso : Enemy {

	Vector2 targetPosition, nextMove;
	public Transform fireTransform;
	public float speed;
	public float trackingTime;
	public float turningSpeed;

	public override void EnemyAwake (){}
	public override void EnemyFixedUpdate (){}
	public override void EnemyOnEnable (){}
	public override void EnemyOnDisable (){}
	public override void EnemyOnTriggerEnter2D (Collider2D other){}
	public override void EnemyReset (){
	}
	public override void EnemySetup (){
		if(moveMultiplier > 0){	targetPosition = GameSystem.GetPositionToCamera(new Vector2(0.9f,0.3f));	}
		else{	targetPosition = GameSystem.GetPositionToCamera(new Vector2(0.1f,0.3f));	}
		nextMove = Vector2.MoveTowards(((Vector2)transform.position),targetPosition,speed*Mathf.Abs(moveMultiplier));
	}
	public override void EnemyStart (){}
	public override void EnemyUpdate (){
		nextMove = Vector2.MoveTowards(((Vector2)transform.position),targetPosition,speed*Mathf.Abs(moveMultiplier));

		if(projectileCounter > 0){	projectileCounter -= Time.deltaTime;	}
		else {	projectileCounter = 0;	}
		
		if( projectileCounter == 0){
			Fire(projectileDamage, projectileSpeed);
			projectileCounter = 1/rateOfFirePerSec;
		}
	}
	public void Fire(int damage, float speed){
		Projectile newProjectile = RequestProjectile(0);
		newProjectile.GetComponent<Projectile>().SetupProjectile(fireTransform.position,damage,speed,0,GameSystem.enemyProjectileLayer, trackingTime, turningSpeed);
	}
	public override void Move (Vector2 input, float multiplier){
		controlRigidbody.MovePosition(nextMove);
	}

	void OnTriggerEnter2D(Collider2D collision){		
		if( collision.gameObject.layer == GameSystem.playerLayer && offscreenCollider.IsTouching(collision) ){
			if(touchDamage > 0){	collision.gameObject.GetComponent<Player>().TakeDamage(touchDamage);	}
		}
		EnemyOnTriggerEnter2D (collision);
	}
}
