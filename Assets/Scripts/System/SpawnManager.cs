using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Spawn manager.
/// Handles spawn of all enemies
/// </summary>
public class SpawnManager : Singleton<SpawnManager> {

	public enum ModifierType {	None, Override, Multiply, Add	}

	public List<EnemyWaveSpawn> waveSpawnList = new List<EnemyWaveSpawn>();

	[Tooltip("List of all the enemies that appear on this scene.")]
	public List<EnemySpawn> spawnList = new List<EnemySpawn>();
	List<EnemySpawn> respawningList = new List<EnemySpawn>();

	List<EnemySpawn> spawning = new List<EnemySpawn>();

	float currentTime;

	float nextWaveCount;
	float inWaveCounter;
	int currentWave;
	bool startCountdown = true;
	List<EnemySpawn> waveSpawn = new List<EnemySpawn>();
	List<Enemy> waveEnemiesAlive = new List<Enemy>();


	/// <summary>
	/// Update this instance.
	/// Checks the spawn list if there are spawns pending.
	/// </summary>
	void Update(){
		TimedSpawn();
		WaveSpawn();
	}

	void WaveSpawn(){
		if(currentWave == waveSpawnList.Count){	return;	}
		List<Enemy> removeEnemy = new List<Enemy>();
		foreach(Enemy enemy in waveEnemiesAlive){	if(!enemy.gameObject.activeSelf){	removeEnemy.Add(enemy);	}	}
		for(int i=0; i<removeEnemy.Count;i++){	waveEnemiesAlive.Remove(removeEnemy[i]);	}

		if(nextWaveCount >= waveSpawnList[currentWave].waveCountdown){
			startCountdown = false;
			nextWaveCount = 0;
			waveSpawn = waveSpawnList[currentWave].waveSpawns;
		}
		if(!startCountdown){
			if(waveSpawn.Count > 0){
				List<EnemySpawn> spawning = waveSpawn.FindAll(EnemySpawn => EnemySpawn.time <= inWaveCounter);
				foreach(EnemySpawn spawn in spawning){	waveSpawn.Remove(spawn);	}
				waveEnemiesAlive.AddRange(SpawnEnemyList(spawning));
			}else{
				if(waveEnemiesAlive.Count == 0){
					startCountdown = true;
					inWaveCounter = 0;
					currentWave++;
				}
			}
		}
	}

	void TimedSpawn(){
		// Gets the spawns and add to a list.
		spawning.Clear();
		spawning.AddRange(respawningList.FindAll(EnemySpawn => (EnemySpawn.respawnTimer-(EnemySpawn.shortenRespawns? EnemySpawn.respawnScale.value:0)) <= 0));
		spawning.AddRange(spawnList.FindAll(EnemySpawn => EnemySpawn.time <= currentTime && EnemySpawn.spawned == false));
		// Iterates over the spawning list.

		SpawnEnemyList(spawning);
	}

	List<Enemy> SpawnEnemyList(List<EnemySpawn> enemySpawn){
		List<Enemy> spawnedEnemies = new List<Enemy>();

		foreach(EnemySpawn spawn in enemySpawn){
			spawn.spawned = true;
			// Verifies if the Enemy needs respawning.
			if(spawn.respawns && !respawningList.Contains(spawn)){	respawningList.Add(spawn);	}
			spawn.respawnTimer = spawn.respawnTime;
			// Gets a Enemy instance of the right type.
			Enemy enemy = EnemyManager.Instance.RequestInstance(spawn.prefab);
			// Positions the aquired instance based on the visible area.
			enemy.transform.position = GameSystem.GetPositionToCamera(spawn.startPosition);
			
			// Applies the set modifiers to the Enemy instance.
			switch(spawn.modifiers.modifierType){
			case ModifierType.Override:
				if(spawn.modifiers.enableHitpoints)	enemy.hitpoints = (int) spawn.modifiers.hitpoints;
				if(spawn.modifiers.enableMove)		enemy.moveMultiplier = spawn.modifiers.move;
				if(spawn.modifiers.enableDamage)	enemy.projectileDamage = (int) spawn.modifiers.projectileDamage;
				if(spawn.modifiers.enableSpeed)		enemy.projectileSpeed = spawn.modifiers.projectileSpeed;
				if(spawn.modifiers.enableROF)		enemy.rateOfFirePerSec = spawn.modifiers.rateOfFire;
				if(spawn.modifiers.enableTouch)		enemy.touchDamage = (int) spawn.modifiers.touchDamage;
				if(spawn.modifiers.enableScore)		enemy.defeatScore = (int) spawn.modifiers.score;
				break;
			case ModifierType.Add:
				enemy.hitpoints = (int)( enemy.hitpoints + (spawn.modifiers.enableHitpoints? spawn.modifiers.hitpoints : 0));
				enemy.moveMultiplier = enemy.moveMultiplier + (spawn.modifiers.enableMove? spawn.modifiers.move : 0);
				enemy.projectileDamage = (int)( enemy.projectileDamage + (spawn.modifiers.enableDamage? spawn.modifiers.projectileDamage : 0));
				enemy.projectileSpeed = enemy.projectileSpeed + (spawn.modifiers.enableSpeed? spawn.modifiers.projectileSpeed : 0);
				enemy.rateOfFirePerSec = enemy.rateOfFirePerSec + (spawn.modifiers.enableROF? spawn.modifiers.rateOfFire : 0);
				enemy.touchDamage = (int)( enemy.touchDamage + (spawn.modifiers.enableTouch? spawn.modifiers.touchDamage : 0));
				enemy.defeatScore = (int)( enemy.defeatScore + (spawn.modifiers.enableScore? spawn.modifiers.score : 0));
				break;
			case ModifierType.Multiply:
				enemy.hitpoints = (int)( enemy.hitpoints * (spawn.modifiers.enableHitpoints? spawn.modifiers.hitpoints : 1));
				enemy.moveMultiplier = enemy.moveMultiplier * (spawn.modifiers.enableMove? spawn.modifiers.move : 1);
				enemy.projectileDamage = (int)( enemy.projectileDamage * (spawn.modifiers.enableDamage? spawn.modifiers.projectileDamage : 1));
				enemy.projectileSpeed = enemy.projectileSpeed * (spawn.modifiers.enableSpeed? spawn.modifiers.projectileSpeed : 1);
				enemy.rateOfFirePerSec = enemy.rateOfFirePerSec * (spawn.modifiers.enableROF? spawn.modifiers.rateOfFire : 1);
				enemy.touchDamage = (int)( enemy.touchDamage * (spawn.modifiers.enableTouch? spawn.modifiers.touchDamage : 1));
				enemy.defeatScore = (int)( enemy.defeatScore * (spawn.modifiers.enableScore? spawn.modifiers.score : 1));
				break;
			case ModifierType.None:
				break;
			}
			
			enemy.projectileCounter = 1/enemy.rateOfFirePerSec;
			enemy.EnemySetup();
			spawnedEnemies.Add(enemy);
		}
		return spawnedEnemies;
	}

	/// <summary>
	/// Fixed Update.
	/// Updates the timers.
	/// </summary>
	void FixedUpdate () {
		currentTime += Time.fixedDeltaTime;
		foreach(EnemySpawn spawn in respawningList){
			spawn.respawnTimer -= Time.fixedDeltaTime;
			if(spawn.spawned){	
				spawn.respawnScale.UpdateValue();
				spawn.modifiers.modifierScale.UpdateValue();
			}
		}

		if(startCountdown){	nextWaveCount += Time.fixedDeltaTime;	}
		else{	inWaveCounter += Time.fixedDeltaTime;	}
	}
	
	[System.Serializable]
	/// <summary>
	/// Enemy Spawn.
	/// Class for describing a Enemy Spawn.
	/// </summary>
	public class EnemySpawn{
		[Tooltip("Enemy prefab, drag the prefab from the project folder to here.")]
		public GameObject prefab;
		[Tooltip("The first spawn time.")]
		public float time;
		[Tooltip("Enemy starting position based on the camera. 0-1 is inside camera")]
		public Vector2 startPosition;
		[Tooltip("Sets to respawn.")]
		public bool respawns;
		[Tooltip("Respawn time after first spawn.")]
		public float respawnTime;
		[Tooltip("Modifies the respawn time.")]
		public bool shortenRespawns;
		
		[HideInInspector]
		public bool spawned = false;
		[HideInInspector]
		public float respawnTimer = 0;

		/// <summary> The respawn time modifier. </summary>
		public TimeScale respawnScale = new TimeScale();
		[System.Serializable]
		public class TimeScale{
			[Tooltip("The acceleration per second.")]
			public float acceleration;
			/// <summary> The value of the modifier. </summary>
			[Tooltip("The value of the modifier.")]
			public float value;
			[Tooltip("The maximum value of the modifier.")]
			public float max;

			/// <summary> Updates the value of the modifier. </summary>
			public void UpdateValue(){
				if(value+acceleration*Time.fixedDeltaTime <= max){	value += acceleration*Time.fixedDeltaTime;	}
				else{	value = max;	}
			}
		}

		/// <summary> The enemy instance modifier on this spawn. </summary>
		public EnemyModifier modifiers = new EnemyModifier();

		[System.Serializable]
		///<summary> Class for describing the modifiers of a Enemy spawn. </summary>
		public class EnemyModifier{

			public ModifierType modifierType;
			public bool enableHitpoints;
			public float _hitpoints;
			public bool enableMove;
			public float _move;
			public bool enableDamage;
			public float _projectileDamage;
			public bool enableSpeed;
			public float _projectileSpeed;
			public bool enableROF;
			public float _rateOfFire;
			public bool enableTouch;
			public float _touchDamage;
			public bool enableScore;
			public float _score;

			/// <summary>
			/// Gets the hitpoints already modified.
			/// </summary>
			/// <value>The hitpoints.</value>
			public float hitpoints{			get{	return _hitpoints * (overTime? modifierScale.value : 1);		}	}
			/// <summary>
			/// Gets the move already modified.
			/// </summary>
			/// <value>The move.</value>
			public float move{				get{	return _move * (overTime? modifierScale.value : 1);				}	}
			/// <summary>
			/// Gets the projectile damage already modified.
			/// </summary>
			/// <value>The projectile damage.</value>
			public float projectileDamage{	get{	return _projectileDamage * (overTime? modifierScale.value : 1);	}	}
			/// <summary>
			/// Gets the projectile speed already modified.
			/// </summary>
			/// <value>The projectile speed.</value>
			public float projectileSpeed{	get{	return _projectileSpeed * (overTime? modifierScale.value : 1);	}	}
			/// <summary>
			/// Gets the rate of fire already modified.
			/// </summary>
			/// <value>The rate of fire.</value>
			public float rateOfFire{		get{	return _rateOfFire * (overTime? modifierScale.value : 1);		}	}
			/// <summary>
			/// Gets the touch damage already modified.
			/// </summary>
			/// <value>The touch damage.</value>
			public float touchDamage{		get{	return _touchDamage * (overTime? modifierScale.value : 1);		}	}
			/// <summary>
			/// Gets the score already modified.
			/// </summary>
			/// <value>The score.</value>
			public float score{				get{	return _score * (overTime? modifierScale.value : 1);			}	}

			[Tooltip("Sets the modifiers to change over time.")]
			public bool overTime;
			public TimeScale modifierScale = new TimeScale();
		}
	}

	[System.Serializable]
	public class EnemyWaveSpawn{
		public float waveCountdown;
		public List<EnemySpawn> waveSpawns = new List<EnemySpawn>();
	}
}

