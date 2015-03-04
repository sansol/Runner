using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameController : MonoBehaviour
{

    public static GameController instance = null;
    private BoardController boardController;

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

    // Update is called once per frame
    void Update()
    {
    }

    // Initialize the game
    void InitGame()
    {
        boardController.BoardSetup();
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



