using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MenuController : MonoBehaviour {

    //canvases holding the different screens of the menu
    private Dictionary<MenuScreenType, GameObject> menuScreens = new Dictionary<MenuScreenType, GameObject>();

    public void StartRun()
    {
        LoadScene("Race");
    }

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
        ShowMenuScene(menuScreen);
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

	// Use this for initialization
	void Start () 
    {
	    // get references to the different ui parts
        menuScreens.Add(MenuScreenType.Entrance, GameObject.Find("CanvasEntrance"));
        menuScreens.Add(MenuScreenType.AfterRace, GameObject.Find("CanvasAfterRace"));
        menuScreens.Add(MenuScreenType.Share, GameObject.Find("CanvasShare"));
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

