using UnityEngine;
using System.Collections;

public class BulletExample : PooledObject
{
	public float speed;
	public Rigidbody rigidBody;
	void Awake ()
	{
		rigidBody = GetComponent<Rigidbody> ();
	}
	public override void OnEnterPool ()
	{
		base.OnEnterPool ();
		rigidBody.velocity = Vector3.zero;
		rigidBody.angularVelocity = Vector3.zero;
	}
	
	public override void OnExitPool ()
	{
		base.OnExitPool ();
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		transform.Translate (Vector3.forward * Time.deltaTime * speed);
		if (lifetime <= 0) {
			//Use ReturnObject instead of destroy
			pool.ReturnObject (this);
		} else {
			lifetime -= Time.deltaTime;
		}
	}
}
