using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class BoardController : MonoBehaviour {

    private Path currentPath = new Path();

    // object to hold the whole path
    public GameObject boardHolder;

    // to be able to state the prefabs
    public GameObject tileVertical;
    public GameObject tileCornerSE;
    public GameObject tileArrival;

    private Dictionary<TileType, TileConfiguration> prefabTiles = new Dictionary<TileType, TileConfiguration>();

    public void BoardSetup()
    {
        ConfigurePrefabs();
        TileType[] newPathTileTypes = {
            TileType.Vertical,
            TileType.CornerSW,
            TileType.CornerSE,
            TileType.Vertical,
            TileType.CornerNE,

            TileType.Horizontal,
            TileType.CornerNW,
            TileType.CornerSE,
            TileType.Horizontal,
            TileType.CornerNW,

            TileType.Vertical,
            TileType.CornerSW,
            TileType.CornerSE,
            TileType.CornerNW,
            TileType.CornerNE,

            TileType.CornerSW,
            TileType.Horizontal,
            TileType.Horizontal,
            TileType.Finish
        }; 
        GenerateNewPathFromTileTypes(newPathTileTypes);
        LoadTrack(new Vector3(0.0f, 0.0f, 0.0f));
    }

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
    }

    // given a set of tileTypes, generate a path
    void GenerateNewPathFromTileTypes(TileType[] tileTypeList)
    {
        foreach(TileType currentTileType in tileTypeList)
        {
            currentPath.AddTile(new Tile(currentTileType));
        }
    }

    // load the path in "currentPath" by using the available prefabs
    void LoadTrack(Vector3 origin)
    {
        // last tile (used to add the new one)
        Vector3 currentPosition = origin;
        Direction comingFrom = Direction.South; // the game starts as if the runner came from the south
        foreach( Tile currentTile in currentPath)
        {
            // load the prefab according to the tile
            TileConfiguration currentTileConfig = prefabTiles[currentTile.tileType];
            GameObject toInstantiate = currentTileConfig.tilePrefab;
            GameObject instance = Instantiate(toInstantiate, currentPosition, Quaternion.identity) as GameObject;
            instance.transform.SetParent(boardHolder.transform); // just so the hierarchy shows it nicely
            instance.transform.Rotate(currentTileConfig.tileRotation);
            // calculate next position
            currentPosition = CalculateNextPosition(currentPosition, currentTileConfig.tileType, 10.0f, comingFrom); // 10.0f since the tiles are square with side 1
            comingFrom = CalculateNextDirection(comingFrom, currentTileConfig.tileType);
        }
    }
    // calculates the position for the next tile
    Vector3 CalculateNextPosition(Vector3 currentPosition, TileType currentTileType, float distance, Direction direction)
    {
        Vector3 newPosition = new Vector3(currentPosition.x, currentPosition.y, currentPosition.z);
        switch (currentTileType)
        {
            case TileType.Vertical: // either we are coming from the south or the north
                if(direction == Direction.North) newPosition.z = newPosition.z - distance;
                else newPosition.z = newPosition.z + distance;
                break;
            case TileType.Horizontal:
                if(direction == Direction.East) newPosition.x = newPosition.x - distance;
                else newPosition.x = newPosition.x + distance;
                break;
            case TileType.CornerSE:
                if(direction == Direction.South) newPosition.x = newPosition.x + distance; // towards east
                else newPosition.z = newPosition.z - distance; // towards south
                break;
            case TileType.CornerSW:
                if(direction == Direction.South) newPosition.x = newPosition.x - distance; // towards west
                else newPosition.z = newPosition.z - distance; // towards south
                break;
            case TileType.CornerNE:
                if(direction == Direction.North) newPosition.x = newPosition.x + distance; // towards east
                else newPosition.z = newPosition.z + distance; // towards north
                break;
            case TileType.CornerNW:
                if(direction == Direction.North) newPosition.x = newPosition.x - distance; // towards west
                else newPosition.z = newPosition.z + distance; // towards north
                break;
            case TileType.Finish: // there's no exit. position the same since there shouldn't be anything
            default:
                break;
        }
        return newPosition;
    }
    // calculates the new direction the runner will be after applying the tile (direction for the new position!)
    Direction CalculateNextDirection(Direction direction, TileType currentTileType)
    {
        Direction newDirection = direction;
        switch (currentTileType)
        {
            case TileType.Vertical:
            case TileType.Horizontal: // direction doesn't change since the tiles go straight
                break;
            case TileType.CornerSE: // coming from either south or east
                if(direction == Direction.South) newDirection = Direction.West;
                else newDirection = Direction.North;
                break;
            case TileType.CornerSW: // from south or west
                if(direction == Direction.South) newDirection = Direction.East;
                else newDirection = Direction.North;
                break;
            case TileType.CornerNE:
                if(direction == Direction.North) newDirection = Direction.West;
                else newDirection = Direction.South;
                break;
            case TileType.CornerNW:
                if(direction == Direction.North) newDirection = Direction.East;
                else newDirection = Direction.South;
                break;
            case TileType.Finish: // no new direction since there's no exit
            default:
                break;
        }
        return newDirection;
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
    private ArrayList tileList;

    public Path()
    {
        tileList = new ArrayList();
    }

    public void AddTile(Tile newTile) 
    {
        tileList.Add(newTile);
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

enum Direction
{
    South,
    North,
    East,
    West
}
