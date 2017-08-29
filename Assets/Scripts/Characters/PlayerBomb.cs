using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class PlayerBomb : Projectile {

	Vector2 screenCenter;
	bool exploding = false;
	float explosionSpeed;
	float currentRadius;
	Vector3 originalScale;

	public List<Enemy> alreadyHit = new List<Enemy>();

	public override void ProjectileAwake (){
		originalScale = transform.localScale;
	}

	// Update is called once per frame
	void Update () {
		if(currentRadius > 10){
			currentRadius = 0;
			transform.localScale = originalScale;
			exploding = false;
			gameObject.SetActive(false);
		}
		if(!exploding){
			Vector2 newPosition = Vector2.MoveTowards(((Vector2)transform.position), screenCenter, speed*Time.deltaTime);
			controlRigidbody.MovePosition(newPosition);
			if((Vector2)transform.position == screenCenter){	exploding = true;	}
		}else{
			currentRadius += explosionSpeed*Time.deltaTime;
			transform.localScale = currentRadius * originalScale * 2;
			Transform[] caughtInExplosion = (FindObjectsOfType<Transform>().Where(Transform => Vector2.Distance(transform.position,Transform.position) <= currentRadius).ToArray());
			foreach(Transform enemyTransform in caughtInExplosion){
				if(enemyTransform.gameObject.layer == GameSystem.enemyLayer){
					Enemy enemy = enemyTransform.gameObject.GetComponent<Enemy>();
					if(!alreadyHit.Contains(enemy)){
						enemy.TakeDamage(damage);
						alreadyHit.Add(enemy);
					}
				}
				if(enemyTransform.gameObject.layer == GameSystem.enemyProjectileLayer){
					enemyTransform.gameObject.SetActive(false);
				}
			}
		}
	}

	public override void SetupProjectile (Vector3 position, int damage, float speed, float angle, int layer, params float[] extraParameters){
		alreadyHit.Clear();

		this.damage = damage;
		this.speed = speed;
		this.angle = angle;
		gameObject.layer = 0;

		explosionSpeed = extraParameters[0];

		screenCenter = GameSystem.GetPositionToCamera(new Vector2(0.5f,0.5f));

		currentRadius = 0;
		transform.localScale = originalScale;
		exploding = false;
		
		this.transform.position = position;
		//Vector3 rotation = this.transform.eulerAngles;
		//rotation.z = angle*Mathf.Rad2Deg;
		//this.transform.eulerAngles = rotation;
		
		//controlRigidbody.velocity = new Vector2(Mathf.Cos(angle)*speed,Mathf.Sin(angle)*speed);
	}

	void OnTriggerEnter2D(){}
}
