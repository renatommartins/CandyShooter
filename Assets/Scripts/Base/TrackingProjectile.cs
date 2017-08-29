using UnityEngine;
using System.Collections;

public class TrackingProjectile : Projectile {

	public Transform lookAtTransform;
	public float trackingTime;
	public float maxTurnSpeed;
	float trackingRemaining;

	void Update(){
		trackingRemaining += Time.deltaTime;
		if(trackingRemaining < trackingTime){
			Player closestPlayer = null;
			float distance = Mathf.Infinity, newDistance = -1;
			foreach(Player player in GameSystem.Instance.players){
				newDistance = 	Vector3.Distance(transform.position,player.transform.position);
				if(newDistance < distance){	
					closestPlayer = player;
					distance = newDistance;
				}
			}
			lookAtTransform.LookAt(closestPlayer.transform);
			transform.right = Vector3.RotateTowards(transform.right,lookAtTransform.forward,maxTurnSpeed*Mathf.Deg2Rad*Time.deltaTime,1);
		}
		controlRigidbody.velocity = ((Vector2) transform.right) * speed;
	}

	public override void SetupProjectile(Vector3 position, int damage, float speed, float angle, int layer, params float[] extraParameters){
		this.damage = damage;
		this.speed = speed;
		this.angle = angle;
		gameObject.layer = layer;
		trackingTime = extraParameters[0];
		maxTurnSpeed = extraParameters[1];
		
		this.transform.position = position;
		Player closestPlayer = null;
		float distance = Mathf.Infinity, newDistance = -1;
		foreach(Player player in GameSystem.Instance.players){
			newDistance = 	Vector3.Distance(transform.position,player.transform.position);
			if(newDistance < distance){	
				closestPlayer = player;
				distance = newDistance;
			}
		}
		lookAtTransform.LookAt(closestPlayer.transform);
		transform.right = lookAtTransform.forward;
	}

	void OnDisable(){
		trackingRemaining = 0;
	}
}
