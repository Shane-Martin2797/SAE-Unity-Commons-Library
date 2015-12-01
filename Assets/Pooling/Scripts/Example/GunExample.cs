using UnityEngine;
using System.Collections;

public class GunExample : MonoBehaviour
{
	public enum TypeOfDelay
	{
		Seconds = 1,
		Update = 2,
		Fires = 3
	}
	
	public PoolingTypeII pool;
	float timer;
	public float firerate = 2;
	public int amountOfObjectsToFire = 1;
	public TypeOfDelay typeOfDelay;
	public float delayBeforeFiring;
	
	void Update ()
	{
		if (delayBeforeFiring <= 0) {
			timer -= Time.deltaTime;
			if (timer <= 0) {
				for (int i = 0; i < amountOfObjectsToFire; ++i) {
					Fire ();
				}
				timer = firerate;
			}
		} else {
			switch (typeOfDelay) {
			case TypeOfDelay.Seconds:
				{
					delayBeforeFiring -= Time.deltaTime;
					break;
				}
			case TypeOfDelay.Update:
				{
					delayBeforeFiring--;
					break;
				}
			case TypeOfDelay.Fires:
				{
					delayBeforeFiring -= (Time.deltaTime / firerate);
					break;
				}
			default:
				{
					Debug.Log ("Pick a type");
					break;
				}
			} 
		}
	}
	
	void Fire ()
	{
		PooledObject pooledObj = pool.GetPooledObject ();
		pooledObj.transform.position = transform.position;
	}
}
