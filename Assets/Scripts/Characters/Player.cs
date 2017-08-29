using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Player. Class for Player Character.
/// </summary>
public class Player : Character, IDamageable {

	public Transform firingPoint;
	[Space(10)]
	public float speed;
	public PlayerMovement playerMovement;
	[Space(10)]
	public Slider hitpointsSlider;
	public AudioClip projectileAudio;
	public AudioSource playerAudioSource;
	[Space(10)]
	public int cookieDamage;
	public float explosionSpeed;
	public int bombDamage;

	/// <summary> Substitute of Awake for deriving from Character. DO NOT CALL!!! </summary>
	public override void CharacterAwake (){}

	/// <summary> Substitute of Start for deriving from Character. DO NOT CALL!!! </summary>
	public override void CharacterStart () {}

	/// <summary> Substitute of Awake for deriving from Character. DO NOT CALL!!! </summary>
	public override void CharacterUpdate(){
		if(projectileCounter > 0){	projectileCounter -= Time.deltaTime;	}
		else {	projectileCounter = 0;	}
		
		if(Input.GetButton("Fire1") && projectileCounter == 0){
			Fire(projectileDamage, projectileSpeed, Mathf.PI/2);
			projectileCounter = 1/rateOfFirePerSec;
		}
		if(Input.GetButtonDown("Fire2") && GameSystem.bombAvaliable/* && projectileCounter == 0*/){	FireBomb();	}

		if(Input.GetButtonDown("Fire3")){	animator.SetTrigger("Attack");	}
	}

	/// <summary> Substitute of FixedUpdate for deriving from Character. DO NOT CALL!!! </summary>
	public override void CharacterFixedUpdate(){
		float x = Input.GetAxis("Horizontal");
		float y = Input.GetAxis("Vertical");

		playerMovement.ApplyGravity ((x != 0),(y != 0));
		playerMovement.movement += new Vector2 (x, y);
	}

	/// <summary>
	/// Move the character with the given input.
	/// multiplier may be used in the logic.
	/// </summary>
	/// <param name="input">Input.</param>
	/// <param name="multiplier">Multiplier.</param>
	/// <param name="velocity">Velocity.</param>
	public override void Move (Vector2 velocity, float multiplier ){	controlRigidbody.velocity = playerMovement.movement*speed*multiplier;	}

	/// <summary>
	/// Fire a Projectile with the specified damage, speed and angle.
	/// </summary>
	/// <param name="damage">Damage.</param>
	/// <param name="speed">Speed.</param>
	/// <param name="angle">Angle.</param>
	public void Fire (int damage, float speed, float angle){
		Projectile newProjectile = RequestProjectile(0);
		newProjectile.transform.position = transform.position;
		newProjectile.SetupProjectile(firingPoint.position, damage, speed, angle, 12);
		playerAudioSource.PlayOneShot (projectileAudio);
	}

	public void FireBomb(){
		Projectile newProjectile = RequestProjectile(1);
		newProjectile.SetupProjectile(transform.position,bombDamage,projectileSpeed,0,GameSystem.screenLimitLayer,explosionSpeed);
		GameSystem.Instance.bombCounter = GameSystem.Instance.bombCooldown;
	}

	/// <summary>
	/// Takes the damage and updates the life bar.
	/// </summary>
	/// <param name="damage">Damage.</param>
	public override void TakeDamage(int damage){
		hitpoints -= damage;
		hitpointsSlider.value = hitpoints;
	}

	[System.Serializable]
	public class PlayerMovement{
		public float acceleration;
		public float gravity;
		Vector2 _movement;

		public Vector2 movement{
			get {	return _movement;	}
			set {
				_movement = Vector2.MoveTowards(_movement,value,acceleration*Time.fixedDeltaTime);
				if(_movement.magnitude > 1){	_movement = _movement.normalized;	}
			}
		}

		public void ApplyGravity(bool xPressed, bool yPressed){
			if (!xPressed && !yPressed) {
				_movement = Vector2.MoveTowards(_movement,Vector2.zero,gravity*Time.fixedDeltaTime);
			} else {
				if(!xPressed){	_movement.x = Mathf.MoveTowards(_movement.x,0,gravity*Time.fixedDeltaTime);	}
				if(!yPressed){	_movement.y = Mathf.MoveTowards(_movement.y,0,gravity*Time.fixedDeltaTime);	}
			}

		}
	}
}