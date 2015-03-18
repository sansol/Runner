using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class PlayerControl : MonoBehaviour {

    // TODO the player could keep on losing life while in contact with wall. use oncollisionExit and the Update functions.

    public float speed;
    public Damage damageList;
    public Counter currentScore = new Counter();
    public Counter currentLife = new Counter();

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

        currentLife.setDisplayText("You have ", " life left");

        playerInputEnabled = true; // allow player to control the player object
    }

    void Update()
	{
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
        GetComponent<Rigidbody>().velocity = transform.forward * moveVertical * speed;
        GetComponent<Rigidbody>().angularVelocity = new Vector3 (0.0f, moveHorizontal, 0.0f) * speed;

        // change the animation to the closest direction available
        setAnimation(); //
        // TODO understand if the direction should be fixed as well (matching animation)
    }
    
    // calculates the best animation given the current rotation of the player
    void setAnimation()
    {
        string triggerString = "";
        float rotation = this.GetComponent<Rigidbody>().rotation.eulerAngles.y;
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
                // and one more point
                currentScore.Increase();
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
                Debug.Log(" wall " + damageList.wall);
                currentLife.Decrease(damageList.wall);
                break;
            // TODO add enemy
            default:
                break;
        }

        checkGameOver();
    }

    void checkGameOver()
    {
        if (currentLife.points <= 0)
        {
            // disable player controls. NOT the object
            playerInputEnabled = false;
            // and tell gameController to stop the race
            gameController.GameOver(true);
        }
    }
}

// holds a counter, with initial points and text referene visible in editor
[Serializable]
public class Counter
{
    public Text pointsTextPanel;
    string preceedingText = ""; // text preceeding the points when displayed
    string followingText = ""; // text following the points when displayed
    public int points = 1;

    public void setDisplayText (string pre, string post)
    {
        preceedingText = pre;
        followingText = post;
        UpdateGraphics();
    }
    public void Increase(int newPoints = 1)
    {
        points += newPoints;
        UpdateGraphics();
    }
    public void Decrease(int newPoints = 1)
    {
        points -= newPoints;
        UpdateGraphics();
    }
    void UpdateGraphics()
    {
        pointsTextPanel.text = preceedingText + points + followingText;
    }
}
// holds the damages done by different type of objects. visible in editor
[Serializable]
public class Damage
{
    public int wall = 1;
    public int enemyDamage = 1;
}