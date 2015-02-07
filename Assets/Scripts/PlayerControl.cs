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
		
		Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);
		rigidbody.velocity = movement*speed;
	}
}
