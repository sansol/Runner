using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour {

    // TODO the player could keep on losing life while in contact with wall. use oncollisionExit and the Update functions.

	public float speed;
    public int lifePoints; // TODO this info could be in the gamecontroller, saving it there between "levels" or "runs"
    public int wallDamage;
    public Text lifePointsText;

    private Vector2 touchOrigin = -Vector2.one; // is outside the screen

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
	
	void FixedUpdate () // to be used instead of Update if using rigidbody for physics.
	{
        float moveHorizontal = 0.0f;
        float moveVertical = 0.0f;

// TODO depending on the platform, different inputs should be allowed. simply uncomment the right one.
//#if UNITY_EDITOR
        // editor and with a keyboard
		moveHorizontal = Input.GetAxis ("Horizontal");
		moveVertical = Input.GetAxis ("Vertical");
//#else
        // mobile possibilities
        // touch swipes
/*        if(Input.touchCount > 0)
        {
            Touch myTouch = Input.touches[0];

            if(myTouch.phase == TouchPhase.Began) touchOrigin = myTouch.position;
            else if(myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
            {
                Vector2 touchEnd = myTouch.position;
                float x = touchEnd.x - touchOrigin.x;
                float y = touchEnd.y - touchOrigin.y;
                touchOrigin.x = -1;
                if(Mathf.Abs(x) > Mathf.Abs(y)) moveHorizontal = x > 0 ? 1: -1;
                else moveVertical = y > 0 ? 1: -1;
            }
        }*/
        // accelerometer
/*        moveHorizontal = Input.acceleration.x;
        moveVertical = Input.acceleration.y;
*/
//#endif
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
