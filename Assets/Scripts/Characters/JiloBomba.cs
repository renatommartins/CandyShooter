using UnityEngine;
using System.Collections;

public class JiloBomba : Enemy {

	Vector2 position = new Vector2();

	[Space(10)]
	public GameObject blown;
	public GameObject notBlown;
	public float speed;
	Vector2 calculatedSpeed;



	public override void EnemyAwake (){}
	public override void EnemyStart (){	position = initialPosition;	}
	public override void EnemyUpdate (){}
	public override void EnemyFixedUpdate (){}
	public override void EnemyOnEnable (){}
	public override void EnemyOnDisable(){}
	public override void EnemySetup (){	SetDirection();	}
	public override void EnemyReset () {
		position = initialPosition;
		blown.SetActive(false);
		notBlown.SetActive(true);
	}
	public override void Move (Vector2 input, float multiplier){
		position.x += calculatedSpeed.x*Time.fixedDeltaTime;
		position.y += calculatedSpeed.y*Time.fixedDeltaTime;
		controlRigidbody.MovePosition(position);
	}

	void SetDirection(){
		if(Mathf.Sign(moveMultiplier) == -1){
			Vector2 revertPosition = transform.position;
			revertPosition.x *= -1;
			transform.position = revertPosition;
			position = transform.position;
		}else{
			position = transform.position;
		}
		float angle = GetAngle(GetClosestPlayer().transform);
		calculatedSpeed.x = Mathf.Cos(angle)*speed*Mathf.Abs(moveMultiplier);
		calculatedSpeed.y = Mathf.Sin(angle)*speed*Mathf.Abs(moveMultiplier);
	}

	public void Explode(){
		for(int i=0; i<12;i++){
			Projectile newProjectile = RequestProjectile(0);
			newProjectile.SetupProjectile(transform.position, projectileDamage, projectileSpeed,Mathf.PI/6+Mathf.PI/6*i,GameSystem.enemyProjectileLayer);
		}
		blown.SetActive(true);
		notBlown.SetActive(false);
	}

	public override void EnemyOnTriggerEnter2D(Collider2D other){
		if(other.gameObject.layer == GameSystem.playerLayer && !blown.activeSelf){
			Explode();
		}
	}
}
