using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// Game system. Manages the state of the game in general.
/// Layer tracking, score update, restarting game...
/// </summary>
public class GameSystem : Singleton<GameSystem> {

	public Vector2 _cameraUpperLeft;

	[SerializeField]
	[Tooltip("Set only one Layer!!!")]
	LayerMask _playerLayer;
	[SerializeField]
	[Tooltip("Set only one Layer!!!")]
	LayerMask _playerProjectileLayer;
	[Space(5)]
	[SerializeField]
	[Tooltip("Set only one Layer!!!")]
	LayerMask _enemyLayer;
	[SerializeField]
	[Tooltip("Set only one Layer!!!")]
	LayerMask _enemyProjectileLayer;
	[Space(5)]
	[SerializeField]
	[Tooltip("Set only one Layer!!!")]
	LayerMask _screenLimitLayer;
	[SerializeField]
	[Tooltip("Set only one Layer!!!")]
	LayerMask _offscreenLimitLayer;

	[Space(10)]
	public GameObject gameoverText;
	public Button restartButton;
	public Text scoreText;
	int score;
	[Space(10)][SerializeField]
	private Slider bossHealthSlider;

	public Text bombText;
	public float bombCooldown;
	[HideInInspector]
	public float bombCounter;
	public static bool bombAvaliable{	get{	return Instance.bombCounter <= 0;	}	}

	/// <summary>
	/// List of all the active Players.
	/// </summary>
	public List<Player> players = new List<Player>();

	/// <summary>
	/// Gets the player layer.
	/// </summary>
	/// <value>The player layer.</value>
	public static int playerLayer			{	get{	return Utils.GetSetBit(Instance._playerLayer.value);			}	}
	/// <summary>
	/// Gets the player projectile layer.
	/// </summary>
	/// <value>The player projectile layer.</value>
	public static int playerProjectileLayer	{	get{	return Utils.GetSetBit(Instance._playerProjectileLayer.value);	}	}
	/// <summary>
	/// Gets the enemy layer.
	/// </summary>
	/// <value>The enemy layer.</value>
	public static int enemyLayer			{	get{	return Utils.GetSetBit(Instance._enemyLayer.value);				}	}
	/// <summary>
	/// Gets the enemy projectile layer.
	/// </summary>
	/// <value>The enemy projectile layer.</value>
	public static int enemyProjectileLayer	{	get{	return Utils.GetSetBit(Instance._enemyProjectileLayer.value);	}	}
	/// <summary>
	/// Gets the screen limit layer.
	/// </summary>
	/// <value>The screen limit layer.</value>
	public static int screenLimitLayer		{	get{	return Utils.GetSetBit(Instance._screenLimitLayer.value);		}	}
	/// <summary>
	/// Gets the offscreen limit layer.
	/// </summary>
	/// <value>The offscreen limit layer.</value>
	public static int offscreenLimitLayer	{	get{	return Utils.GetSetBit(Instance._offscreenLimitLayer.value);	}	}
		
	public static Vector2 cameraUpperLeft {	get{	return Instance._cameraUpperLeft;	}	}

	public static Vector2 GetPositionToCamera(Vector2 relativePosition){
		return (cameraUpperLeft + new Vector2	(relativePosition.x*Camera.main.orthographicSize*2*Camera.main.aspect,
		                  	(-relativePosition.y)*Camera.main.orthographicSize*2));
	}

	void Awake(){

		Time.timeScale = 1;
		players.AddRange(FindObjectsOfType<Player>());
		_cameraUpperLeft = new Vector2(Camera.main.transform.position.x-Camera.main.orthographicSize*Camera.main.aspect,
		                               Camera.main.transform.position.y+Camera.main.orthographicSize);
		bossHealthSlider.gameObject.SetActive(false);
		bombCounter = bombCooldown;
	}

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start(){
		scoreText.text = "Score: 0";
	}

	void Update(){
		if(bombCounter > 0){
			bombCounter -= Time.deltaTime;
			bombText.text = "Next Bomb in " + Mathf.FloorToInt(bombCounter);
		}else{
			bombText.text = " Bomb Avaliable";
		}

		if(players.Find(Player => Player.hitpoints >= 0) == null){
			gameoverText.SetActive(true);
			GameSystem.Instance.restartButton.gameObject.SetActive(true);
			Time.timeScale = 0;
		}
	}

	/// <summary>
	/// Fixed Update.
	/// Adds score over time.
	/// </summary>
	void FixedUpdate(){
		AddScore(10);
	}

	/// <summary>
	/// Restart this instance.
	/// Loads up the scene again.
	/// </summary>
	public void Restart(){
		Application.LoadLevel(0);
	}

	/// <summary>
	/// Adds the given points to score.
	/// </summary>
	/// <param name="points">Points added to the score.</param>
	public void AddScore(int points){
		score += points;
		scoreText.text = "Score: " + score;
	}

	public static Slider RequestBossSlider(){
		if(!Instance.bossHealthSlider.gameObject.activeInHierarchy){
			Instance.bossHealthSlider.gameObject.SetActive(true);
			return Instance.bossHealthSlider;
		}
		else{	return null;	}
	}
}
