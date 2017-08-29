using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AvoDoCarlos : Enemy {
	
	float arcTan;

	bool battleStarted = false;
	public Vector2 finalPosition;
	Vector2 targetPosition;
	Vector2 nextPosition;
	public float speed;

	public int spoonDamage;
	public float nextMoveCounter;
	float moveRepeat;

	public int fire1Damage;
	public int fire1Speed;
	public int fire1Quantity;
	public float ROFFire1;
	public bool alternateFire1 = false;
	float fire1Repeat;

	public int fire2Damage;
	public int fire2Speed;
	public float fire2Spread;
	public float ROFFire2;
	float fire2Repeat;

	Slider healthSlider;

	public override void EnemyAwake (){}
	public override void EnemyFixedUpdate (){}
	public override void EnemyOnEnable (){}
	public override void EnemyOnDisable (){}
	public override void EnemyOnTriggerEnter2D (Collider2D other){}
	public override void EnemyReset (){}
	public override void EnemySetup (){
		healthSlider = GameSystem.RequestBossSlider();
		healthSlider.value = 1;
		targetPosition = GameSystem.GetPositionToCamera(finalPosition);
		nextPosition = Vector2.MoveTowards(((Vector2)transform.position),targetPosition,speed*Mathf.Abs(moveMultiplier)*Time.deltaTime);
	}
	public override void EnemyStart (){}
	public override void EnemyUpdate (){
		nextPosition = Vector2.MoveTowards(((Vector2)transform.position),targetPosition,speed*Mathf.Abs(moveMultiplier)*Time.deltaTime);
		if(battleStarted){
			if(moveRepeat <= 0){
				float nextMove = Random.value;
				if(nextMove > 0.5f){	animator.SetTrigger("Sweep");	}
				else{	animator.SetTrigger("Stab");	}
				moveRepeat = nextMoveCounter;
			}
			moveRepeat -= Time.deltaTime;

			if(fire1Repeat <= 0){
				Fire1();
				fire1Repeat = 1/ROFFire1;
			}
			fire1Repeat -= Time.deltaTime;

			arcTan = GetAngle(GetClosestPlayer().transform);

			if(fire2Repeat <= 0){
				Fire2();
				fire2Repeat = 1/ROFFire2;
			}
			fire2Repeat -= Time.deltaTime;
		}
		else{
			hitpoints = originalValues.hitpoints;
			if((Vector2)transform.position == targetPosition){	battleStarted = true;	}
		}
	}
	public override void Move (Vector2 input, float multiplier){
		controlRigidbody.MovePosition(nextPosition);
	}

	void Fire1(){
		alternateFire1 = !alternateFire1;
		int fireQuantity = alternateFire1? fire1Quantity-1 : fire1Quantity;
		float angleStep = Mathf.PI/(fireQuantity);
		float angleOffset = fireQuantity%2 == 0? angleStep/2 : 0;
		int forStart = -Mathf.FloorToInt(fireQuantity/2);
		int forEnd = fireQuantity%2 == 0? Mathf.FloorToInt(fireQuantity/2)-1: Mathf.FloorToInt(fireQuantity/2);

		for(int i= forStart; i<=forEnd; i++){
			Projectile newProjectile = RequestProjectile(0);
			newProjectile.SetupProjectile(transform.position,fire1Damage,fire1Speed,(Mathf.PI/2*3)+(angleStep*i) + angleOffset ,GameSystem.enemyProjectileLayer);
		}
	}

	void Fire2(){
		for(int i= -1; i<=1; i++){
			Projectile newProjectile = RequestProjectile(1);
			newProjectile.SetupProjectile(transform.position,fire2Damage,fire2Speed,Mathf.Deg2Rad*(fire2Spread*i) + arcTan ,GameSystem.enemyProjectileLayer);
		}
	}

	public override void TakeDamage (int damage){
		hitpoints -= damage;
		healthSlider.value = ((float)hitpoints)/((float)originalValues.hitpoints);
		if(hitpoints <= 0){	StartDefeatedSequence();	}
	}

	void StartDefeatedSequence(){
		gameObject.SetActive(false);
		GameSystem.Instance.AddScore(defeatScore);
		healthSlider.gameObject.SetActive(false);
	}

	void OnTriggerEnter2D(Collider2D collision){
		if( collision.gameObject.layer == GameSystem.playerLayer && offscreenCollider.IsTouching(collision) ){
			if(touchDamage > 0){	collision.gameObject.GetComponent<Player>().TakeDamage(touchDamage);	}
		}
		EnemyOnTriggerEnter2D (collision);
	}
}
