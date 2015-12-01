using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BulletPool : MonoBehaviour {

	public EnemyBulletScript enemyBullet;
	public Bullet playerBullet;
	public int enemyPoolSize = 40;
	public int playerPoolSize = 100;
	public List<EnemyBulletScript> inactiveBullets; 
	public List<Bullet> playerInactiveBullets; 

	void populatePool(){
		for (int i = 0; i < enemyPoolSize; i++){
			buildEnemyBullets();
		}
		for (int j = 0; j < playerPoolSize; j++) {
		//	buildPlayerBullet();
		}
	}


	EnemyBulletScript buildEnemyBullets(){
		EnemyBulletScript newBullet = GameObject.Instantiate(enemyBullet) as EnemyBulletScript;
		newBullet.name = "EnemyBullet";
		newBullet.gameObject.SetActive(false);
		newBullet.transform.SetParent(gameObject.transform);
		inactiveBullets.Add(newBullet);
		return newBullet;
	}
	Bullet buildPlayerBullet(){
		Bullet bullet = GameObject.Instantiate(playerBullet) as Bullet;
		bullet.gameObject.SetActive(false);			
		bullet.transform.SetParent(gameObject.transform);
		playerInactiveBullets.Add(bullet);
		return bullet;
	}
	public void EnemyReturn(EnemyBulletScript bullet){
		bullet.rigidbody.velocity = Vector3.zero;
		bullet.rigidbody.angularVelocity = Vector3.zero;
		bullet.gameObject.SetActive(false);
		bullet.transform.SetParent(gameObject.transform);
		inactiveBullets.Add(bullet);
	}
	public void PlayerReturn(Bullet bullet){
		bullet.rigidbody.velocity = Vector3.zero;
		bullet.rigidbody.angularVelocity = Vector3.zero;
		bullet.gameObject.SetActive(false);
		bullet.transform.SetParent(gameObject.transform);
		playerInactiveBullets.Add(bullet);
	}
	public EnemyBulletScript EnemyGet()
	{
		EnemyBulletScript bullet;
		if (inactiveBullets.Count == 0) {
			Debug.LogWarning ("Bullet Pool too small");
			bullet = buildEnemyBullets ();
		} else {
			bullet = inactiveBullets[0];
		}
		bullet.gameObject.SetActive (true);
		bullet.transform.SetParent (null);
		bullet.pool = this;
		inactiveBullets.Remove (bullet);
		return bullet;
	}

	public Bullet PlayerGet()
	{
		Bullet bullet;
		if (inactiveBullets.Count == 0) {
			Debug.LogWarning ("Bullet Pool too small");
			bullet = buildPlayerBullet ();
		} else {
			bullet = playerInactiveBullets[0];
		}
		
		bullet.gameObject.SetActive (true);
		bullet.transform.SetParent (null);
		playerInactiveBullets.Remove (bullet);
		return bullet;
	}


	// Use this for initialization
	void Start () {
	
	}
	void Awake() {
		inactiveBullets = new List<EnemyBulletScript> ();
		playerInactiveBullets = new List<Bullet> ();
		populatePool ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
