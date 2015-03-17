using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameController : MonoBehaviour
{
    public static GameController instance = null;
    private BoardController boardController;

    public int playerLifePoints = 3;

    private GameObject BGImage;
    private Text gameOverText;

    public bool raceFinished = false; // if true, the race JUST finished

    public int passedTiles; // tiles the Player has been through
    private int previouslyPassedTiles;

    // Use this for initialization
    void Awake()
    {
        // make sure there is only one instance of GameController
        if (instance == null) // if it is the first instance
        {
            instance = this;
            DontDestroyOnLoad(this);
        } 
        else if (instance != this) // if we aren't the instance, suicide but call the right init
        {
            // we don't destroy the object because we want to call initScene and that must be done in Start
        }

        // init references
        boardController = GetComponent<BoardController>();
    }
    // Start is called after all the Awakes. this is called JUST for the first instance
    void Start()
    {
        // initialize the scene from the right instance (might not be this one)
        instance.InitCurrentScene(); // must be here otherwise the references might not have been initialized yet
        if(instance != this)
            Destroy(gameObject); // destroy can't be on awake. if it were it wouldn't call start.
    }

    // initalizes the right scene
    void InitCurrentScene()
    {
        switch(Application.loadedLevelName)
        {
            case "Race":
                InitRace();
                break;
            case "Menu": // Menucontroller decides this now
                break;
        }
    }

    // Initialize the race, setting board etc.
    void InitRace()
    {
        // initialize variables
        passedTiles = 0;
        previouslyPassedTiles = 0;
        // set board
        int raceId = 0;
        boardController.BoardInitialSetup(raceId);
        // set current UI
        BGImage = GameObject.Find("BGimage"); // finding by NAME!
        this.gameOverText = GameObject.Find("TextRunOver").GetComponent<Text>();
        BGImage.SetActive(false);
        // set start game
        raceFinished = false;
    }

    // Run has finished
    public void GameOver(bool playerLost)
    {
        // TODO disable controls for the player
        // TODO show X seconds message
        // show message "you lost" or "you won"
        if(playerLost) this.gameOverText.text = "Game Over";
        else gameOverText.text = "VICTORIA!";
        BGImage.SetActive(true);
        raceFinished = true; // it just finished
        // load menu
        StartMenus();
    }
    void StartMenus()
    {
        Application.LoadLevel("Menu");
    }

    // Update is called once per frame
    void Update()
    {
        // if on race, check for generating more track
        if(!raceFinished && previouslyPassedTiles < passedTiles)
        {
            boardController.UpdateBoard(passedTiles);
            previouslyPassedTiles = passedTiles;
        }
    }

    // functions to save and delete game data to a file
    // TODO test. this function is currently not used
    public void Save()
    {
        // open a binary file
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file;
        if (File.Exists(Application.persistentDataPath + "/gameStatus.dat"))
        {
            file = File.Open(Application.persistentDataPath + "/gameStatus.dat", FileMode.Open);
        } else
        {
            file = File.Create(Application.persistentDataPath + "/gameStatus.dat");
        }
        // fill data object and save it
        GameStatusData gameData = new GameStatusData();
        gameData.currentPath = boardController.GetPath();
        bf.Serialize(file, gameData);
        // close the file
        file.Close();
    }

    // TODO test. this function is currently not used
    public void Load()
    {
        // open a binary file
        if (File.Exists(Application.persistentDataPath + "/gameStatus.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gameStatus.dat", FileMode.Open);
            // get data object and save it
            GameStatusData gameData = (GameStatusData)bf.Deserialize(file);
            // close the file
            file.Close();
            // pass data from data object to the current game
            boardController.SetPath(gameData.currentPath);
        }
    }
}

// class to contain the status of the game
[Serializable]
class GameStatusData
{
    // the race map
    // TODO think: should probably be some other class to just store the data, not a Path directy.
    public Path currentPath;
}


