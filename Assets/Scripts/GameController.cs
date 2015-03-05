using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    public static GameController instance = null;
    private BoardController boardController;

    public int playerLifePoints = 3;

    private GameObject BGImage;
    private Text gameOverText;

    // Use this for initialization
    void Awake()
    {
        // make sure there is only one instance of GameController
        if (instance == null)
        {
            instance = this;
        } else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        // get other necessary components
        boardController = GetComponent<BoardController>();
        // initialize the game
        InitGame();
    }

    // Initialize the game
    void InitGame()
    {
        boardController.BoardSetup();
        // load UI variables
        BGImage = GameObject.Find("BGimage"); // finding by NAME!
        gameOverText = GameObject.Find("TextRunOver").GetComponent<Text>();
        // hide UI final elements
        BGImage.SetActive(false);
    }

    // Run has finished
    public void GameOver(bool playerLost)
    {
        // TODO disable controls for the player and pass to controlling menus
        // show message "you lost" or "you won"
        if(playerLost) gameOverText.text = "Game Over";
        else gameOverText.text = "VICTORIA!";
        BGImage.SetActive(true);
        // disable the game controller
        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
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



