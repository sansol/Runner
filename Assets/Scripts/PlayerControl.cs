using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class PlayerControl : MonoBehaviour {

    // TODO the player could keep on losing life while in contact with wall. use oncollisionExit and the Update functions.

	public float speed;
    public int lifePoints; // TODO this info could be in the gamecontroller, saving it there between "levels" or "runs"
    public int wallDamage;
    public Text lifePointsText;

//    private Vector2 touchOrigin = -Vector2.one; // is outside the screen. used if "touch" input is used
    private Animator animator;
    private Transform renderTransform;
    private int currentDirection; // contains discrete direction of the player
    private GameController gameController; // reference to the gameController
    private bool playerInputEnabled;

    void Start()
    {
        // get the child with the animation
        GameObject animationHolder = transform.Find("AnimationHolder").gameObject;
        // gets the Animator and the transform to change them when changing animations
        animator = animationHolder.GetComponent<Animator>();
        renderTransform = animationHolder.transform;
        // get external references
        GameObject gameControllerObject = GameObject.Find("GameController");
        gameController = gameControllerObject.GetComponent<GameController>();

        playerInputEnabled = true; // allow player to control the player object

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

        if(playerInputEnabled){
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
        }
        // move the runner
        rigidbody.velocity = transform.forward * moveVertical * speed;
        rigidbody.angularVelocity = new Vector3 (0.0f, moveHorizontal, 0.0f) * speed;

        // change the animation to the closest direction available
        setAnimation(); //
        // TODO understand if the direction should be fixed as well (matching animation)
    }
    
    // calculates the best animation given the current rotation of the player
    void setAnimation()
    {
        string triggerString = "";
        float rotation = this.rigidbody.rotation.eulerAngles.y;
        // 0 to 359
        int totalDirections = 8; // allowing now cardinals + diagonals
        int degreesOffset = 0; // compensating the isometic camera

        int newDirection = (int) (((rotation + degreesOffset) % 360) * totalDirections / 360);
//        Debug.Log(rotation + " " + newDirection);
        if(currentDirection != newDirection)
        {
            switch(newDirection)
            {
                case 0:
                    triggerString = "toN";
                    break;
                case 1:
                    triggerString = "toNE";
                    break;
                case 2:
                    triggerString = "toE";
                    break;
                case 3:
                    triggerString = "toSE";
                    break;
                case 4:
                    triggerString = "toS";
                    break;
                case 5:
                    triggerString = "toSW";
                    break;
                case 6:
                    triggerString = "toW";
                    break;
                case 7:
                    triggerString = "toNW";
                    break;
            }
            currentDirection = newDirection;
            // update the animator
            animator.SetTrigger(triggerString);
        }
        // set the sprite to render facing camera
//        renderTransform.rotation = Camera.main.transform.rotation;
        renderTransform.rotation = Quaternion.identity;
    }

    void OnTriggerEnter(Collider other) // check if the object touched other trigger objects
    {
        switch (other.gameObject.tag)
        {
            case "Finish": // final object TODO consider removing this, since track will be permanently generated
                gameController.GameOver(false);
                break;
            case "Tile": // each of the tiles
                gameController.passedTiles ++;
                DestroyObject(other.gameObject);
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
            // disable player controls. NOT the object
            playerInputEnabled = false;
            // and tell gameController to stop the race
            gameController.GameOver(true);
        }
    }

    void updateLifeText()
    {
        lifePointsText.text = "Life left: " + lifePoints;
    }
}
