using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MenuController : MonoBehaviour {

    //canvases holding the different screens of the menu
    private Dictionary<MenuScreenType, GameObject> menuScreens = new Dictionary<MenuScreenType, GameObject>();
    // ref to the game controller
    GameController gameController;

    public void StartRun()
    {
        LoadScene("Race");
    }
    public void StartMenus()
    {
        LoadScene("Menu");
    }
    public void LoadShareScreen()
    {
        SetMenu(MenuScreenType.Share);
    }
    public void LoadAfterRaceScreen()
    {
        SetMenu(MenuScreenType.AfterRace);
    }

    // makes the stated menu screen visibile while loading the right data into it
    public void SetMenu(MenuScreenType menuScreen)
    {
        switch (menuScreen)
        {
            case MenuScreenType.Entrance: // TODO load a message if available
                break;
            case MenuScreenType.AfterRace: // TODO load score
                break;
            case MenuScreenType.Share: // TODO actions to show people to share to
                break;
        }
        ShowMenuScene(menuScreen); // TODO fade in and fade out menus
    }
    // shows the given menu screen while hiding the others
    void ShowMenuScene (MenuScreenType menuScreenToShow)
    {
        // loop all menu screens
        foreach (KeyValuePair<MenuScreenType, GameObject> pair in menuScreens)
        {
            if(pair.Key == menuScreenToShow)
            {
                pair.Value.SetActive(true);
            }
            else
            {
                pair.Value.SetActive(false);
            }
        }
    }

    // Load a scene
    void LoadScene(string sceneName)
    {
        Application.LoadLevel(sceneName);
    }

	// Use this for reference setting
	void Awake ()
    {
        // load menu screen canvases
        menuScreens.Add(MenuScreenType.Entrance, GameObject.Find("CanvasEntrance"));
        menuScreens.Add(MenuScreenType.AfterRace, GameObject.Find("CanvasAfterRace"));
        menuScreens.Add(MenuScreenType.Share, GameObject.Find("CanvasShare"));
    }

    // Use this for initialization. No reference setting
    void Start ()
    {
        // set game controller reference. needs to be here since GameController.Awake is setting the right GameController
        gameController = GameController.instance;

        // decide what menu screen to load
        MenuScreenType screenToLoad = MenuScreenType.Entrance; // default is entrance
        // if race finished, show after race menu screen
        if(gameController.raceFinished)
            screenToLoad = MenuScreenType.AfterRace;
        // load the right screen
        SetMenu(screenToLoad);
    }
	// Update is called once per frame
	void Update () {
	
	}
}

public enum MenuScreenType
{
    Entrance,
    AfterRace,
    Share
}
