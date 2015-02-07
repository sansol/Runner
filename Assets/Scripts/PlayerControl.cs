using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {

	public float speed;

	void Update()
	{
/*		if (Input.GetButton ("Fire1") && Time.time > nextFire) 
		{
			//actions taken by the player like jumping or shooting (if any)
		}*/
	}
	
	void FixedUpdate () 
	{
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");
		
		rigidbody.velocity = transform.forward * moveVertical * speed;
		rigidbody.angularVelocity = new Vector3 (0.0f, moveHorizontal, 0.0f) * speed;
	}
}
