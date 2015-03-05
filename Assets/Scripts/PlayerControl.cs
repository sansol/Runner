using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour {

    // TODO the player could keep on losing life while in contact with wall. use oncollisionExit and the Update functions.

	public float speed;
    public int lifePoints; // TODO this info could be in the gamecontroller, saving it there between "levels" or "runs"
    public int wallDamage;
    public Text lifePointsText;

    void Start()
    {
        //lifePoints = 10;
        updateLifeText();
    }
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

    void OnTriggerEnter(Collider other) // check if the object touched other trigger objects
    {
        switch (other.gameObject.tag)
        {
            case "Finish": // final object
                GameController.instance.GameOver(false);
                break;
            default:
                break;
        }
    }

    void OnCollisionEnter(Collision other) 
    {
        switch (other.gameObject.tag)
        {
            case "Wall": // a wall. hurts the player by wallDamage
                lifePoints -= wallDamage; 
                break;
            default:
                break;
        }

        updateLifeText();
        checkGameOver();
    }

    void checkGameOver()
    {
        if (lifePoints <= 0)
        {
            GameController.instance.GameOver(true);
        }
    }

    void updateLifeText()
    {
        lifePointsText.text = "Life left: " + lifePoints;
    }
}
