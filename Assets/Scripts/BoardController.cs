using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class BoardController : MonoBehaviour {

    private Path currentPath;

    // object to hold the whole path
    public GameObject boardHolder;

    // to be able to state the prefabs
    public GameObject tileVertical;
    public GameObject tileCornerSE;
    public GameObject tileArrival;

    // holds tiles to be used on board
    private Dictionary<TileType, TileConfiguration> prefabTiles = new Dictionary<TileType, TileConfiguration>();
    bool configuredPrefabTiles = false;
    // hold visited and possible nodes of the board graph, as well as age to delete generated node
    private List<Vector2> generatedBoardLocations = new List<Vector2>();
    private List<GameObject> generatedBoardTiles = new List<GameObject>();
    private int maxOldGeneratedLocation = 6; // if it were 7 or more the current algorithm could lock itself in a loop
    int minimumTilesInPath = 5; // minimum amount of tiles the path always needs to have
    Vector3 currentLoadPosition;
    TileDirection currentLoadComingFrom;
    Vector3 currentPathPosition;
    TileDirection currentPathComingFrom;
    
    System.Random pathGenerator;
    int minimumTilesAhead = 2; // num of tiles the player must have in front at all times
    int loadedTiles; // num of tiles we have loaded since the beginning



    public void BoardInitialSetup(int boardId)
    {
        // get holder for the track
        boardHolder = GameObject.Find("Track"); // the track is destroyed between scenes, so we reload it
        // configure the tiles from the prefabs
        if(!configuredPrefabTiles)
            ConfigurePrefabs();
        // set initial variables
        currentPath = new Path();
        currentPathPosition = new Vector3(0.0f, 0.0f, 0.0f); // origin
        currentPathComingFrom = TileDirection.South; // runner comes from the south
        currentLoadPosition = currentPathPosition;
        currentLoadComingFrom = currentPathComingFrom;
        generatedBoardLocations.Clear();
        generatedBoardTiles.Clear();
        pathGenerator = new System.Random(boardId); // id used as seed
        loadedTiles = 0; // we haven't loaded any tiles yet
        generatedBoardTiles.Clear();

        // generate first path and start adding tiles as done afterwards
        UpdateBoard(0);
    }

    // continue setting the board
    public void UpdateBoard(int passedTiles)
    {
        // check if more tiles are needed
        while(passedTiles + minimumTilesAhead >= loadedTiles)
        {
            // check if more path is needed
            if(currentPath.Count() < minimumTilesInPath)
            {
                GenerateMorePath();
            }
            LoadTile();
        }
        // check if we need to clean tiles
        if(generatedBoardLocations.Count > maxOldGeneratedLocation)
        {
            generatedBoardLocations.RemoveAt(0); // deletes the first one
            DestroyObject(generatedBoardTiles[0]);
            generatedBoardTiles.RemoveAt(0);
        }
    }
    // generate more path random using the given seed
    void GenerateMorePath()
    {
        // calculate possible positions given the running direction and the current position
        List<Vector2> possibleBoardLocations = new List<Vector2>();
        // get all possible 4 tiles if we aren't coming from that direction
        if(currentPathComingFrom != TileDirection.South) 
            possibleBoardLocations.Add(new Vector2(currentPathPosition.x, currentPathPosition.y-1));
        if(currentPathComingFrom != TileDirection.North) 
            possibleBoardLocations.Add(new Vector2(currentPathPosition.x, currentPathPosition.y+1));
        if(currentPathComingFrom != TileDirection.East) 
            possibleBoardLocations.Add(new Vector2(currentPathPosition.x+1, currentPathPosition.y));
        if(currentPathComingFrom != TileDirection.West) 
            possibleBoardLocations.Add(new Vector2(currentPathPosition.x-1, currentPathPosition.y));
        // take out the ones in generatedBoardLocationsf
        for(int i = 0; i < possibleBoardLocations.Count; i++)
        {
            Vector2 currentBoardLocation = possibleBoardLocations[i];
            if(generatedBoardLocations.Contains(currentBoardLocation))
            {
                possibleBoardLocations.Remove(currentBoardLocation);
                i--; // since we just removed one element
            }
        }
        // randomize one of the still remaining possibilities
        Vector2 chosenLocation = possibleBoardLocations[pathGenerator.Next(possibleBoardLocations.Count)];
        generatedBoardLocations.Add(chosenLocation); // save it 
        // calculate the tile that goes to the chosen location
        Tile chosenTile = calculateTileToDirection(chosenLocation);
        // and add it to the right places
        currentPath.AddTile(new Tile(chosenTile.tileType));
        currentPathPosition = new Vector3(chosenLocation.x, chosenLocation.y, 0.0f);
        currentPathComingFrom = CalculateNextDirection(currentPathComingFrom, chosenTile.tileType);
    }
    // calculates the necessary tile to set to go from the current position to the chosenLocation
    Tile calculateTileToDirection(Vector2 chosenLocation)
    {
        Tile newTile = new Tile(TileType.Vertical);
        // use chosenLocation, currentPathPosition and currentPathComingFrom
        float diffMovement = chosenLocation.x - currentPathPosition.x;
        if(diffMovement > 0) // east
        {
            if(currentPathComingFrom == TileDirection.West)
                newTile.tileType = TileType.Horizontal;
            if(currentPathComingFrom == TileDirection.South)
                newTile.tileType = TileType.CornerSE;
            if(currentPathComingFrom == TileDirection.North)
                newTile.tileType = TileType.CornerNE;
        }
        else if(diffMovement < 0) // west
        {
            if(currentPathComingFrom == TileDirection.East)
                newTile.tileType = TileType.Horizontal;
            if(currentPathComingFrom == TileDirection.South)
                newTile.tileType = TileType.CornerSW;
            if(currentPathComingFrom == TileDirection.North)
                newTile.tileType = TileType.CornerNW;
        }
        else
        {
            diffMovement = chosenLocation.y - currentPathPosition.y;
            if(diffMovement > 0) // north
            {
                if(currentPathComingFrom == TileDirection.South)
                    newTile.tileType = TileType.Vertical;
                if(currentPathComingFrom == TileDirection.East)
                    newTile.tileType = TileType.CornerNE;
                if(currentPathComingFrom == TileDirection.West)
                    newTile.tileType = TileType.CornerNW;
            }
            else if(diffMovement < 0) // south
            {
                if(currentPathComingFrom == TileDirection.North)
                    newTile.tileType = TileType.Vertical;
                if(currentPathComingFrom == TileDirection.East)
                    newTile.tileType = TileType.CornerNE;
                if(currentPathComingFrom == TileDirection.West)
                    newTile.tileType = TileType.CornerNW;
            }
        }

        return newTile;
    }
    // loads a tile from path in the current position
    void LoadTile ()
    {
        // get the first tile not yet loaded
        Tile currentTile = currentPath.GetTile();
        // load the prefab according to the tile
        TileConfiguration currentTileConfig = prefabTiles[currentTile.tileType];
        GameObject toInstantiate = currentTileConfig.tilePrefab;
        GameObject instance = Instantiate(toInstantiate, currentLoadPosition, Quaternion.Euler(currentTileConfig.tileRotation)) as GameObject;
        instance.transform.SetParent(boardHolder.transform, false); // all tiles in one track object.
//        instance.transform.Rotate(currentTileConfig.tileRotation);

        // calculate next position elements
        currentLoadPosition = CalculateNextPosition(currentLoadPosition, currentTileConfig.tileType, 10.0f, currentLoadComingFrom); // 10.0f since the tiles are square with side 1
        currentLoadComingFrom = CalculateNextDirection(currentLoadComingFrom, currentTileConfig.tileType);

        // we have now one more tile visible
        loadedTiles++;
        generatedBoardTiles.Add(instance);
    }

    // configure all the tiles to be used based on the available prefabs
    void ConfigurePrefabs()
    {
        // Set reused prefabs rotation
        prefabTiles.Add(TileType.Vertical, new TileConfiguration(TileType.Vertical, tileVertical, new Vector3(0.0f, 0.0f, 0.0f)));
        prefabTiles.Add(TileType.Horizontal, new TileConfiguration(TileType.Horizontal, tileVertical, new Vector3(0.0f, 90.0f, 0.0f)));
        prefabTiles.Add(TileType.CornerSE, new TileConfiguration(TileType.CornerSE, tileCornerSE, new Vector3(0.0f, 0.0f, 0.0f)));
        prefabTiles.Add(TileType.CornerSW, new TileConfiguration(TileType.CornerSW, tileCornerSE, new Vector3(0.0f, 90.0f, 0.0f)));
        prefabTiles.Add(TileType.CornerNE, new TileConfiguration(TileType.CornerNE, tileCornerSE, new Vector3(0.0f, 270.0f, 0.0f)));
        prefabTiles.Add(TileType.CornerNW, new TileConfiguration(TileType.CornerNW, tileCornerSE, new Vector3(0.0f, 180.0f, 0.0f)));
        prefabTiles.Add(TileType.Finish, new TileConfiguration(TileType.Finish, tileArrival, new Vector3(0.0f, 0.0f, 0.0f)));
        configuredPrefabTiles = true;
    }

    // load the path in "currentPath" by using the available prefabs
/*    void LoadTrack(Vector3 origin)
    {
        // last tile (used to add the new one)
        Vector3 currentPosition = origin;
        TileDirection comingFrom = TileDirection.South; // the game starts as if the runner came from the south
        foreach( Tile currentTile in currentPath)
        {
            // load the prefab according to the tile TODO fix so it loads according to the parent rotation and position not the world
            TileConfiguration currentTileConfig = prefabTiles[currentTile.tileType];
            GameObject toInstantiate = currentTileConfig.tilePrefab;
            GameObject instance = Instantiate(toInstantiate, currentPosition, Quaternion.Euler(currentTileConfig.tileRotation)) as GameObject;
//            instance.transform.SetParent(boardHolder.transform, false); // all tiles in one track object.
            Debug.Log("parent angle "+ boardHolder.transform.eulerAngles.ToString());
            instance.transform.Rotate(boardHolder.transform.eulerAngles, Space.World);
            // calculate next position
            currentPosition = CalculateNextPosition(currentPosition, currentTileConfig.tileType, 10.0f, comingFrom); // 10.0f since the tiles are square with side 1
            comingFrom = CalculateNextDirection(comingFrom, currentTileConfig.tileType);
        }
    }*/
    // calculates the position for the next tile
    Vector3 CalculateNextPosition(Vector3 currentPosition, TileType currentTileType, float distance, TileDirection direction)
    {
        Vector3 newPosition = new Vector3(currentPosition.x, currentPosition.y, currentPosition.z);
        switch (currentTileType)
        {
            case TileType.Vertical: // either we are coming from the south or the north
                if(direction == TileDirection.North) newPosition.z = newPosition.z - distance;
                else newPosition.z = newPosition.z + distance;
                break;
            case TileType.Horizontal:
                if(direction == TileDirection.East) newPosition.x = newPosition.x - distance;
                else newPosition.x = newPosition.x + distance;
                break;
            case TileType.CornerSE:
                if(direction == TileDirection.South) newPosition.x = newPosition.x + distance; // towards east
                else newPosition.z = newPosition.z - distance; // towards south
                break;
            case TileType.CornerSW:
                if(direction == TileDirection.South) newPosition.x = newPosition.x - distance; // towards west
                else newPosition.z = newPosition.z - distance; // towards south
                break;
            case TileType.CornerNE:
                if(direction == TileDirection.North) newPosition.x = newPosition.x + distance; // towards east
                else newPosition.z = newPosition.z + distance; // towards north
                break;
            case TileType.CornerNW:
                if(direction == TileDirection.North) newPosition.x = newPosition.x - distance; // towards west
                else newPosition.z = newPosition.z + distance; // towards north
                break;
            case TileType.Finish: // there's no exit. position the same since there shouldn't be anything
            default:
                break;
        }
        return newPosition;
    }
    // calculates the new direction the runner will be after applying the tile (direction for the new position!)
    TileDirection CalculateNextDirection(TileDirection direction, TileType currentTileType)
    {
        TileDirection newDirection = direction;
        switch (currentTileType)
        {
            case TileType.Vertical:
            case TileType.Horizontal: // direction doesn't change since the tiles go straight
                break;
            case TileType.CornerSE: // coming from either south or east
                if(direction == TileDirection.South) newDirection = TileDirection.West;
                else newDirection = TileDirection.North;
                break;
            case TileType.CornerSW: // from south or west
                if(direction == TileDirection.South) newDirection = TileDirection.East;
                else newDirection = TileDirection.North;
                break;
            case TileType.CornerNE:
                if(direction == TileDirection.North) newDirection = TileDirection.West;
                else newDirection = TileDirection.South;
                break;
            case TileType.CornerNW:
                if(direction == TileDirection.North) newDirection = TileDirection.East;
                else newDirection = TileDirection.South;
                break;
            case TileType.Finish: // no new direction since there's no exit
            default:
                break;
        }
        return newDirection;
    }

    // prepares the terrain to have an isometric view, so camera does not need to move 
    void SetIsoView()
    {
        boardHolder.transform.Rotate(0.0f, 315.0f, 0.0f);
    }

    // to be able to save the path in file
    public void SetPath(Path newPath)
    {
        currentPath = newPath;
    }
    public Path GetPath()
    {
        return currentPath;
    }
    
    // Use this for initialization
    // TODO: needed?
    void Start () {
        
    }
    
    // Update is called once per frame
    // TODO: needed?
    void Update () {
        
    }

}

// holds a path to be ran. is an abstraction layer to a sorted List
// TODO check if path needs to be serializable (since it's stored)
[Serializable]
public class Path {

    // list with the tiles forming the path in order
    private List<Tile> tileList;

    public Path()
    {
        tileList = new List<Tile>();
    }

    public void AddTile(Tile newTile) 
    {
        tileList.Add(newTile);
    }

    public Tile GetTile() 
    {
        Tile firstTile = tileList[0];
        tileList.RemoveAt(0);
        return firstTile;
    }

    // get an enumerator of the tiles
    public IEnumerator GetEnumerator()
    {
        return tileList.GetEnumerator();
    }

    public int Count() {
        return tileList.Count;
    }
}

[Serializable]
public class Tile {

    public TileType tileType;

    // given a tile type set the Tile
    public Tile(TileType tileType)
    {
        this.tileType = tileType;
    }
}

// define tile possiblities
public enum TileType
{
    Vertical, // tile goes straight vertically
    Horizontal, // same horizontally
    CornerSE, // tile turns connecting south and east
    CornerSW, 
    CornerNE,
    CornerNW, // tile turns connecting north and east
    Finish
};

public class TileConfiguration 
{
    public TileType tileType;
    public GameObject tilePrefab;
    public Vector3 tileRotation;

    public TileConfiguration (TileType tileType, GameObject tilePrefab, Vector3 tileRotation)
    {
        this.tileType = tileType;
        this.tilePrefab = tilePrefab;
        this.tileRotation = tileRotation;
    }
}

enum TileDirection
{
    South,
    North,
    East,
    West
}