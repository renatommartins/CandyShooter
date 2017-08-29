using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
/// <summary>
/// Character. Base Class to derive all Characters in the game.
/// </summary>
public abstract class Character : MonoBehaviour, IDamageable{

	public int hitpoints;
	public float moveMultiplier = 1;
	public int projectileDamage;
	public float rateOfFirePerSec;
	public float projectileSpeed;

	[HideInInspector]
	public Animator animator;

	[SerializeField]
	List<GameObject> projectileList = new List<GameObject>();
	[HideInInspector]
	public float projectileCounter, projectileCooldownTime;
	[HideInInspector]
	public Vector2 moveInput;
	[HideInInspector]
	public Rigidbody2D controlRigidbody;

	public Projectile RequestProjectile(int projectileType){		
		return ProjectileManager.Instance.RequestInstance(projectileList[projectileType]);
	}

	//Callbacks reserved for Character needed instructions ---
	void Awake(){	
		animator = GetComponent<Animator>();
		controlRigidbody = GetComponent<Rigidbody2D>();
		projectileCounter = 1/rateOfFirePerSec;
		CharacterAwake();
	}

	void Start(){
		CharacterStart();
	}

	void Update(){	CharacterUpdate();	}

	void FixedUpdate(){
		Move(moveInput, moveMultiplier);
		CharacterFixedUpdate();
	}
	//--------------------------------------------------------

	/// <summary> (Callback) Substitute of Awake for deriving from Character. DO NOT CALL!!! </summary>
	public abstract void CharacterAwake();
	/// <summary> (Callback) Substitute of Start for deriving from Character. DO NOT CALL!!! </summary>
	public abstract void CharacterStart();
	/// <summary> (Callback) Substitute of FixedUpdate for deriving from Character. DO NOT CALL!!! </summary>
	public abstract void CharacterFixedUpdate();
	/// <summary> (Callback) Substitute of Awake for deriving from Character. DO NOT CALL!!! </summary>
	public abstract void CharacterUpdate();

	/// <summary>
	/// (Callback)
	/// Moves the character with the given input.
	/// multiplier may be used in the logic. DO NOT CALL!!!
	/// </summary>
	/// <param name="input">Input.</param>
	/// <param name="multiplier">Multiplier.</param>
	public abstract void Move(Vector2 input, float multiplier);

	/// <summary>
	/// Takes the damage.
	/// </summary>
	/// <param name="damage">Damage.</param>
	public abstract void TakeDamage(int damage);
}
