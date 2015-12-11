using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour
{
	public float speed;

	// Update is called once per frame
	void Update ()
	{
		if (Input.GetAxis ("Vertical") != 0 || Input.GetAxis ("Horizontal") != 0)
		{
			Vector2 axis = new Vector2 (Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical"));
			transform.Translate (axis * Time.deltaTime * speed);
		}
	}
}
